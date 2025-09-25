using System;
using System.Collections.Generic;

namespace BLL.Exceptions
{
    /// <summary>
    /// Excepción lanzada cuando se detecta una violación de integridad
    /// en la estructura de permisos (por ejemplo, ciclos en familias)
    /// </summary>
    public class PermissionIntegrityException : Exception
    {
        /// <summary>
        /// Tipo de violación de integridad
        /// </summary>
        public PermissionIntegrityViolationType ViolationType { get; }

        /// <summary>
        /// IDs de entidades involucradas en la violación
        /// </summary>
        public List<Guid> InvolvedEntities { get; }

        /// <summary>
        /// Ruta del ciclo (si aplica)
        /// </summary>
        public List<string> CyclePath { get; }

        /// <summary>
        /// Constructor básico
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="violationType">Tipo de violación</param>
        public PermissionIntegrityException(string message, PermissionIntegrityViolationType violationType) 
            : base(message)
        {
            ViolationType = violationType;
            InvolvedEntities = new List<Guid>();
            CyclePath = new List<string>();
        }

        /// <summary>
        /// Constructor con entidades involucradas
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="violationType">Tipo de violación</param>
        /// <param name="involvedEntities">Entidades involucradas</param>
        public PermissionIntegrityException(string message, PermissionIntegrityViolationType violationType, 
            List<Guid> involvedEntities) : base(message)
        {
            ViolationType = violationType;
            InvolvedEntities = involvedEntities ?? new List<Guid>();
            CyclePath = new List<string>();
        }

        /// <summary>
        /// Constructor completo para ciclos
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="violationType">Tipo de violación</param>
        /// <param name="involvedEntities">Entidades involucradas</param>
        /// <param name="cyclePath">Ruta del ciclo</param>
        public PermissionIntegrityException(string message, PermissionIntegrityViolationType violationType,
            List<Guid> involvedEntities, List<string> cyclePath) : base(message)
        {
            ViolationType = violationType;
            InvolvedEntities = involvedEntities ?? new List<Guid>();
            CyclePath = cyclePath ?? new List<string>();
        }

        /// <summary>
        /// Constructor con inner exception
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="violationType">Tipo de violación</param>
        /// <param name="innerException">Excepción interna</param>
        public PermissionIntegrityException(string message, PermissionIntegrityViolationType violationType, 
            Exception innerException) : base(message, innerException)
        {
            ViolationType = violationType;
            InvolvedEntities = new List<Guid>();
            CyclePath = new List<string>();
        }
    }

    /// <summary>
    /// Tipos de violaciones de integridad de permisos
    /// </summary>
    public enum PermissionIntegrityViolationType
    {
        /// <summary>
        /// Se detectó un ciclo en la jerarquía de familias
        /// </summary>
        CyclicReference,

        /// <summary>
        /// Usuario quedaría sin permisos efectivos
        /// </summary>
        NoEffectivePermissions,

        /// <summary>
        /// Referencia a una entidad inexistente
        /// </summary>
        OrphanReference,

        /// <summary>
        /// Estructura de permisos inválida
        /// </summary>
        InvalidStructure
    }
}