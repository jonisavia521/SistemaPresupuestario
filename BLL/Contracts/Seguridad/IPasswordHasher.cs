using System;

namespace BLL.Contracts.Seguridad
{
    /// <summary>
    /// Interfaz para el hashing de contraseñas con salt
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Genera un hash seguro de la contraseña con salt aleatorio
        /// </summary>
        /// <param name="password">Contraseña en texto plano</param>
        /// <returns>Hash en formato {salt}.{hash}</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifica si una contraseña coincide con el hash almacenado
        /// </summary>
        /// <param name="password">Contraseña en texto plano a verificar</param>
        /// <param name="hashedPassword">Hash almacenado en formato {salt}.{hash}</param>
        /// <returns>True si la contraseña coincide</returns>
        bool VerifyPassword(string password, string hashedPassword);

        /// <summary>
        /// Extrae el salt del hash almacenado
        /// </summary>
        /// <param name="hashedPassword">Hash en formato {salt}.{hash}</param>
        /// <returns>Salt en texto plano</returns>
        string ExtractSalt(string hashedPassword);

        /// <summary>
        /// Valida que el hash tenga el formato correcto
        /// </summary>
        /// <param name="hashedPassword">Hash a validar</param>
        /// <returns>True si el formato es válido</returns>
        bool IsValidHashFormat(string hashedPassword);
    }
}