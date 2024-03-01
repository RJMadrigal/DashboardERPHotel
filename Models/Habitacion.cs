namespace web_hoteldemo.Models
{
    public class Habitacion
    {

        public int habitacionID { get; set; }
        public string? tipo { get; set; } 
        public string? detalle { get; set; }
        public int? capacidad { get; set; }  
        public  EstadoHabitacion oEstadoHabitacion { get; set; }
        public bool estado { get; set; }    
        public int precioXpersona { get; set; }

    }
}
