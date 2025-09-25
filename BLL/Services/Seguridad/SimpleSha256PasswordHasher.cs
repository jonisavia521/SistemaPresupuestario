using BLL.Contracts.Seguridad;
using System;
using System.Security.Cryptography;
using System.Text;

namespace BLL.Services.Seguridad
{
    /// <summary>
    /// Implementación simple de hashing de contraseñas usando SHA256 con salt
    /// DECISIÓN: Implementación básica para compatibilidad, migración futura a algoritmos más seguros recomendada
    /// </summary>
    public class SimpleSha256PasswordHasher : IPasswordHasher
    {
        private const int SaltLength = 16; // 16 bytes = 32 caracteres hex
        private const char Separator = '.';

        /// <summary>
        /// Genera un hash seguro de la contraseña con salt aleatorio
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>Hash en formato {salt}.{hash}</returns>
        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("La contraseña no puede estar vacía", nameof(password));

            // Generar salt aleatorio
            var salt = GenerateRandomSalt();
            
            // Calcular hash
            var hash = CalculateHash(password, salt);
            
            // Retornar en formato {salt}.{hash}
            return $"{salt}{Separator}{hash}";
        }

        /// <summary>
        /// Verifica si una contraseña coincide con el hash almacenado
        /// </summary>
        /// <param name="password">Contraseña en texto plano a verificar</param>
        /// <param name="hashedPassword">Hash almacenado en formato {salt}.{hash}</param>
        /// <returns>True si la contraseña coincide</returns>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
                return false;

            if (!IsValidHashFormat(hashedPassword))
                return false;

            try
            {
                // Extraer salt y hash almacenado
                var parts = hashedPassword.Split(Separator);
                var salt = parts[0];
                var storedHash = parts[1];

                // Calcular hash de la contraseña ingresada con el mismo salt
                var computedHash = CalculateHash(password, salt);

                // Comparar hashes de forma segura (constant-time comparison)
                return SecureStringCompare(storedHash, computedHash);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Extrae el salt del hash almacenado
        /// </summary>
        /// <param name="hashedPassword">Hash en formato {salt}.{hash}</param>
        /// <returns>Salt en texto plano</returns>
        public string ExtractSalt(string hashedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword) || !IsValidHashFormat(hashedPassword))
                return null;

            return hashedPassword.Split(Separator)[0];
        }

        /// <summary>
        /// Valida que el hash tenga el formato correcto
        /// </summary>
        /// <param name="hashedPassword">Hash a validar</param>
        /// <returns>True si el formato es válido</returns>
        public bool IsValidHashFormat(string hashedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword))
                return false;

            var parts = hashedPassword.Split(Separator);
            
            // Debe tener exactamente 2 partes: salt y hash
            if (parts.Length != 2)
                return false;

            // Salt debe tener la longitud esperada (32 caracteres hex = 16 bytes)
            if (parts[0].Length != SaltLength * 2)
                return false;

            // Hash debe tener la longitud de SHA256 (64 caracteres hex = 32 bytes)
            if (parts[1].Length != 64)
                return false;

            // Validar que sean caracteres hexadecimales válidos
            return IsHexString(parts[0]) && IsHexString(parts[1]);
        }

        /// <summary>
        /// Genera un salt aleatorio
        /// </summary>
        /// <returns>Salt en formato hexadecimal</returns>
        private static string GenerateRandomSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var saltBytes = new byte[SaltLength];
                rng.GetBytes(saltBytes);
                return BytesToHexString(saltBytes);
            }
        }

        /// <summary>
        /// Calcula el hash SHA256 de la contraseña con salt
        /// </summary>
        /// <param name="password">Contraseña</param>
        /// <param name="salt">Salt</param>
        /// <returns>Hash en formato hexadecimal</returns>
        private static string CalculateHash(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = salt + password;
                var saltedPasswordBytes = Encoding.UTF8.GetBytes(saltedPassword);
                var hashBytes = sha256.ComputeHash(saltedPasswordBytes);
                return BytesToHexString(hashBytes);
            }
        }

        /// <summary>
        /// Convierte bytes a string hexadecimal
        /// </summary>
        /// <param name="bytes">Bytes a convertir</param>
        /// <returns>String hexadecimal</returns>
        private static string BytesToHexString(byte[] bytes)
        {
            var builder = new StringBuilder(bytes.Length * 2);
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Verifica si un string contiene solo caracteres hexadecimales
        /// </summary>
        /// <param name="str">String a verificar</param>
        /// <returns>True si es hexadecimal válido</returns>
        private static bool IsHexString(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            foreach (char c in str)
            {
                if (!Uri.IsHexDigit(c))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Comparación segura de strings (constant-time) para evitar timing attacks
        /// </summary>
        /// <param name="a">Primer string</param>
        /// <param name="b">Segundo string</param>
        /// <returns>True si son iguales</returns>
        private static bool SecureStringCompare(string a, string b)
        {
            if (a == null || b == null)
                return a == b;

            if (a.Length != b.Length)
                return false;

            var result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }

            return result == 0;
        }
    }
}