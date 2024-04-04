namespace web_hoteldemo.Models
{
    public class Factura
    {
        public int FacturaID { get; set; }
        public int PagoID { get; set; }
        public string Detalle { get; set; }
        public double MontoTotal { get; set; }
        public double IV { get; set; }
        public double MontoPagado { get; set; }
        public double? MontoVuelto { get; set; } // Cambiado a double?
        public string Observaciones { get; set; }
        public DateTime FechaFactura { get; set; }
        public double? montoDolares { get; set; } // Cambiado a double?
        public double? montoColones { get; set; } // Cambiado a double?
        public double? montoTarjeta { get; set; }
    }
}
