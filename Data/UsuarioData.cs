using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Text;
using web_hoteldemo.Models;
using web_hoteldemo.Models.DB;
using web_hoteldemo.Services;

namespace web_hoteldemo.Data
{
    public class UsuarioData
    {

        private readonly db_adminHotelContext _context;

        public UsuarioData(db_adminHotelContext context)
        {
            _context = context;
        }

        public List<Usuario> Listar()
        {
            List<Usuario> Lista = new List<Usuario>();
            try
            {
                Lista = _context.Usuario
                                .Select(u => new Usuario
                                {
                                    UsuarioId = u.UsuarioId,
                                    NombreUsuario = u.NombreUsuario,
                                    Contraseña = u.Contraseña,
                                    Estado = u.Estado,
                                    Rol = u.Rol
                                })
                                .ToList();
            }
            catch (Exception ex)
            {
                // Manejo de errores, log, etc.
            }
            return Lista;
        }


        public Usuario ObtenerUsuarioPorCredenciales(string nombreUsuario, string contraseña)
        {
            try
            {
                return _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario && u.Contraseña == contraseña);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public int ObtenerId(string nombreUsuario)
        {

            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario );
                if (usuario != null)
                {
                    return usuario.UsuarioId;
                    
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return -1; 
            }



        }

    }
}
