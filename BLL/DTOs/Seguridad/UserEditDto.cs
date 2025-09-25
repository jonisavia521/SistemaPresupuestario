using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Seguridad
{
    /// <summary>
    /// DTO para edición de usuarios con validaciones completas
    /// </summary>
    public class UserEditDto
    {
        /// <summary>
        /// Identificador único del usuario (Guid.Empty para nuevo usuario)
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre completo del usuario
        /// </summary>
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(1000, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 1000 caracteres")]
        public string Nombre { get; set; }

        /// <summary>
        /// Nombre de usuario para login
        /// </summary>
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 20 caracteres")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "El nombre de usuario solo puede contener letras, números y guiones bajos")]
        public string Usuario { get; set; }

        /// <summary>
        /// Contraseña del usuario (solo para creación/cambio)
        /// </summary>
        [StringLength(50, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 50 caracteres")]
        public string Clave { get; set; }

        /// <summary>
        /// Confirmación de contraseña
        /// </summary>
        [Compare("Clave", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarClave { get; set; }

        /// <summary>
        /// Lista de IDs de familias asignadas al usuario
        /// </summary>
        public List<Guid> FamiliasAsignadas { get; set; } = new List<Guid>();

        /// <summary>
        /// Lista de IDs de patentes asignadas al usuario
        /// </summary>
        public List<Guid> PatentesAsignadas { get; set; } = new List<Guid>();

        /// <summary>
        /// Versión para control de concurrencia
        /// </summary>
        public string VersionConcurrencia { get; set; }

        /// <summary>
        /// Indica si el usuario está activo
        /// </summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Indica si es una operación de cambio de contraseña
        /// </summary>
        public bool CambiarClave { get; set; } = false;
    }
}