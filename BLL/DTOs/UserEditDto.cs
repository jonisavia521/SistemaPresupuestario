using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// DTO para edición de usuarios con información completa
    /// Incluye asignaciones de familias y patentes
    /// </summary>
    public class UserEditDto
    {
        /// <summary>
        /// Identificador único del usuario (Guid.Empty para nuevos usuarios)
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre de usuario para login (único en el sistema)
        /// </summary>
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "El usuario debe tener entre 3 y 20 caracteres")]
        public string Usuario { get; set; }

        /// <summary>
        /// Nombre completo del usuario
        /// </summary>
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(1000, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 1000 caracteres")]
        public string Nombre { get; set; }

        /// <summary>
        /// Nueva clave (opcional en edición, obligatoria en alta)
        /// Solo se completa cuando se desea cambiar la clave
        /// </summary>
        [StringLength(100, ErrorMessage = "La clave no puede exceder 100 caracteres")]
        public string NuevaClaveOpcional { get; set; }

        /// <summary>
        /// Confirmación de la nueva clave
        /// </summary>
        public string ConfirmarClave { get; set; }

        /// <summary>
        /// IDs de las familias asignadas al usuario
        /// </summary>
        public List<Guid> FamiliasAsignadasIds { get; set; }

        /// <summary>
        /// IDs de las patentes asignadas directamente al usuario
        /// </summary>
        public List<Guid> PatentesAsignadasIds { get; set; }

        /// <summary>
        /// Timestamp para control de concurrencia (codificado en Base64)
        /// </summary>
        public string TimestampBase64 { get; set; }

        /// <summary>
        /// Indica si se debe forzar cambio de contraseña en próximo login
        /// </summary>
        public bool ForzarCambioClave { get; set; }

        /// <summary>
        /// Estado del usuario (opcional)
        /// </summary>
        public bool? Estado { get; set; }

        public UserEditDto()
        {
            FamiliasAsignadasIds = new List<Guid>();
            PatentesAsignadasIds = new List<Guid>();
        }

        /// <summary>
        /// Valida que las claves coincidan cuando se proporciona una nueva clave
        /// </summary>
        /// <returns>True si la validación es exitosa</returns>
        public bool ValidarClaves()
        {
            if (string.IsNullOrEmpty(NuevaClaveOpcional))
                return true; // No hay nueva clave, válido para edición

            return NuevaClaveOpcional == ConfirmarClave;
        }
    }
}