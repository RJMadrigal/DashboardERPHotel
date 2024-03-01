using System.Security.Cryptography;
using System.Text;

namespace web_hoteldemo.Services
{
    public class UtilidadServicio
    {

        public static string ConvertirSHA256(string contraseña)
        {

            if (contraseña == null)
            {
                throw new ArgumentNullException(nameof(contraseña), "El argumento 'contraseña' no puede ser nulo.");
            }
            string hash = string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                // obtener el hash del contraseña recibido
                byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(contraseña));

                // convertir el array byte en cadena de contraseña
                foreach (byte b in hashValue)
                    hash += $"{b:X2}";

              

                return hash;

            }
          
        }
    }
}
