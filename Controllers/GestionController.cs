using Microsoft.AspNetCore.Mvc;
using web_hoteldemo.Models;
using web_hoteldemo.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using Microsoft.CodeAnalysis;
using NuGet.Packaging;

namespace web_hoteldemo.Controllers
{
    public class GestionController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ListarCategoria()
        {
            List<Categoria> oLista = new List<Categoria>();
            
            return Json(new { data = oLista });
        }


        [Authorize]
        public IActionResult Recepcion()

        {
            ViewBag.Message = TempData["Message"];
            return View();
        }

        [HttpGet]
        public JsonResult ActualizarEstadoHabitacion(int habitacionID, int idEstadoHabitacion)
        {
            bool respuesta = false;
            respuesta = HabitacionData.Instancia.ActualizarEstado(habitacionID, idEstadoHabitacion);
            return Json(new { resultado = respuesta });
        }


        [HttpGet]
        public JsonResult ListarHabitacionR(string tipoCategoria)
        {
            List<Habitacion> oLista = new List<Habitacion>();

            

           
            string query = "";
            if (tipoCategoria == "Todas")
            {
                query = "SELECT * FROM Habitacion ORDER BY habitacionID";
            }
            else
            {
                query = "SELECT * FROM Habitacion WHERE tipo = @tipoCategoria ORDER BY habitacionID";
            }

            // Crear una lista para almacenar los resultados
            using (SqlConnection connection = new SqlConnection(conexion.CN))
            {
                // Abrir la conexión
                connection.Open();

                // Crear el comando SQL
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (tipoCategoria != "Todas")
                    {
                        // Agregar el parámetro tipoCategoria si no es "Todas"
                        command.Parameters.AddWithValue("@tipoCategoria", tipoCategoria);
                    }

                    // Ejecutar el comando y leer los resultados
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Crear un objeto Habitacion y agregarlo a la lista
                            Habitacion habitacion = new Habitacion();
                            habitacion.habitacionID = reader.GetInt32(0); 
                            habitacion.tipo = reader.GetString(1);
                            habitacion.idEstadoHabitacion = reader.GetInt32(4);
                                                                               
                            oLista.Add(habitacion);
                        }
                    }
                }
            }

            return Json(new { data = oLista });
        }

        // GET: Gestion
        public ActionResult RecepcionRegistro(int idhabitacion)
        {
            

            Habitacion objeto = HabitacionData.Instancia.Listar().Where(h => h.habitacionID == idhabitacion).FirstOrDefault();
            if (objeto == null)
            {
                // Si no se encuentra la habitación, puedes manejar la situación aquí, por ejemplo, redirigir a una página de error.
                return RedirectToAction("Error");
            }
            return View(objeto);
        }




        [HttpPost]
        public ActionResult RegistrarReservacion(int habitacionID, DateTime fechaEntrada, DateTime fechaSalida, int cantidadHuespedes, string nombreCliente)
        {
            // Acceder al usuario actual
            var usuarioActual = User;

            // Verificar si el usuario actual está autenticado
            if (usuarioActual.Identity.IsAuthenticated)
            {
                // Obtener el usuarioID desde los claims
                var usuarioIDClaim = User.FindFirstValue("usuarioID");

                // Convertir el valor de usuarioIDClaim a entero
                if (int.TryParse(usuarioIDClaim, out int usuarioID))
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(conexion.CN))
                        {
                            using (SqlCommand command = new SqlCommand("RegistrarReservacion", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;

                                command.Parameters.AddWithValue("@habitacionID", habitacionID);
                                command.Parameters.AddWithValue("@usuarioID", usuarioID);
                                command.Parameters.AddWithValue("@fechaEntrada", fechaEntrada);
                                command.Parameters.AddWithValue("@fechaSalida", fechaSalida);
                                command.Parameters.AddWithValue("@cantidadHuespedes", cantidadHuespedes);
                                command.Parameters.AddWithValue("@nombreCliente", nombreCliente);

                                connection.Open();
                                command.ExecuteNonQuery();
                            }
                        }

                        return RedirectToAction("Recepcion", "Gestion");
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Error al registrar la reservación: " + ex.Message);
                        return RedirectToAction("RecepcionRegistro", new { idhabitacion = habitacionID });
                    }
                }
                else
                {
                    System.Console.WriteLine("Error: No se pudo obtener el ID de usuario.");
                    return RedirectToAction("RecepcionRegistro", new { idhabitacion = habitacionID });
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        public ActionResult DetalleRecepcion(int idhabitacion)
        {
            try
            {
                Reservacion oReservacion = ReservacionData.Instancia.Listar().FirstOrDefault(h => h.habitacionID == idhabitacion);
                Habitacion oHabitacion = HabitacionData.Instancia.Listar().FirstOrDefault(h => h.habitacionID == idhabitacion);

                if (oReservacion == null || oHabitacion == null)
                {
                   
                    ViewBag.ErrorMessage = "No se pudo encontrar la reservación o la habitación.";
                    return View("Recepcion");
                }

                DateTime fechaEntrada = oReservacion.fechaEntrada;
                DateTime fechaSalida = oReservacion.fechaSalida;
                TimeSpan diferencia = fechaSalida.Subtract(fechaEntrada);

               

                int diasEstancia = Math.Abs(diferencia.Days);

                double monto = oReservacion.cantidadHuespedes * oHabitacion.precioXpersona;

                double subTotal = monto * diasEstancia;

                double IVA = subTotal * 0.13;

                double Total = subTotal + IVA;

                double montoDolares = Total / 502;

                DetalleRecepcion detalleRecepcion = new DetalleRecepcion
                {
                    oReservacion = oReservacion,
                    oHabitacion = oHabitacion,
                    SubTotal = subTotal,
                    diasEstancia = diasEstancia,
                    IVA = IVA,
                    Total = Total,  
                    montoDolares = montoDolares
                };



                return View(detalleRecepcion);
            }
            catch (Exception ex)
            {
                
                System.Console.WriteLine(ex.Message);
                return View("Recepcion");
            }
        }

        public ActionResult Pago(Pago pago)
        {
            float montoPagado = (float)(pago.montoColones + (pago.montoDolares * 502) + pago.montoTarjeta);
            float montoVuelto = montoPagado - pago.montoTotal;

            // Acceder al usuario actual
            var usuarioActual = User;

            // Verificar si el usuario actual está autenticado
            if (usuarioActual.Identity.IsAuthenticated)
            {
                // Obtener el usuarioID desde los claims
                var usuarioIDClaim = User.FindFirstValue("usuarioID");

                // Convertir el valor de usuarioIDClaim a entero
                if (int.TryParse(usuarioIDClaim, out int usuarioID))
                {
                    try
                    {
                        using (SqlConnection connection = new SqlConnection(conexion.CN))
                        {
                            using (SqlCommand command = new SqlCommand("RegistrarPagoYFactura", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;

                                command.Parameters.AddWithValue("@usuarioID", usuarioID);
                                command.Parameters.AddWithValue("@reservacionID", pago.reservacionID);
                                command.Parameters.AddWithValue("@detalle", pago.detalle);
                                command.Parameters.AddWithValue("@montoTotal", pago.montoTotal);
                                command.Parameters.AddWithValue("@iv", pago.IV);
                                command.Parameters.AddWithValue("@montoPagado", montoPagado);
                                command.Parameters.AddWithValue("@montoVuelto", montoVuelto);
                                command.Parameters.AddWithValue("@observaciones", pago.observaciones);
                                command.Parameters.AddWithValue("@montoDolares", pago.montoDolares);
                                command.Parameters.AddWithValue("@montoColones", pago.montoColones);
                                command.Parameters.AddWithValue("@montoTarjeta", pago.montoTarjeta);

                                connection.Open();
                                command.ExecuteNonQuery();
                            }
                        }
                        TempData["Message"] = "Pago realizado correctamente";
                        return RedirectToAction("Recepcion");
                    }
                    catch(Exception ex)
                    {
                        System.Console.WriteLine("Fallo insercion" + ex.Message);
                        return View("Recepcion");
                    }

                }
                else
                {
                    System.Console.WriteLine("Error: No se pudo obtener el ID de usuario.");
                    return View("Recepcion");
                }

            }
            else
            {
                return Unauthorized();
            }
            

            
        }


        [HttpGet]
        public JsonResult ListarHabitacion()
        {
            List<Habitacion> oLista = new List<Habitacion>();
            try
            {
                // Llama al método Listar de HabitacionData para obtener la lista de habitaciones
                oLista = HabitacionData.Instancia.Listar().OrderBy(o => o.habitacionID).ToList();

               
            }
            catch (Exception ex)
            {
                // Si ocurre un error, registra el mensaje de error
                System.Console.WriteLine("Error al cargar habitaciones: " + ex.Message);
            }


            return Json(new { data = oLista });
        }


    }
}
