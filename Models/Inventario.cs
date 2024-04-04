using System.ComponentModel.DataAnnotations;

namespace web_hoteldemo.Models
{

    public class Inventario
    {
       [Key]
        public int inventarioID { get; set; }


        public string producto { get; set; }

       
        public int cantidadDisponible { get; set; }

    }




}