using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// DTO específico para edición de usuarios con asignaciones de permisos
    /// </summary>
    public class UserEditDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(1000, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 1000 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "El usuario debe tener entre 3 y 20 caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "El usuario solo puede contener letras, números y guiones bajos")]
        public string Usuario { get; set; }

        /// <summary>
        /// Clave en texto plano (solo para creación/actualización)
        /// </summary>
        [StringLength(50, MinimumLength = 6, ErrorMessage = "La clave debe tener entre 6 y 50 caracteres")]
        public string Clave { get; set; }

        /// <summary>
        /// Confirmación de clave
        /// </summary>
        [Compare("Clave", ErrorMessage = "Las claves no coinciden")]
        public string ConfirmarClave { get; set; }

        /// <summary>
        /// Indica si se debe cambiar la clave (para edición)
        /// </summary>
        public bool CambiarClave { get; set; }

        /// <summary>
        /// IDs de familias asignadas al usuario
        /// </summary>
        public List<Guid> FamiliasAsignadas { get; set; } = new List<Guid>();

        /// <summary>
        /// IDs de patentes asignadas directamente al usuario
        /// </summary>
        public List<Guid> PatentesAsignadas { get; set; } = new List<Guid>();

        /// <summary>
        /// Control de concurrencia
        /// </summary>
        public byte[] Timestamp { get; set; }

        /// <summary>
        /// Indica si es un nuevo usuario
        /// </summary>
        public bool EsNuevo => Id == Guid.Empty;
    }
}