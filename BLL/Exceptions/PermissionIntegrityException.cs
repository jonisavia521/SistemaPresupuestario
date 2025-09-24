using System;
using System.Collections.Generic;

namespace BLL.Exceptions
{
    /// <summary>
    /// Excepción lanzada cuando se violan reglas de integridad de permisos
    /// DECISIÓN: Validar integridad de permisos a nivel de negocio para evitar estados inconsistentes
    /// </summary>
    public class PermissionIntegrityException : Exception
    {
        /// <summary>
        /// Tipo de violación de integridad
        /// </summary>
        public enum ViolationType
        {
            /// <summary>
            /// Usuario no tiene permisos efectivos
            /// </summary>
            NoEffectivePermissions,
            
            /// <summary>
            /// Se detectó un ciclo en la jerarquía de familias
            /// </summary>
            CircularReference,
            
            /// <summary>
            /// Intento de crear relación padre-hijo inválida
            /// </summary>
            InvalidHierarchy,
            
            /// <summary>
            /// Asignación de permiso duplicado
            /// </summary>
            DuplicatePermission,
            
            /// <summary>
            /// Intento de eliminar permiso requerido
            /// </summary>
            RequiredPermissionDeletion,
            
            /// <summary>
            /// Estructura de permisos inválida
            /// </summary>
            InvalidPermissionStructure
        }

        /// <summary>
        /// Tipo de violación detectada
        /// </summary>
        public ViolationType Violation { get; }

        /// <summary>
        /// IDs de entidades involucradas en la violación
        /// </summary>
        public List<Guid> InvolvedEntities { get; }

        /// <summary>
        /// Contexto adicional sobre la violación
        /// </summary>
        public string Context { get; }

        /// <summary>
        /// Constructor básico con mensaje
        /// </summary>
        /// <param name="mensaje">Descripción de la violación</param>
        public PermissionIntegrityException(string mensaje) : base(mensaje)
        {
            InvolvedEntities = new List<Guid>();
        }

        /// <summary>
        /// Constructor con tipo de violación
        /// </summary>
        /// <param name="violation">Tipo de violación</param>
        /// <param name="mensaje">Descripción de la violación</param>
        public PermissionIntegrityException(ViolationType violation, string mensaje) : base(mensaje)
        {
            Violation = violation;
            InvolvedEntities = new List<Guid>();
        }

        /// <summary>
        /// Constructor completo
        /// </summary>
        /// <param name="violation">Tipo de violación</param>
        /// <param name="mensaje">Descripción de la violación</param>
        /// <param name="context">Contexto adicional</param>
        /// <param name="involvedEntities">Entidades involucradas</param>
        public PermissionIntegrityException(ViolationType violation, string mensaje, string context, 
            params Guid[] involvedEntities) : base(mensaje)
        {
            Violation = violation;
            Context = context;
            InvolvedEntities = new List<Guid>(involvedEntities);
        }

        /// <summary>
        /// Constructor con excepción interna
        /// </summary>
        /// <param name="mensaje">Descripción de la violación</param>
        /// <param name="innerException">Excepción que causó este error</param>
        public PermissionIntegrityException(string mensaje, Exception innerException) : base(mensaje, innerException)
        {
            InvolvedEntities = new List<Guid>();
        }

        /// <summary>
        /// Crea excepción para usuario sin permisos efectivos
        /// </summary>
        /// <param name="usuarioId">ID del usuario</param>
        /// <param name="nombreUsuario">Nombre del usuario</param>
        /// <returns>Nueva instancia de PermissionIntegrityException</returns>
        public static PermissionIntegrityException ForNoEffectivePermissions(Guid usuarioId, string nombreUsuario)
        {
            return new PermissionIntegrityException(
                ViolationType.NoEffectivePermissions,
                $"El usuario '{nombreUsuario}' debe tener al menos un permiso efectivo (patente directa o familia asignada)",
                $"Usuario: {nombreUsuario}",
                usuarioId
            );
        }

        /// <summary>
        /// Crea excepción para referencia circular en familias
        /// </summary>
        /// <param name="familiaIds">IDs de familias en el ciclo</param>
        /// <param name="path">Ruta del ciclo detectado</param>
        /// <returns>Nueva instancia de PermissionIntegrityException</returns>
        public static PermissionIntegrityException ForCircularReference(Guid[] familiaIds, string path)
        {
            return new PermissionIntegrityException(
                ViolationType.CircularReference,
                "Se detectó una referencia circular en la jerarquía de familias",
                $"Ruta del ciclo: {path}",
                familiaIds
            );
        }

        /// <summary>
        /// Crea excepción para jerarquía inválida
        /// </summary>
        /// <param name="padreId">ID de la familia padre</param>
        /// <param name="hijoId">ID de la familia hijo</param>
        /// <param name="razon">Razón de la invalidez</param>
        /// <returns>Nueva instancia de PermissionIntegrityException</returns>
        public static PermissionIntegrityException ForInvalidHierarchy(Guid padreId, Guid hijoId, string razon)
        {
            return new PermissionIntegrityException(
                ViolationType.InvalidHierarchy,
                $"Jerarquía inválida entre familias: {razon}",
                razon,
                padreId, hijoId
            );
        }

        /// <summary>
        /// Crea mensaje amigable para el usuario
        /// </summary>
        /// <returns>Mensaje comprensible para mostrar en UI</returns>
        public string CreateUserFriendlyMessage()
        {
            return Violation switch
            {
                ViolationType.NoEffectivePermissions => 
                    "El usuario debe tener al menos un permiso asignado (patente directa o familia).",
                
                ViolationType.CircularReference => 
                    "No es posible crear esta relación porque generaría una referencia circular en la jerarquía de familias.",
                
                ViolationType.InvalidHierarchy => 
                    "La relación padre-hijo especificada no es válida en la jerarquía de familias.",
                
                ViolationType.DuplicatePermission => 
                    "El permiso ya está asignado al usuario.",
                
                ViolationType.RequiredPermissionDeletion => 
                    "No es posible eliminar este permiso porque es requerido por el sistema.",
                
                ViolationType.InvalidPermissionStructure => 
                    "La estructura de permisos no es válida.",
                
                _ => "Se detectó una violación de integridad en los permisos."
            };
        }
    }
}