using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaPresupuestario.DomainModel.Seguridad.Base
{
    /// <summary>
    /// Base class for all domain entities providing common properties like Id and concurrency control
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary>
        /// Primary key identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Row version for optimistic concurrency control
        /// </summary>
        [Timestamp]
        public byte[] RowVersion { get; set; }

        /// <summary>
        /// Creation timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Last modification timestamp
        /// </summary>
        public DateTime? ModifiedAt { get; set; }

        protected EntityBase()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }
    }
}