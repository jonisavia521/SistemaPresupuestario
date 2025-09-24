using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using SistemaPresupuestario.BLL.Services.Interfaces;

namespace SistemaPresupuestario.BLL.Services.Implementations
{
    /// <summary>
    /// Implementation of password hashing using SHA256 with salt
    /// Provides secure password storage and validation
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 32; // 256 bits
        private const int MinPasswordLength = 6;
        private const int RecommendedPasswordLength = 8;

        public string HashPassword(string password, out string salt)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            salt = GenerateSalt();
            return HashPassword(password, salt);
        }

        public string HashPassword(string password, string salt)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            if (string.IsNullOrWhiteSpace(salt))
                throw new ArgumentException("Salt cannot be null or empty", nameof(salt));

            try
            {
                using (var sha256 = SHA256.Create())
                {
                    // Combine password and salt
                    var passwordBytes = Encoding.UTF8.GetBytes(password);
                    var saltBytes = Convert.FromBase64String(salt);
                    var combinedBytes = new byte[passwordBytes.Length + saltBytes.Length];
                    
                    Array.Copy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
                    Array.Copy(saltBytes, 0, combinedBytes, passwordBytes.Length, saltBytes.Length);

                    // Hash multiple times for additional security
                    var hash = combinedBytes;
                    for (int i = 0; i < 10000; i++) // 10,000 iterations
                    {
                        hash = sha256.ComputeHash(hash);
                    }

                    return Convert.ToBase64String(hash);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error hashing password", ex);
            }
        }

        public bool VerifyPassword(string password, string hash, string salt)
        {
            if (string.IsNullOrWhiteSpace(password) || 
                string.IsNullOrWhiteSpace(hash) || 
                string.IsNullOrWhiteSpace(salt))
            {
                return false;
            }

            try
            {
                var computedHash = HashPassword(password, salt);
                return string.Equals(hash, computedHash, StringComparison.Ordinal);
            }
            catch
            {
                return false;
            }
        }

        public string GenerateSalt()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var saltBytes = new byte[SaltSize];
                rng.GetBytes(saltBytes);
                return Convert.ToBase64String(saltBytes);
            }
        }

        public PasswordValidationResult ValidatePasswordStrength(string password)
        {
            var result = new PasswordValidationResult();
            var errors = new List<string>();
            var recommendations = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                result.IsValid = false;
                result.Strength = PasswordStrength.VeryWeak;
                result.ValidationErrors = new[] { "La contraseña es obligatoria" };
                return result;
            }

            // Check minimum length
            if (password.Length < MinPasswordLength)
            {
                errors.Add($"La contraseña debe tener al menos {MinPasswordLength} caracteres");
            }

            // Check for various character types
            var hasLower = password.Any(char.IsLower);
            var hasUpper = password.Any(char.IsUpper);
            var hasDigit = password.Any(char.IsDigit);
            var hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

            var criteriaCount = 0;
            if (hasLower) criteriaCount++;
            if (hasUpper) criteriaCount++;
            if (hasDigit) criteriaCount++;
            if (hasSpecial) criteriaCount++;

            // Basic validation rules
            if (!hasLower && !hasUpper)
            {
                errors.Add("La contraseña debe contener al menos una letra");
            }

            // Recommendations
            if (password.Length < RecommendedPasswordLength)
            {
                recommendations.Add($"Se recomienda usar al menos {RecommendedPasswordLength} caracteres");
            }

            if (!hasUpper)
            {
                recommendations.Add("Incluya al menos una letra mayúscula");
            }

            if (!hasLower)
            {
                recommendations.Add("Incluya al menos una letra minúscula");
            }

            if (!hasDigit)
            {
                recommendations.Add("Incluya al menos un número");
            }

            if (!hasSpecial)
            {
                recommendations.Add("Incluya al menos un carácter especial (!@#$%^&*)");
            }

            // Check for common weak patterns
            if (IsCommonWeakPassword(password))
            {
                errors.Add("La contraseña es muy común o predecible");
                recommendations.Add("Evite contraseñas comunes como '123456', 'password', etc.");
            }

            // Check for repeated characters
            if (HasRepeatedCharacters(password))
            {
                recommendations.Add("Evite repetir el mismo carácter consecutivamente");
            }

            // Calculate strength
            result.Strength = CalculatePasswordStrength(password, criteriaCount);
            result.IsValid = errors.Count == 0 && result.Strength >= PasswordStrength.Fair;
            result.ValidationErrors = errors.ToArray();
            result.Recommendations = recommendations.ToArray();

            return result;
        }

        private PasswordStrength CalculatePasswordStrength(string password, int criteriaCount)
        {
            var score = 0;

            // Length scoring
            if (password.Length >= 8) score += 2;
            else if (password.Length >= 6) score += 1;

            // Character variety scoring
            score += criteriaCount;

            // Bonus for longer passwords
            if (password.Length >= 12) score += 1;
            if (password.Length >= 16) score += 1;

            // Penalty for common patterns
            if (IsCommonWeakPassword(password)) score -= 3;
            if (HasRepeatedCharacters(password)) score -= 1;

            return score switch
            {
                >= 8 => PasswordStrength.VeryStrong,
                >= 6 => PasswordStrength.Strong,
                >= 4 => PasswordStrength.Fair,
                >= 2 => PasswordStrength.Weak,
                _ => PasswordStrength.VeryWeak
            };
        }

        private bool IsCommonWeakPassword(string password)
        {
            var commonPasswords = new[]
            {
                "123456", "password", "123456789", "12345678", "12345", "1234567",
                "admin", "administrator", "root", "user", "guest", "qwerty", "abc123",
                "letmein", "welcome", "monkey", "dragon", "pass", "master"
            };

            return commonPasswords.Any(common => 
                string.Equals(password, common, StringComparison.OrdinalIgnoreCase));
        }

        private bool HasRepeatedCharacters(string password)
        {
            return Regex.IsMatch(password, @"(.)\1{2,}"); // 3 or more repeated characters
        }
    }
}