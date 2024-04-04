namespace web_hoteldemo.Models
{
    public class DetalleRecepcion
    {
        public Reservacion oReservacion { get;set; }
        public Habitacion oHabitacion { get;set; }

        public int diasEstancia { get;set; }

        public double SubTotal { get;set; }   

        public double IVA { get;set; }

        public double Total { get;set; }

        public double montoDolares { get; set; }
    }
}
