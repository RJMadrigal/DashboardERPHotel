using Microsoft.Build.Construction;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.AccessControl;
using System.Text;
using web_hoteldemo.Models;

namespace web_hoteldemo.Data
{
    public class ReporteData
    {
        private static ReporteData? instancia = null;

        public ReporteData()
        {

        }

        public static ReporteData Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new ReporteData();
                }

                return instancia;
            }
        }


        public List<Factura> ListarFacturas()
        {
            List<Factura> listaFacturas = new List<Factura>();

            using (SqlConnection oConexion = new SqlConnection(conexion.CN))
            {
                try
                {
                    string query = "SELECT * FROM factura";
                    SqlCommand cmd = new SqlCommand(query, oConexion);
                    cmd.CommandType = CommandType.Text;
                    oConexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Factura factura = new Factura()
                            {
                                FacturaID = Convert.ToInt32(dr["facturaID"]),
                                PagoID = Convert.ToInt32(dr["pagoID"]),
                                Detalle = dr["detalle"].ToString(),
                                MontoTotal = Convert.ToDouble(dr["montoTotal"]),
                                IV = Convert.ToDouble(dr["iv"]),
                                MontoPagado = Convert.ToDouble(dr["montoPagado"]),
                                MontoVuelto = dr["montoVuelto"] == DBNull.Value ? 0 : Convert.ToDouble(dr["montoVuelto"]),
                                Observaciones = dr["observaciones"].ToString(),
                                FechaFactura = Convert.ToDateTime(dr["fechaFactura"]),
                                montoDolares = dr["MontoDolares"] == DBNull.Value ? 0 : Convert.ToDouble(dr["MontoDolares"]),
                                montoColones = dr["MontoColones"] == DBNull.Value ? 0 : Convert.ToDouble(dr["MontoColones"]),
                                montoTarjeta = dr["MontoTarjeta"] == DBNull.Value ? 0 : Convert.ToDouble(dr["MontoTarjeta"])
                            };

                            listaFacturas.Add(factura);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al intentar leer la base de datos:");
                    Console.WriteLine(ex.Message);
                }
            }

            return listaFacturas;
        }



        public List<Reservacion> ListarReservacion()
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
                            Console.WriteLine("Leyendo fila de la base de datos...");

                            Reservacion reservacion = new Reservacion()
                            {
                                reservacionID = Convert.ToInt32(dr["reservacionID"]),
                                habitacionID = Convert.ToInt32(dr["habitacionID"]),
                                usuarioID = Convert.ToInt32(dr["habitacionID"]),
                                fechaEntrada = dr.IsDBNull(dr.GetOrdinal("fechaEntrada")) ? DateTime.MinValue : (DateTime)dr["fechaEntrada"],
                                fechaSalida = dr.IsDBNull(dr.GetOrdinal("fechaSalida")) ? DateTime.MinValue : (DateTime)dr["fechaSalida"],
                                cantidadHuespedes = Convert.ToInt32(dr["cantidadHuespedes"]),
                                nombreCliente = dr["nombreCliente"].ToString()

                            };

                            Lista.Add(reservacion);
                        }
                    }

                }catch(Exception ex) 
                {
                    Lista = new List<Reservacion>();
                    Console.WriteLine("Error al intentar leer la base de datos:");
                    Console.WriteLine(ex.Message);
                }

            }
            return Lista;
        }


        public List<Bitacora> Listar()
        {
            List<Bitacora> Lista = new List<Bitacora>();
            using (SqlConnection oConexion = new SqlConnection(conexion.CN))
            {
                try
                {
                    string query = "SELECT * FROM bitacora";

                    SqlCommand cmd = new SqlCommand(query, oConexion);
                    cmd.CommandType = CommandType.Text;

                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Console.WriteLine("Leyendo fila de la base de datos...");

                            Bitacora bitacora = new Bitacora()
                            {
                                sesionId = Convert.ToInt32(dr["sesionID"]),
                                usuarioID = Convert.ToInt32(dr["usuarioID"]),
                                fecha = dr.IsDBNull(dr.GetOrdinal("fecha")) ? DateTime.MinValue : (DateTime)dr["fecha"],
                                horaEntrada = dr.IsDBNull(dr.GetOrdinal("horaEntrada")) ? TimeSpan.MinValue : (TimeSpan)dr["horaEntrada"]
                            };

                            // Verificar si los campos opcionales son DBNull antes de convertirlos
                            if (!dr.IsDBNull(dr.GetOrdinal("horaSalida")))
                            {
                                bitacora.horaSalida = (TimeSpan)dr["horaSalida"];
                            }
                            else
                            {
                                Console.WriteLine("La hora de salida es nula.");
                            }

                            if (!dr.IsDBNull(dr.GetOrdinal("comentario")))
                            {
                                bitacora.comentario = dr["comentario"].ToString();
                            }
                            else
                            {
                                Console.WriteLine("El comentario es nulo.");
                            }

                            

                            Lista.Add(bitacora);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Lista = new List<Bitacora>();
                    Console.WriteLine("Error al intentar leer la base de datos:");
                    Console.WriteLine(ex.Message);
                }
            }
            return Lista;
        }



    }
}
