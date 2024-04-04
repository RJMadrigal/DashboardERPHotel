using Microsoft.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Text;
using web_hoteldemo.Models;

namespace web_hoteldemo.Data
{
    public class ReservacionData
    {
        private static ReservacionData instancia = null;

        public ReservacionData()
        {

        }

        public static ReservacionData Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new ReservacionData();
                }

                return instancia;
            }
        }

        public List<Reservacion> Listar()
        {
            List<Reservacion> Lista = new List<Reservacion>();
            using (SqlConnection oConexion = new SqlConnection(conexion.CN))
            {
                try
                {
                    string query = "SELECT * FROM reservacion";

                    SqlCommand cmd = new SqlCommand(query, oConexion);
                    cmd.CommandType = CommandType.Text;

                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Lista.Add(new Reservacion()
                            {
                                reservacionID = Convert.ToInt32(dr["reservacionID"]),
                                habitacionID = Convert.ToInt32(dr["habitacionID"]),
                                usuarioID = Convert.ToInt32(dr["usuarioID"]),
                                fechaEntrada = Convert.ToDateTime(dr["fechaEntrada"]),
                                fechaSalida = Convert.ToDateTime(dr["fechaSalida"]),
                                cantidadHuespedes = Convert.ToInt32(dr["cantidadHuespedes"]),
                                nombreCliente = dr["nombreCliente"].ToString()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de excepciones
                    Lista = new List<Reservacion>();
                }
            }
            return Lista;
        }


    }
}
