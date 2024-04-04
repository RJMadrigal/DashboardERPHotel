using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using web_hoteldemo.Data;
using web_hoteldemo.Models;

namespace web_hoteldemo.Controllers
{
    public class ReporteController : Controller
    {
        public IActionResult IndexFinanciero()
        {
            FlujoDeCaja();
            return View();
        }

        public IActionResult IndexBitacora()
        {
            return View();
        }
        public IActionResult IndexRecepciones()
        {
            return View();
        }
        public IActionResult IndexVentasDia()
        {
            return View();
        }


        public JsonResult ListarBitacora()
        {
            List<Bitacora> oLista = new List<Bitacora>();
            try
            {
                // Llama al método Listar
                oLista = ReporteData.Instancia.Listar();

           

            }
            catch (Exception ex)
            {
                // Si ocurre un error, registra el mensaje de error
                System.Console.WriteLine("Error al cargar datos de la bitacora: " + ex.Message);
            }

            System.Console.WriteLine(oLista);
            // Devuelve los datos como JSON
            return Json(new { data = oLista });

        }

        public JsonResult ListarFacturas()
        {
            List<Factura> oLista = new List<Factura>();
            try
            {
                // Llama al método Listar
                oLista = ReporteData.Instancia.ListarFacturas();



            }
            catch (Exception ex)
            {
                // Si ocurre un error, registra el mensaje de error
                System.Console.WriteLine("Error al cargar datos de la FACTURAS: " + ex.Message);
            }

            System.Console.WriteLine(oLista);
            // Devuelve los datos como JSON
            return Json(new { data = oLista });

        }





        public JsonResult ListarRecepciones()
        {
            List<Reservacion> oLista = new List<Reservacion>();
            try
            {
                // Llama al método Listar
                oLista = ReporteData.Instancia.ListarReservacion();

            }
            catch (Exception ex)
            {
                // Si ocurre un error, registra el mensaje de error
                System.Console.WriteLine("Error al cargar datos de la reservacion: " + ex.Message);
            }

            System.Console.WriteLine(oLista);
            // Devuelve los datos como JSON
            return Json(new { data = oLista });

        }

        public ActionResult FlujoDeCaja()
        {
            double total = 0;
            double totalBoucher = 0;
            double totalDolares = 0;

            using (SqlConnection connection = new SqlConnection(conexion.CN))
            {
                string query = "SELECT SUM(montoTotal) AS Total, SUM(montoTarjeta) AS TotalTarjeta, SUM(montoDolares) AS TotalDolares FROM factura";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    total = (double)reader["Total"];
                    totalDolares = (double)reader["TotalDolares"];
                    totalBoucher = (double)reader["TotalTarjeta"];
                }
                reader.Close();
            }

            ViewBag.TotalEfectivo = total;
            ViewBag.TotalBoucher = totalBoucher;
            ViewBag.TotalDolares = totalDolares;
            

            return View("IndexFinanciero");
        }

        public ActionResult VentasDelDia(DateTime fecha)
        {
            double ventasColones = 0;
            double ventasDolares = 0;
            double ventasTarjeta = 0;
            double ventasTotales = 0;

            using (SqlConnection connection = new SqlConnection(conexion.CN))
            {
                string query = @"
            SELECT COALESCE(SUM(montoColones), 0) AS VentasColones, 
                   COALESCE(SUM(montoDolares), 0) AS VentasDolares, 
                   COALESCE(SUM(montoTarjeta), 0) AS VentasTarjeta,
                   COALESCE(SUM(montoTotal), 0) AS VentasTotales
            FROM factura 
            WHERE CONVERT(DATE, fechaFactura) = CONVERT(DATE, @fecha)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@fecha", fecha);
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    object colones = reader["VentasColones"];
                    object dolares = reader["VentasDolares"];
                    object tarjeta = reader["VentasTarjeta"];
                    object montoTotal = reader["VentasTotales"];

                    // Verificar si los valores son DBNull o 0
                    ventasColones = colones != DBNull.Value ? (double)colones : 0;
                    ventasDolares = dolares != DBNull.Value ? (double)dolares : 0;
                    ventasTarjeta = tarjeta != DBNull.Value ? (double)tarjeta : 0;
                    ventasTotales = montoTotal != DBNull.Value ? (double)montoTotal : 0;
                }
                reader.Close();
            }

            ViewBag.VentasColones = ventasColones;
            ViewBag.VentasDolares = ventasDolares;
            ViewBag.VentasTarjeta = ventasTarjeta;
            ViewBag.VentasTotales = ventasTotales;

            return View("IndexVentasDia");
        }

    }

}
