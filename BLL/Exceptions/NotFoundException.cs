using System;

namespace BLL.Exceptions
{
    /// <summary>
    /// Excepción lanzada cuando no se encuentra una entidad solicitada
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// ID de la entidad no encontrada
        /// </summary>
        public Guid EntityId { get; }

        /// <summary>
        /// Tipo de entidad
        /// </summary>
        public string EntityType { get; }

        /// <summary>
        /// Constructor básico
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        public NotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor con detalles de entidad
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="entityId">ID de la entidad</param>
        /// <param name="entityType">Tipo de entidad</param>
        public NotFoundException(string message, Guid entityId, string entityType) : base(message)
        {
            EntityId = entityId;
            EntityType = entityType;
        }

        /// <summary>
        /// Constructor con inner exception
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="innerException">Excepción interna</param>
        public NotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Constructor completo con inner exception
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="entityId">ID de la entidad</param>
        /// <param name="entityType">Tipo de entidad</param>
        /// <param name="innerException">Excepción interna</param>
        public NotFoundException(string message, Guid entityId, string entityType, Exception innerException) 
            : base(message, innerException)
        {
            EntityId = entityId;
            EntityType = entityType;
        }
    }
}