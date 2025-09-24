using System;

namespace SistemaPresupuestario.BLL.Exceptions
{
    /// <summary>
    /// Exception thrown when a concurrency conflict occurs during data modification
    /// </summary>
    public class ConcurrencyException : Exception
    {
        /// <summary>
        /// The entity ID that caused the concurrency conflict
        /// </summary>
        public object EntityId { get; }

        /// <summary>
        /// The entity type that caused the conflict
        /// </summary>
        public Type EntityType { get; }

        /// <summary>
        /// The expected row version
        /// </summary>
        public byte[] ExpectedRowVersion { get; }

        /// <summary>
        /// The actual row version found in database
        /// </summary>
        public byte[] ActualRowVersion { get; }

        public ConcurrencyException() : base("A concurrency conflict occurred. The record has been modified by another user.")
        {
        }

        public ConcurrencyException(string message) : base(message)
        {
        }

        public ConcurrencyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ConcurrencyException(object entityId, Type entityType) 
            : base($"A concurrency conflict occurred while updating {entityType?.Name ?? "entity"} with ID {entityId}. The record has been modified by another user.")
        {
            EntityId = entityId;
            EntityType = entityType;
        }

        public ConcurrencyException(object entityId, Type entityType, byte[] expectedRowVersion, byte[] actualRowVersion)
            : base($"A concurrency conflict occurred while updating {entityType?.Name ?? "entity"} with ID {entityId}. The record has been modified by another user since it was last loaded.")
        {
            EntityId = entityId;
            EntityType = entityType;
            ExpectedRowVersion = expectedRowVersion;
            ActualRowVersion = actualRowVersion;
        }

        /// <summary>
        /// Gets a user-friendly message explaining the concurrency conflict
        /// </summary>
        public string GetUserFriendlyMessage()
        {
            if (EntityType != null)
            {
                return $"El registro de {GetEntityDisplayName(EntityType)} ha sido modificado por otro usuario. " +
                       "Por favor, recargue los datos y vuelva a intentar la operación.";
            }

            return "El registro ha sido modificado por otro usuario. " +
                   "Por favor, recargue los datos y vuelva a intentar la operación.";
        }

        /// <summary>
        /// Gets a display name for the entity type
        /// </summary>
        private string GetEntityDisplayName(Type entityType)
        {
            return entityType.Name switch
            {
                "Usuario" => "usuario",
                "Familia" => "familia",
                "Patente" => "patente",
                _ => "registro"
            };
        }

        /// <summary>
        /// Creates a concurrency exception for a specific entity
        /// </summary>
        public static ConcurrencyException ForEntity<T>(object entityId)
        {
            return new ConcurrencyException(entityId, typeof(T));
        }

        /// <summary>
        /// Creates a concurrency exception with row version information
        /// </summary>
        public static ConcurrencyException ForEntity<T>(object entityId, byte[] expectedVersion, byte[] actualVersion)
        {
            return new ConcurrencyException(entityId, typeof(T), expectedVersion, actualVersion);
        }
    }
}