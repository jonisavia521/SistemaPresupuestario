using System;

namespace BLL.Exceptions
{
    /// <summary>
    /// Excepción lanzada cuando se detecta un conflicto de concurrencia
    /// (rowversion no coincide)
    /// </summary>
    public class ConcurrencyException : Exception
    {
        /// <summary>
        /// ID de la entidad que causó el conflicto
        /// </summary>
        public Guid EntityId { get; }

        /// <summary>
        /// Tipo de entidad
        /// </summary>
        public string EntityType { get; }

        /// <summary>
        /// Versión esperada
        /// </summary>
        public string ExpectedVersion { get; }

        /// <summary>
        /// Versión actual en la base de datos
        /// </summary>
        public string CurrentVersion { get; }

        /// <summary>
        /// Constructor básico
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        public ConcurrencyException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor con detalles de entidad
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="entityId">ID de la entidad</param>
        /// <param name="entityType">Tipo de entidad</param>
        public ConcurrencyException(string message, Guid entityId, string entityType) : base(message)
        {
            EntityId = entityId;
            EntityType = entityType;
        }

        /// <summary>
        /// Constructor completo
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="entityId">ID de la entidad</param>
        /// <param name="entityType">Tipo de entidad</param>
        /// <param name="expectedVersion">Versión esperada</param>
        /// <param name="currentVersion">Versión actual</param>
        public ConcurrencyException(string message, Guid entityId, string entityType, 
            string expectedVersion, string currentVersion) : base(message)
        {
            EntityId = entityId;
            EntityType = entityType;
            ExpectedVersion = expectedVersion;
            CurrentVersion = currentVersion;
        }

        /// <summary>
        /// Constructor con inner exception
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="innerException">Excepción interna</param>
        public ConcurrencyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}