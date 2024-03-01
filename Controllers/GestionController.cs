using Microsoft.AspNetCore.Mvc;
using web_hoteldemo.Models;
using web_hoteldemo.Data;
using Microsoft.AspNetCore.Authorization;

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
        public JsonResult ListarHabitacion()
        {
            List<Habitacion> oLista = new List<Habitacion>();
            try
            {
                // Llama al método Listar de HabitacionData para obtener la lista de habitaciones
                oLista = HabitacionData.Instancia.Listar().OrderBy(o => o.habitacionID).ToList();

                // Agrega un mensaje de registro para verificar si se cargaron los datos correctamente
                //System.Console.WriteLine("Se cargaron " + oLista.Count + " habitaciones correctamente.");

               
            }
            catch (Exception ex)
            {
                // Si ocurre un error, registra el mensaje de error
                System.Console.WriteLine("Error al cargar habitaciones: " + ex.Message);
            }

            // Devuelve los datos como JSON
            return Json(new { data = oLista });
        }


    }
}
