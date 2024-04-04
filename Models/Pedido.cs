using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using web_hoteldemo.Models.DB;

namespace web_hoteldemo.Models
{
    public class Pedido
    {

     [Key]
    
    public int pedidoID { get; set; }

    public int usuarioID { get; set; }

    public int? inventarioID { get; set; }

    [Required]
    
    public string producto { get; set; }

    
    public string detalle { get; set; }

    public int cantidad { get; set; }

    public bool estado { get; set; }

     public string? seguimiento{ get; set; }

    [ForeignKey("inventarioID")]
    public Inventario? inventario { get; set; }

    [ForeignKey("usuarioID")]
    public Usuario usuario { get; set; }
    }
}
