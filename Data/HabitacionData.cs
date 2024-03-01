using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using web_hoteldemo.Models;
using web_hoteldemo.Models.DB;

namespace web_hoteldemo.Data
{
    public class HabitacionData
    {
        private static HabitacionData? instancia = null;

        public HabitacionData()
        {

        }

        public static HabitacionData Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new HabitacionData();
                }

                return instancia;
            }
        }



        public List<Habitacion> Listar()
        {
            List<Habitacion> Lista = new List<Habitacion>();
            using (SqlConnection oConexion = new SqlConnection(conexion.CN))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select h.habitacionID,h.tipo,h.detalle,h.capacidad,h.estado,");
                    query.AppendLine("eh.idEstadoHabitacion,eh.descripcion[descripcion]");
                    query.AppendLine("from habitacion h");
                    query.AppendLine("inner join estado_habitacion eh on eh.idEstadoHabitacion = h.idEstadoHabitacion");

                    SqlCommand cmd = new SqlCommand(query.ToString(), oConexion);
                    cmd.CommandType = CommandType.Text;

                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            
                            Lista.Add(new Habitacion()
                            {
                                habitacionID = Convert.ToInt32(dr["habitacionID"]),
                                tipo = dr["tipo"].ToString(),
                                detalle = dr["detalle"].ToString(),
                                capacidad = Convert.ToInt32(dr["capacidad"].ToString()),

                                oEstadoHabitacion = new EstadoHabitacion() { idEstadoHabitacion = Convert.ToInt32(dr["idEstadoHabitacion"]), descripcion = dr["descripcion"].ToString() },

                                estado = Convert.ToBoolean(dr["estado"])
                            });
                           
                        }
                    }
                }
                catch (Exception ex)
                {
                    Lista = new List<Habitacion>();
                    System.Console.WriteLine(ex.Message);
                }
            }
            return Lista;
        }






        private readonly db_adminHotelContext _context;

        public HabitacionData(db_adminHotelContext context)
        {
            _context = context;
        }
    }
}
