using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using web_hoteldemo.Models.DB;

public class Bitacora
{
    [Key]
    public int sesionId { get; set; }

    public int usuarioID { get; set; }

    public DateTime fecha { get; set; }

    public TimeSpan horaEntrada { get; set; }

    public TimeSpan? horaSalida { get; set; }

    [ForeignKey("UsuarioId")]
    public virtual Usuario usuario { get; set; }
}