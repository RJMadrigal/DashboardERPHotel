namespace web_hoteldemo.Data
{
    public class RecepcionData
    {
        private static RecepcionData instancia = null;

        public RecepcionData()
        {

        }


        public static RecepcionData Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new RecepcionData();
                }

                return instancia;
            }
        }
    }
}
