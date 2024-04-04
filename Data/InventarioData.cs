using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text;
using web_hoteldemo.Models;

namespace web_hoteldemo.Data
{
    public class InventarioData
    {

        private static InventarioData? instancia = null;
        public InventarioData()
        {

        }

        public static InventarioData Instancia
        {
            get
            {
                if (instancia == null)
                {
                    instancia = new InventarioData();
                }

                return instancia;
            }
        }

        public List<Inventario> Listar()
        {
            List<Inventario> Lista = new List<Inventario>();
            using (SqlConnection oConexion = new SqlConnection(conexion.CN))
            {
                try
                {
             
                    string query = ("SELECT inventarioID,producto,cantidadDisponible FROM inventario");
                   
                    SqlCommand cmd = new SqlCommand(query, oConexion);
                    cmd.CommandType = CommandType.Text;

                    oConexion.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {

                            Lista.Add(new Inventario()
                            {
                                inventarioID = Convert.ToInt32(dr["inventarioID"]),
                                producto = dr["producto"].ToString(),
                                cantidadDisponible = Convert.ToInt32(dr["cantidadDisponible"].ToString())

                            });

                        }
                    }
                }
                catch (Exception ex)
                {
                    Lista = new List<Inventario>();
                    System.Console.WriteLine(ex.Message);
                }
            }
            return Lista;
        }

        public bool Registrar(Inventario inventario)
        {
            bool respuesta = true;

            Console.WriteLine("Registro en logica resibio : " + inventario.producto );

            using (SqlConnection oConexion = new SqlConnection(conexion.CN) )
            {
                try
                {

                    Console.WriteLine(inventario.producto);
                    SqlCommand cmd = new SqlCommand("RegistrarProducto", oConexion);
                    cmd.Parameters.AddWithValue("producto", inventario.producto);
                    cmd.Parameters.AddWithValue("cantidadDisponible", inventario.cantidadDisponible);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction= ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oConexion.Open();
                    Console.WriteLine("Registro"+ respuesta);  
                    respuesta = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                }
                catch(Exception ex)

                {
                    Console.WriteLine(ex);
                    respuesta =false;
                }
            }

                return respuesta;
        }

        public bool Modificar(Inventario inventario)
        { 
            bool respuesta = true;
            using (SqlConnection oConexion = new SqlConnection(conexion.CN))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("ModificarProducto", oConexion);
                    cmd.Parameters.AddWithValue("inventarioID", inventario.inventarioID);
                    cmd.Parameters.AddWithValue("producto", inventario.producto);
                    cmd.Parameters.AddWithValue("cantidadDisponible", inventario.cantidadDisponible);

                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;

                    oConexion.Open();

                    cmd.ExecuteNonQuery();

                    respuesta = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    respuesta =false;
                }
            }
            return respuesta;
        }
    }

}
