using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using web_hoteldemo.Models.DB;

namespace web_hoteldemo.Models
{
    public class Reservacion
    {
        [Key]
        public int reservacionID { get; set; }

        [Required]
        public int habitacionID { get; set; }

        [Required]
        public int usuarioID { get; set; }

        [Column(TypeName = "datetime")]
        
        public DateTime fechaEntrada { get; set; }

        [Column(TypeName = "date")]
        public DateTime fechaSalida { get; set; }

        [Required]
        public int cantidadHuespedes { get; set; }

        [StringLength(50)]
        public string nombreCliente { get; set; }


        // Definición de las relaciones con las clases de Usuario y Habitacion
        [ForeignKey("usuarioID")]
        public Usuario Usuario { get; set; }

        [ForeignKey("hbitacionID")]
        public Habitacion Habitacion { get; set; }

    }
}
