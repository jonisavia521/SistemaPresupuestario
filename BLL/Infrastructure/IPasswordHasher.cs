using System;

namespace BLL.Infrastructure
{
    /// <summary>
    /// Interfaz para hash seguro de contraseñas
    /// DECISIÓN: Abstraer hashing para permitir migración futura y testing
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Genera hash seguro de una contraseña con salt aleatorio
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>Hash de la contraseña con salt incluido</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifica si una contraseña coincide con su hash
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <param name="hashedPassword">Hash almacenado</param>
        /// <returns>True si la contraseña es correcta</returns>
        bool VerifyPassword(string password, string hashedPassword);

        /// <summary>
        /// Valida si una contraseña cumple con los requisitos mínimos
        /// </summary>
        /// <param name="password">Contraseña a validar</param>
        /// <returns>True si cumple los requisitos</returns>
        bool ValidatePasswordStrength(string password);

        /// <summary>
        /// Obtiene los requisitos de contraseña para mostrar al usuario
        /// </summary>
        /// <returns>Descripción de los requisitos</returns>
        string GetPasswordRequirements();
    }
}