using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using web_hoteldemo.Data;
using web_hoteldemo.Models;
using web_hoteldemo.Models.DB;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using web_hoteldemo.Services;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace web_hoteldemo.Controllers
{
    public class LoginController : Controller
    {
        private readonly db_adminHotelContext _context;
        private readonly UsuarioData _usuarioData;
        private readonly conexion _conexion;

        public LoginController(db_adminHotelContext context, UsuarioData usuarioData, conexion conexion)
        {
            _context = context;
            _usuarioData = usuarioData;
            _conexion = conexion;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string nombreUsuario, string contraseña)
        {
            var usuario = _usuarioData.ObtenerUsuarioPorCredenciales(nombreUsuario, UtilidadServicio.ConvertirSHA256(contraseña));

            if (usuario != null)
            {
                int usuarioIDParameter = _usuarioData.ObtenerId(nombreUsuario);
                DateTime fechaParameter = DateTime.Today;
                TimeSpan horaEntradaParameter = DateTime.Now.TimeOfDay;

                try
                {
                    using (SqlConnection oConexion = new SqlConnection(conexion.CN))
                    {
                        await oConexion.OpenAsync(); // Abrir la conexión

                        // Crear el comando para ejecutar el procedimiento almacenado
                        using (SqlCommand cmd = new SqlCommand("InsertarBitacora", oConexion))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@usuarioID", usuarioIDParameter);
                            cmd.Parameters.AddWithValue("@fecha", fechaParameter);
                            cmd.Parameters.AddWithValue("@horaEntrada", horaEntradaParameter);

                            // Ejecutar el comando
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Manejar la excepción
                    Console.WriteLine("Error al ejecutar el procedimiento almacenado: " + ex.Message);

                }

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuario.NombreUsuario),
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }
            else
            {

                ViewBag.Message = "Nombre de usuario o contraseña incorrectos";
                return View();
            }
        }

        public async Task<IActionResult> Salir()
        {
            // Obtener el nombre de usuario actualmente autenticado
            var nombreUsuario = User.Identity?.Name;

            // Verificar si el nombre de usuario no es nulo o vacío
            if (!string.IsNullOrEmpty(nombreUsuario))
            {
                // Obtener el ID del usuario utilizando el nombre de usuario
                int idUsuario = _usuarioData.ObtenerId(nombreUsuario);

                // Verificar si se encontró un ID válido
                if (idUsuario != 0)
                {
                    TimeSpan horaSalidaParameter = DateTime.Now.TimeOfDay;

                    // Obtener el registro de la bitácora del usuario actual
                    var bitacoras = await _context.Bitacora
                        .Where(b => b.usuarioID == idUsuario && b.horaSalida == null)
                        .ToListAsync();

                    if (bitacoras != null && bitacoras.Any())
                    {
                        // Actualizar la hora de salida en todos los registros de la bitácora
                        foreach (var bitacora in bitacoras)
                        {
                            bitacora.horaSalida = horaSalidaParameter;
                        }

                        // Guardar los cambios en la base de datos
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    // Manejar el caso en el que no se encuentre un ID válido para el nombre de usuario
                    System.Console.WriteLine("Error: No se encontró un ID válido para el usuario.");
                }
            }
            else
            {
                System.Console.WriteLine("Error: El nombre de usuario es nulo o vacío.");
            }

            // Finalmente, procede a cerrar la sesión como lo estabas haciendo antes
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Login");
        }
    }
}