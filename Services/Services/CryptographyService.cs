using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    /// <summary>
    /// Servicio de criptografía para el hasheo de contraseñas utilizando el algoritmo MD5.
    /// 
    /// NOTA IMPORTANTE SOBRE SEGURIDAD:
    /// Este servicio utiliza MD5, que es considerado criptográficamente débil para aplicaciones
    /// de seguridad críticas en entornos modernos. MD5 es vulnerable a colisiones y ataques
    /// de fuerza bruta. Para sistemas en producción con altos requerimientos de seguridad,
    /// se recomienda migrar a algoritmos más robustos como bcrypt, PBKDF2 o Argon2.
    /// 
    /// USO ACADÉMICO Y DE DEMOSTRACIÓN:
    /// Este servicio está diseñado con fines educativos y de demostración del patrón
    /// de servicios en una arquitectura en capas. Cumple con los requisitos básicos
    /// del sistema presupuestario, pero debe evaluarse para entornos de producción.
    /// 
    /// CONSISTENCIA Y DETERMINISMO:
    /// Se utiliza UTF8 como codificación estándar para garantizar que el mismo texto
    /// genere el mismo hash en todas las plataformas y ejecuciones. Esto es crítico
    /// para la verificación de contraseñas en diferentes sesiones.
    /// </summary>
    public static class CryptographyService
    {
        /// <summary>
        /// Genera un hash MD5 de una contraseña en texto plano.
        /// 
        /// FLUJO DE PROCESAMIENTO:
        /// 1. Recibe texto plano (contraseña sin encriptar)
        /// 2. Convierte el texto a bytes usando codificación UTF-8
        /// 3. Aplica el algoritmo MD5 para generar el hash
        /// 4. Convierte los bytes del hash a representación hexadecimal
        /// 5. Retorna el string hexadecimal (32 caracteres)
        /// 
        /// FORMATO DE SALIDA:
        /// El hash resultante es un string de 32 caracteres en formato hexadecimal (0-9, a-f).
        /// Ejemplo: "5f4dcc3b5aa765d61d8327deb882cf99" para la contraseña "password"
        /// 
        /// USO EN EL SISTEMA:
        /// - Hasheo de contraseñas al crear usuarios
        /// - Generación de hashes para verificación de login
        /// - Cálculo de dígitos verificadores de integridad (DVH)
        /// 
        /// DETERMINISMO:
        /// La misma entrada siempre produce el mismo hash, lo cual es esencial para:
        /// - Verificación de contraseñas en login
        /// - Validación de integridad de datos
        /// - Consistencia entre diferentes ejecuciones del sistema
        /// </summary>
        /// <param name="textPlainPass">Texto en claro a hashear (contraseña, datos a validar, etc.)</param>
        /// <returns>Hash MD5 en formato hexadecimal (string de 32 caracteres)</returns>
        /// <example>
        /// <code>
        /// string password = "miContraseña123";
        /// string hash = CryptographyService.HashPassword(password);
        /// // hash = "a1b2c3d4e5f6..."
        /// </code>
        /// </example>
        public static string HashPassword(string textPlainPass)
        {
            StringBuilder sb = new StringBuilder();

            using (MD5 md5 = MD5.Create())
            {
                // Usar UTF8 para garantizar determinismo y consistencia cross-platform
                // Esto asegura que el mismo texto genere el mismo hash siempre
                byte[] retVal = md5.ComputeHash(Encoding.UTF8.GetBytes(textPlainPass));
                
                // Convertir cada byte del hash a su representación hexadecimal
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
            }
            return sb.ToString();
        }
    }
}
