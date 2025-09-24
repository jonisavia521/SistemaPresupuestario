namespace SistemaPresupuestario.BLL.Services.Interfaces
{
    /// <summary>
    /// Interface for password hashing operations
    /// Provides secure password hashing with salt
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes a password with a generated salt
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <param name="salt">Generated salt (output parameter)</param>
        /// <returns>Hashed password</returns>
        string HashPassword(string password, out string salt);

        /// <summary>
        /// Hashes a password with a provided salt
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <param name="salt">Salt to use for hashing</param>
        /// <returns>Hashed password</returns>
        string HashPassword(string password, string salt);

        /// <summary>
        /// Verifies a password against a hash and salt
        /// </summary>
        /// <param name="password">Plain text password to verify</param>
        /// <param name="hash">Stored password hash</param>
        /// <param name="salt">Stored salt</param>
        /// <returns>True if password matches, false otherwise</returns>
        bool VerifyPassword(string password, string hash, string salt);

        /// <summary>
        /// Generates a random salt
        /// </summary>
        /// <returns>Base64 encoded salt</returns>
        string GenerateSalt();

        /// <summary>
        /// Validates password strength
        /// </summary>
        /// <param name="password">Password to validate</param>
        /// <returns>Validation result with strength information</returns>
        PasswordValidationResult ValidatePasswordStrength(string password);
    }

    /// <summary>
    /// Result of password strength validation
    /// </summary>
    public class PasswordValidationResult
    {
        public bool IsValid { get; set; }
        public PasswordStrength Strength { get; set; }
        public string[] ValidationErrors { get; set; }
        public string[] Recommendations { get; set; }

        public PasswordValidationResult()
        {
            ValidationErrors = new string[0];
            Recommendations = new string[0];
        }
    }

    /// <summary>
    /// Password strength levels
    /// </summary>
    public enum PasswordStrength
    {
        VeryWeak,
        Weak,
        Fair,
        Strong,
        VeryStrong
    }
}