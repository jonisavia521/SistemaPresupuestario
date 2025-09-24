using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BLL.Infrastructure
{
    /// <summary>
    /// Implementación de IPasswordHasher usando SHA256 con salt aleatorio
    /// DECISIÓN: SHA256 + salt como mejora sobre MD5, preparado para migración futura
    /// Formato almacenado: {salt}.{hash}
    /// </summary>
    public class SimpleSha256PasswordHasher : IPasswordHasher
    {
        private const int MIN_PASSWORD_LENGTH = 8;
        private const int SALT_SIZE = 32; // 32 bytes = 256 bits

        /// <summary>
        /// Genera hash SHA256 de contraseña con salt aleatorio
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>Hash formato: {salt}.{hash}</returns>
        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("La contraseña no puede estar vacía", nameof(password));

            // Generar salt aleatorio
            byte[] saltBytes = new byte[SALT_SIZE];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            string salt = Convert.ToBase64String(saltBytes);
            string hash = ComputeHash(password, salt);

            return $"{salt}.{hash}";
        }

        /// <summary>
        /// Verifica si una contraseña coincide con su hash
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <param name="hashedPassword">Hash almacenado en formato {salt}.{hash}</param>
        /// <returns>True si la contraseña es correcta</returns>
        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
                return false;

            try
            {
                // DECISIÓN: Soportar formato legacy MD5 para migración gradual
                if (!hashedPassword.Contains("."))
                {
                    // Formato legacy - usar verificación MD5 temporal
                    return VerifyLegacyPassword(password, hashedPassword);
                }

                // Formato nuevo SHA256
                var parts = hashedPassword.Split('.');
                if (parts.Length != 2)
                    return false;

                string salt = parts[0];
                string expectedHash = parts[1];
                string actualHash = ComputeHash(password, salt);

                return expectedHash.Equals(actualHash, StringComparison.Ordinal);
            }
            catch (Exception)
            {
                // Log error pero no exponer detalles
                return false;
            }
        }

        /// <summary>
        /// Valida fortaleza de contraseña según reglas de negocio
        /// </summary>
        /// <param name="password">Contraseña a validar</param>
        /// <returns>True si cumple requisitos mínimos</returns>
        public bool ValidatePasswordStrength(string password)
        {
            if (string.IsNullOrEmpty(password))
                return false;

            if (password.Length < MIN_PASSWORD_LENGTH)
                return false;

            // Al menos una letra y un número
            if (!Regex.IsMatch(password, @"[a-zA-Z]") || !Regex.IsMatch(password, @"\d"))
                return false;

            return true;
        }

        /// <summary>
        /// Obtiene descripción de requisitos de contraseña
        /// </summary>
        /// <returns>Texto explicativo para el usuario</returns>
        public string GetPasswordRequirements()
        {
            return $"La contraseña debe tener al menos {MIN_PASSWORD_LENGTH} caracteres, " +
                   "incluyendo al menos una letra y un número.";
        }

        /// <summary>
        /// Computa hash SHA256 de contraseña + salt
        /// </summary>
        /// <param name="password">Contraseña</param>
        /// <param name="salt">Salt en Base64</param>
        /// <returns>Hash SHA256 en Base64</returns>
        private string ComputeHash(string password, string salt)
        {
            string combined = password + salt;
            byte[] inputBytes = Encoding.UTF8.GetBytes(combined);

            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// Verificación temporal para contraseñas MD5 legacy
        /// DECISIÓN: Mantener compatibilidad durante migración
        /// </summary>
        /// <param name="password">Contraseña</param>
        /// <param name="legacyHash">Hash MD5 existente</param>
        /// <returns>True si coincide</returns>
        private bool VerifyLegacyPassword(string password, string legacyHash)
        {
            try
            {
                using (var md5 = MD5.Create())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);
                    string computedHash = Convert.ToBase64String(hashBytes);
                    return legacyHash.Equals(computedHash, StringComparison.Ordinal);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Determina si un hash está en formato legacy
        /// </summary>
        /// <param name="hashedPassword">Hash almacenado</param>
        /// <returns>True si es formato MD5 legacy</returns>
        public bool IsLegacyFormat(string hashedPassword)
        {
            return !string.IsNullOrEmpty(hashedPassword) && !hashedPassword.Contains(".");
        }

        /// <summary>
        /// Migra un hash legacy a formato nuevo
        /// NOTA: Requiere la contraseña en texto plano, usar durante login exitoso
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>Nuevo hash en formato SHA256</returns>
        public string MigrateLegacyPassword(string password)
        {
            return HashPassword(password);
        }
    }
}