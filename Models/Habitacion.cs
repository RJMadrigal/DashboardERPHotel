using System.ComponentModel.DataAnnotations.Schema;

namespace web_hoteldemo.Models
{
    public class Habitacion
    {

        public int habitacionID { get; set; }
        public string? tipo { get; set; } 
        public string? detalle { get; set; }
        public int? capacidad { get; set; }
        public int idEstadoHabitacion { get; set; } // Propiedad para almacenar el ID de estado

        [Column("idEstadoHabitacion")] // Nombre de la columna de la clave foránea en la base de datos
        public  EstadoHabitacion? oEstadoHabitacion { get; set; }
        public bool estado { get; set; }    
        public int precioXpersona { get; set; }

    }
}
