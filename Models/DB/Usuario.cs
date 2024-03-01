using System;
using System.Collections.Generic;

namespace web_hoteldemo.Models.DB
{
    public partial class Usuario
    {
        public int UsuarioId { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Contraseña { get; set; }
        public bool? Estado { get; set; }
        public string? Rol { get; set; }
    }
}
