namespace web_hoteldemo.Models
{
    public class UsuarioD
    {
        public int UsuarioId { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Contraseña { get; set; }
        public bool? Estado { get; set; }
        public string? Rol { get; set; }
    }
}
