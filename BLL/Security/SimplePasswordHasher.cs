using System;
using System.Security.Cryptography;
using System.Text;

namespace BLL.Security
{
    /// <summary>
    /// Implementación simple de hash de contraseñas usando SHA256 con salt
    /// NOTA: Para producción se recomienda usar BCrypt o Argon2
    /// </summary>
    public class SimplePasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; // 128 bits
        private const char Delimiter = ':';

        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("La contraseña no puede estar vacía", nameof(password));

            // Generar salt aleatorio
            var salt = GenerateRandomSalt();
            
            // Crear hash con salt
            var hash = ComputeHash(password, salt);
            
            // Combinar salt y hash en formato "salt:hash"
            return Convert.ToBase64String(salt) + Delimiter + Convert.ToBase64String(hash);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
                return false;

            try
            {
                // Separar salt y hash
                var parts = hashedPassword.Split(Delimiter);
                if (parts.Length != 2)
                {
                    // Formato legacy (sin salt) - para retrocompatibilidad
                    return VerifyLegacyPassword(password, hashedPassword);
                }

                var salt = Convert.FromBase64String(parts[0]);
                var storedHash = Convert.FromBase64String(parts[1]);

                // Calcular hash de la contraseña con el salt almacenado
                var computedHash = ComputeHash(password, salt);

                // Comparar hashes
                return AreEqual(storedHash, computedHash);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private byte[] GenerateRandomSalt()
        {
            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private byte[] ComputeHash(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var saltedPassword = new byte[passwordBytes.Length + salt.Length];
                
                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);
                
                return sha256.ComputeHash(saltedPassword);
            }
        }

        private bool AreEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            var result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }
            return result == 0;
        }

        /// <summary>
        /// Verifica contraseñas con formato legacy (para retrocompatibilidad)
        /// </summary>
        private bool VerifyLegacyPassword(string password, string hashedPassword)
        {
            // Implementar según el método actual de hash
            // Por ahora asumimos que es hash simple sin salt
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(passwordBytes);
                var computedHash = Convert.ToBase64String(hash);
                return computedHash == hashedPassword;
            }
        }
    }
}