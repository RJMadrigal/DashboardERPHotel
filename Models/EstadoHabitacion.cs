using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_hoteldemo.Models
{

    [Table("estado_habitacion")]
    public class EstadoHabitacion
    {
        [Key]
        public int idEstadoHabitacion { get; set; }
        public string? descripcion { get; set; }
        public bool estado { get; set; }
    }
}
