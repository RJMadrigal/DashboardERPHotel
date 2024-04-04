namespace web_hoteldemo.Models
{
    public class Pago
    {
        public int? usuarioID { get; set; }
        public int reservacionID { get; set; }
        public string detalle { get; set; }
        public float montoTotal { get; set; }
        public float IV { get; set; }
        public float? montoPagado { get; set; }
        public float? montoVuelto { get; set; }
        public string observaciones { get; set; }
        public float? montoDolares { get; set; }
        public float? montoColones { get; set; }
        public float? montoTarjeta { get; set; }

    }
}
