using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public static class CryptographyService
    {
        public static string HashPassword(string textPlainPass)
        {
            StringBuilder sb = new StringBuilder();

            using (MD5 md5 = MD5.Create())
            {
                // ✅ CORREGIDO: UTF8 es determinista y consistente en todas las plataformas
                byte[] retVal = md5.ComputeHash(Encoding.UTF8.GetBytes(textPlainPass));
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
            }
            return sb.ToString();
        }
    }
}
