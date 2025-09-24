using System;

namespace BLL.Exceptions
{
    /// <summary>
    /// Excepción lanzada cuando se detecta un conflicto de concurrencia
    /// DECISIÓN: Usar timestamp (rowversion) para control optimista de concurrencia
    /// </summary>
    public class ConcurrencyException : Exception
    {
        /// <summary>
        /// Identificador de la entidad que tiene conflicto
        /// </summary>
        public Guid EntityId { get; }

        /// <summary>
        /// Tipo de entidad que tiene conflicto
        /// </summary>
        public string EntityType { get; }

        /// <summary>
        /// Timestamp esperado (el que tenía el cliente)
        /// </summary>
        public byte[] ExpectedTimestamp { get; }

        /// <summary>
        /// Timestamp actual en la base de datos
        /// </summary>
        public byte[] CurrentTimestamp { get; }

        /// <summary>
        /// Constructor básico con mensaje
        /// </summary>
        /// <param name="mensaje">Descripción del conflicto</param>
        public ConcurrencyException(string mensaje) : base(mensaje)
        {
        }

        /// <summary>
        /// Constructor con contexto de entidad
        /// </summary>
        /// <param name="mensaje">Descripción del conflicto</param>
        /// <param name="entityId">ID de la entidad en conflicto</param>
        /// <param name="entityType">Tipo de entidad</param>
        public ConcurrencyException(string mensaje, Guid entityId, string entityType) : base(mensaje)
        {
            EntityId = entityId;
            EntityType = entityType;
        }

        /// <summary>
        /// Constructor completo con timestamps
        /// </summary>
        /// <param name="mensaje">Descripción del conflicto</param>
        /// <param name="entityId">ID de la entidad en conflicto</param>
        /// <param name="entityType">Tipo de entidad</param>
        /// <param name="expectedTimestamp">Timestamp esperado</param>
        /// <param name="currentTimestamp">Timestamp actual</param>
        public ConcurrencyException(string mensaje, Guid entityId, string entityType, 
            byte[] expectedTimestamp, byte[] currentTimestamp) : base(mensaje)
        {
            EntityId = entityId;
            EntityType = entityType;
            ExpectedTimestamp = expectedTimestamp;
            CurrentTimestamp = currentTimestamp;
        }

        /// <summary>
        /// Constructor con excepción interna
        /// </summary>
        /// <param name="mensaje">Descripción del conflicto</param>
        /// <param name="innerException">Excepción que causó este error</param>
        public ConcurrencyException(string mensaje, Exception innerException) : base(mensaje, innerException)
        {
        }

        /// <summary>
        /// Crea mensaje detallado para mostrar al usuario
        /// </summary>
        /// <returns>Mensaje amigable para el usuario</returns>
        public string CreateUserFriendlyMessage()
        {
            return $"El {EntityType ?? "registro"} ha sido modificado por otro usuario. " +
                   "Por favor, recargue los datos y vuelva a intentar la operación.";
        }

        /// <summary>
        /// Convierte timestamp a string para logging
        /// </summary>
        /// <param name="timestamp">Timestamp a convertir</param>
        /// <returns>String representación del timestamp</returns>
        public static string TimestampToString(byte[] timestamp)
        {
            return timestamp != null ? Convert.ToBase64String(timestamp) : "null";
        }
    }
}