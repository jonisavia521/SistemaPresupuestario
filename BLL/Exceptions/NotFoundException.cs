using System;

namespace BLL.Exceptions
{
    /// <summary>
    /// Excepción lanzada cuando no se encuentra una entidad solicitada
    /// DECISIÓN: Separar entidades no encontradas de otros errores para manejo específico en UI
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Tipo de entidad que no se encontró
        /// </summary>
        public string EntityType { get; }

        /// <summary>
        /// Identificador de la entidad que no se encontró
        /// </summary>
        public object EntityId { get; }

        /// <summary>
        /// Constructor básico con mensaje
        /// </summary>
        /// <param name="mensaje">Descripción de lo que no se encontró</param>
        public NotFoundException(string mensaje) : base(mensaje)
        {
        }

        /// <summary>
        /// Constructor con tipo de entidad
        /// </summary>
        /// <param name="entityType">Tipo de entidad (ej: "Usuario", "Familia")</param>
        /// <param name="entityId">ID de la entidad</param>
        public NotFoundException(string entityType, object entityId) 
            : base($"{entityType} con ID '{entityId}' no fue encontrado")
        {
            EntityType = entityType;
            EntityId = entityId;
        }

        /// <summary>
        /// Constructor con mensaje personalizado y contexto
        /// </summary>
        /// <param name="mensaje">Mensaje personalizado</param>
        /// <param name="entityType">Tipo de entidad</param>
        /// <param name="entityId">ID de la entidad</param>
        public NotFoundException(string mensaje, string entityType, object entityId) : base(mensaje)
        {
            EntityType = entityType;
            EntityId = entityId;
        }

        /// <summary>
        /// Constructor con excepción interna
        /// </summary>
        /// <param name="mensaje">Descripción del error</param>
        /// <param name="innerException">Excepción que causó este error</param>
        public NotFoundException(string mensaje, Exception innerException) : base(mensaje, innerException)
        {
        }

        /// <summary>
        /// Crea una instancia para Usuario no encontrado
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <returns>Nueva instancia de NotFoundException</returns>
        public static NotFoundException ForUsuario(Guid usuarioId)
        {
            return new NotFoundException("Usuario", usuarioId);
        }

        /// <summary>
        /// Crea una instancia para Usuario no encontrado por nombre de usuario
        /// </summary>
        /// <param name="nombreUsuario">Nombre de usuario</param>
        /// <returns>Nueva instancia de NotFoundException</returns>
        public static NotFoundException ForUsuarioByName(string nombreUsuario)
        {
            return new NotFoundException($"Usuario con nombre '{nombreUsuario}' no fue encontrado", "Usuario", nombreUsuario);
        }

        /// <summary>
        /// Crea una instancia para Familia no encontrada
        /// </summary>
        /// <param name="familiaId">ID de la familia</param>
        /// <returns>Nueva instancia de NotFoundException</returns>
        public static NotFoundException ForFamilia(Guid familiaId)
        {
            return new NotFoundException("Familia", familiaId);
        }

        /// <summary>
        /// Crea una instancia para Patente no encontrada
        /// </summary>
        /// <param name="patenteId">ID de la patente</param>
        /// <returns>Nueva instancia de NotFoundException</returns>
        public static NotFoundException ForPatente(Guid patenteId)
        {
            return new NotFoundException("Patente", patenteId);
        }
    }
}