using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using web_hoteldemo.Data;
using web_hoteldemo.Models;
using web_hoteldemo.Models.DB;

namespace web_hoteldemo.Controllers
{
    public class HabitacionController : Controller
    {
        private readonly db_adminHotelContext _context;


        public HabitacionController(db_adminHotelContext context)
        {
            _context = context;

        }
        // GET: HabitacionController
        public ActionResult Index()
        {
            return View();
        }


        


        public JsonResult ListarHabitacion()
        {
            List<Habitacion> oLista = new List<Habitacion>();
            try
            {
                // Llama al método Listar
                oLista = HabitacionData.Instancia.Listar();

                // Agrega un mensaje de registro para verificar si se cargaron los datos correctamente
                //System.Console.WriteLine("Se cargaron " + oLista.Count + " productos correctamente.");


            }
            catch (Exception ex)
            {
                // Si ocurre un error, registra el mensaje de error
                System.Console.WriteLine("Error al cargar datos de inventario: " + ex.Message);
            }

            System.Console.WriteLine(oLista);
            // Devuelve los datos como JSON
            return Json(new { data = oLista });

        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Create(Habitacion habitacion)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(conexion.CN))
                {
                    string query = "INSERT INTO habitacion (tipo, detalle, capacidad, idEstadoHabitacion, precioXpersona) VALUES (@Tipo, @Detalle, @Capacidad, @IdEstadoHabitacion, @PrecioXPersona);";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Tipo", habitacion.tipo);
                        command.Parameters.AddWithValue("@Detalle", habitacion.detalle);
                        command.Parameters.AddWithValue("@Capacidad", habitacion.capacidad);
                        command.Parameters.AddWithValue("@IdEstadoHabitacion", habitacion.idEstadoHabitacion);
                        command.Parameters.AddWithValue("@PrecioXPersona", habitacion.precioXpersona);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index", "Habitacion");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);

            }
            return View(habitacion);
        }





        // GET: HabitacionController/Edit/5
        public IActionResult Edit(int id)
        {
            Habitacion habitacion = new Habitacion();

            // Consulta SQL para obtener la habitación por su ID
            string query = "SELECT habitacionID, tipo, detalle, capacidad, idEstadoHabitacion, estado, precioXpersona FROM Habitacion WHERE habitacionID = @HabitacionID";

            using (SqlConnection connection = new SqlConnection(conexion.CN))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@HabitacionID", id);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    habitacion.habitacionID = reader.GetInt32(0);
                    habitacion.tipo = reader.GetString(1);
                    habitacion.detalle = reader.GetString(2);
                    habitacion.capacidad = reader.GetInt32(3);
                    habitacion.idEstadoHabitacion = reader.GetInt32(4);
                    habitacion.estado = reader.GetBoolean(5);
                    habitacion.precioXpersona = reader.GetInt32(6);
                }

                reader.Close();
            }

            // Pasar la habitación a la vista para mostrar el formulario de edición
            return View(habitacion);
        }


        [HttpPost]
        public IActionResult Edit(Habitacion habitacion)
        {
            try
            {


                using (SqlConnection connection = new SqlConnection(conexion.CN))
                {
                    string query = @"UPDATE Habitacion
                                 SET tipo = @Tipo,
                                     detalle = @Detalle,
                                     capacidad = @Capacidad,
                                     idEstadoHabitacion = @IdEstadoHabitacion,
                                     estado = @Estado,
                                     precioXpersona = @PrecioXPersona
                                 WHERE habitacionID = @HabitacionID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Tipo", habitacion.tipo);
                        command.Parameters.AddWithValue("@Detalle", habitacion.detalle);
                        command.Parameters.AddWithValue("@Capacidad", habitacion.capacidad);
                        command.Parameters.AddWithValue("@IdEstadoHabitacion", habitacion.idEstadoHabitacion);
                        command.Parameters.AddWithValue("@Estado", habitacion.estado);
                        command.Parameters.AddWithValue("@PrecioXPersona", habitacion.precioXpersona);
                        command.Parameters.AddWithValue("@HabitacionID", habitacion.habitacionID);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index"); // Redirigir a la acción Index después de editar

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            return View(habitacion);
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
                    string query = "SELECT habitacionID, tipo, detalle, estado FROM habitacion WHERE habitacionID = @HabitacionID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@HabitacionID", id);
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var habitacion = new Habitacion
                                {
                                    habitacionID = reader.GetInt32(0),
                                    tipo = reader.GetString(1),
                                    detalle = reader.GetString(2),
                                    estado = reader.GetBoolean(3),

                                };




                                return View(habitacion);
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

        // POST: HabitacionController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletedConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(conexion.CN))
                {
                    string query = "DELETE FROM habitacion WHERE habitacionID = @HabitacionID";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@HabitacionID", id);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index"); // Redirigir a la acción Index después de eliminar
            }
            catch (Exception ex)
            {
                // Manejar el error
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
