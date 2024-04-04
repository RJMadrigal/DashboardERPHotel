using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using web_hoteldemo.Models;

namespace web_hoteldemo.Data
{
    public class PedidoData
    {

        private static PedidoData? instancia = null;

        public PedidoData()
        {

        }

        public static PedidoData Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new PedidoData();
                }

                return instancia;
            }
        }


        public List<Pedido> Listar(int usuarioID)
        {
            List<Pedido> Lista = new List<Pedido>();



            using (SqlConnection oConexion = new SqlConnection(conexion.CN))
            {
                try
                {
                    string query = "SELECT pedidoID, producto, detalle, cantidad, seguimiento, estado FROM pedidos WHERE usuarioID = @usuarioID";

                    SqlCommand cmd = new SqlCommand(query.ToString(), oConexion);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@usuarioID", usuarioID); // Agrega el parámetro usuarioID


                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {

                            Lista.Add(new Pedido()
                            {
                                pedidoID = Convert.ToInt32(dr["pedidoID"]),
                                producto  = dr["producto"].ToString(),
                                detalle = dr["detalle"].ToString(),
                                cantidad = Convert.ToInt32(dr["cantidad"].ToString()),
                                seguimiento = dr["seguimiento"].ToString(),
                                estado = Convert.ToBoolean(dr["estado"])
                            });

                        }
                    }
                }
                catch (Exception ex)
                {
                    Lista = new List<Pedido>();
                    System.Console.WriteLine(ex.Message);
                }
            }
            return Lista;
        }

        public List<Pedido> ListarGestor()
        {
            List<Pedido> Lista = new List<Pedido>();

            using (SqlConnection oConexion = new SqlConnection(conexion.CN))
            {
                try
                {
                    string query = "SELECT pedidoID, inventarioID, usuarioID, producto, detalle, cantidad, seguimiento, estado FROM pedidos";

                    SqlCommand cmd = new SqlCommand(query.ToString(), oConexion);
                    cmd.CommandType = CommandType.Text;

                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Pedido pedido = new Pedido()
                            {
                                pedidoID = Convert.ToInt32(dr["pedidoID"]),
                                producto = dr["producto"].ToString(),
                                detalle = dr["detalle"].ToString(),
                                cantidad = Convert.ToInt32(dr["cantidad"].ToString()),
                                seguimiento = dr["seguimiento"].ToString(),
                                estado = Convert.ToBoolean(dr["estado"])
                            };

                            // Verificar si los campos opcionales son DBNull antes de convertirlos
                            if (dr["inventarioID"] != DBNull.Value)
                            {
                                pedido.inventarioID = Convert.ToInt32(dr["inventarioID"]);
                            }

                            if (dr["usuarioID"] != DBNull.Value)
                            {
                                pedido.usuarioID = Convert.ToInt32(dr["usuarioID"]);
                            }

                            Lista.Add(pedido);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Lista = new List<Pedido>();
                    System.Console.WriteLine(ex.Message);
                }
            }
            return Lista;
        }
    }
}
