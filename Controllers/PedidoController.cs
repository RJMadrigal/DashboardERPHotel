using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using web_hoteldemo.Data;
using web_hoteldemo.Models;

namespace web_hoteldemo.Controllers
{
    public class PedidoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GestorPedidos()
        {
            return View();
        }
        public JsonResult ListarPedido()
        {
            List<Pedido> oLista = new List<Pedido>();

            try
            {
                // Obtener el usuarioID del claim
                var usuarioIDClaim = HttpContext.User.FindFirstValue("usuarioID");

                // Verificar si el claim existe y si su valor es un número entero
                if (!string.IsNullOrEmpty(usuarioIDClaim) && int.TryParse(usuarioIDClaim, out int usuarioID))
                {
                    // Llamar al método Listar de PedidoData con el usuarioID
                    oLista = PedidoData.Instancia.Listar(usuarioID);
                }
                else
                {
                    // Manejar el caso en el que el claim no esté presente o no sea válido
                    
                    System.Console.WriteLine("El claim 'usuarioID' no está presente o no es válido.");
                }
            }
            catch (Exception ex)
            {
                // Si ocurre un error, registra el mensaje de error
                System.Console.WriteLine("Error al cargar datos de pedidos: " + ex.Message);
            }

            // Devuelve los datos como JSON
            return Json(new { data = oLista });

        }

        public JsonResult ListarGestor()
        {
            List<Pedido> oLista = new List<Pedido>();

            try
            {
               
                    oLista = PedidoData.Instancia.ListarGestor();
               
            }

            catch (Exception ex)
            {
                // Si ocurre un error, registra el mensaje de error
                System.Console.WriteLine("Error al cargar datos de pedidos: " + ex.Message);
            }

            // Devuelve los datos como JSON
            return Json(new { data = oLista });

        }

        // GET: HabitacionController/Edit/5
        public IActionResult Edit(int id)
        {

            // Inicializa la lista de productos del inventario
            ViewBag.ProductosInventario = new List<Inventario>();
            // Instancia de InventarioData para llamar al método Listar
            InventarioData inventarioData = new InventarioData();
            // Obtiene la lista de productos del inventario utilizando el método Listar de InventarioData
            var listaInventario = inventarioData.Listar();
            // Asigna la lista de productos del inventario al ViewBag
            ViewBag.ProductosInventario = listaInventario;


            Pedido pedido = new Pedido();

            // Consulta SQL para obtener la habitación por su ID
            string query = "SELECT pedidoID,inventarioID,usuarioID, producto, detalle, cantidad,seguimiento FROM pedidos WHERE pedidoID = @pedidoID";

            using (SqlConnection connection = new SqlConnection(conexion.CN))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@pedidoID", id);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    pedido.pedidoID = reader.GetInt32(0);
                    pedido.inventarioID = reader.IsDBNull(1) ? null : (int?)reader.GetInt32(1);
                    pedido.usuarioID = reader.GetInt32(2);
                    pedido.producto = reader.GetString(3);
                    pedido.detalle = reader.GetString(4);
                    pedido.cantidad = reader.GetInt32(5);
                    pedido.seguimiento = reader.IsDBNull(6) ? null : reader.GetString(6); 
                    
                }

                reader.Close();

            }

            // Pasar la habitación a la vista para mostrar el formulario de edición
            return View(pedido);
        }






        [HttpPost]
        public IActionResult Edit(Pedido pedido)
        {
            try



            {

              

                using (SqlConnection connection = new SqlConnection(conexion.CN))
                {
                    string query = @"UPDATE pedidos
                             SET inventarioID = @InventarioID,
                                 detalle = @Detalle,
                                 cantidad = @Cantidad,
                                 seguimiento = @Seguimiento
                             WHERE pedidoID = @PedidoID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@InventarioID", pedido.inventarioID ?? (object)DBNull.Value); // Manejar valores null
                        command.Parameters.AddWithValue("@Detalle", pedido.detalle);
                        command.Parameters.AddWithValue("@Cantidad", pedido.cantidad);
                        command.Parameters.AddWithValue("@Seguimiento", pedido.seguimiento);
                        command.Parameters.AddWithValue("@PedidoID", pedido.pedidoID);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return RedirectToAction("GestorPedidos"); // Redirigir a la acción Index después de editar
                        }
                        else
                        {
                            // Si no se actualizó ningún registro, puede ser útil mostrar un mensaje al usuario
                            TempData["ErrorMessage"] = "No se pudo actualizar el pedido.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción y mostrar un mensaje al usuario
                TempData["ErrorMessage"] = "Se produjo un error al intentar actualizar el pedido.";
                System.Console.WriteLine(ex.Message);
            }

            // Si ocurre un error, ocurrirá el flujo aquí y devolverá la vista con el modelo
            return View(pedido);
        }


        public IActionResult ActualizarInventario()
        {

            InventarioData inventarioData = new InventarioData();



            return View();
        }



        [HttpGet]
        public IActionResult Create()
        {   
            // Inicializa la lista de productos del inventario
            ViewBag.ProductosInventario = new List<Inventario>();

            try
            {
                // Instancia de InventarioData para llamar al método Listar
                InventarioData inventarioData = new InventarioData();
                // Obtiene la lista de productos del inventario utilizando el método Listar de InventarioData
                var listaInventario = inventarioData.Listar();
                // Asigna la lista de productos del inventario al ViewBag
                ViewBag.ProductosInventario = listaInventario;
            }
            catch (Exception ex)
            {
                // Maneja cualquier excepción que pueda ocurrir durante la obtención de la lista de productos del inventario
                System.Console.WriteLine("Error al obtener la lista de productos del inventario: " + ex.Message);
            }

            // Retorna la vista de creación con la lista de productos del inventario en el ViewBag
            return View();

        }

        [HttpPost]
        public IActionResult Create(Pedido pedido)
        {
            
            
            try
            {
                using (SqlConnection con = new SqlConnection(conexion.CN))
                {
                    con.Open();

                    string query = @"INSERT INTO pedidos (producto, detalle, cantidad, usuarioID, inventarioID) 
                                    VALUES (@Producto, @Detalle, @Cantidad, @UsuarioID, @InventarioID)";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Producto", pedido.producto);
                    cmd.Parameters.AddWithValue("@Detalle", pedido.detalle);
                    cmd.Parameters.AddWithValue("@Cantidad", pedido.cantidad);
                    cmd.Parameters.AddWithValue("@UsuarioID", pedido.usuarioID);
                    cmd.Parameters.AddWithValue("@InventarioID", pedido.inventarioID.HasValue ? pedido.inventarioID.Value : (object)DBNull.Value);


                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        // Éxito: redirigir a la acción Index
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {

                        System.Console.WriteLine("Error");
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción
                System.Console.WriteLine( ex.Message);
            }

            // Si llega aquí, significa que ocurrió un error: volver a mostrar el formulario
            return View(pedido);

        }

        // GET: HabitacionController/Delete/5
        public IActionResult Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(conexion.CN))
                {
                    string query = "SELECT pedidoID, producto, detalle, cantidad FROM pedidos WHERE pedidoID = @pedidoID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@pedidoID", id);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var pedido = new Pedido
                                {
                                    pedidoID = reader.GetInt32(0),
                                    producto = reader.GetString(1),
                                    detalle = reader.GetString(2),
                                    cantidad = reader.GetInt32(3),
                                    

                                };

                                CheckTempDataForMessages();


                                return View(pedido);
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        private void CheckTempDataForMessages()
        {
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
            }
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(conexion.CN))
                {
                    string query = "SELECT seguimiento FROM pedidos WHERE pedidoID = @PedidoID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PedidoID", id);
                        connection.Open();

                        object seguimiento = command.ExecuteScalar();
                        if (seguimiento != DBNull.Value)
                        {
                            TempData["ErrorMessage"] = "No se puede eliminar el pedido porque ya está en seguimiento.";
                            return RedirectToAction(nameof(Delete), new { id = id });
                        }
                    }

                    query = "DELETE FROM pedidos WHERE pedidoID = @PedidoID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PedidoID", id);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            TempData["SuccessMessage"] = "El pedido se eliminó correctamente.";
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Error interno del servidor");
            }
        }



    }
}
