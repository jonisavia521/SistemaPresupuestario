using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaPresupuestario.BLL.DTOs
{
    /// <summary>
    /// Data Transfer Object for Usuario entity (create/update operations)
    /// </summary>
    public class UsuarioEditDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres")]
        public string NombreUsuario { get; set; }

        [StringLength(255, ErrorMessage = "El email no puede exceder los 255 caracteres")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string Email { get; set; }

        /// <summary>
        /// Password (only for create or when changing password)
        /// </summary>
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        public string Clave { get; set; }

        /// <summary>
        /// Password confirmation
        /// </summary>
        [Compare("Clave", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarClave { get; set; }

        public bool Activo { get; set; }

        public bool DebeRenovarClave { get; set; }

        /// <summary>
        /// Whether to change password (for edit operations)
        /// </summary>
        public bool CambiarClave { get; set; }

        /// <summary>
        /// IDs of families to assign to this user
        /// </summary>
        public List<Guid> FamiliasAsignadasIds { get; set; }

        /// <summary>
        /// IDs of patents to assign directly to this user
        /// </summary>
        public List<Guid> PatentesAsignadasIds { get; set; }

        /// <summary>
        /// Row version for concurrency control
        /// </summary>
        public byte[] RowVersion { get; set; }

        /// <summary>
        /// Indicates if this is a new user (for validation)
        /// </summary>
        public bool IsNewUser => Id == Guid.Empty;

        public UsuarioEditDto()
        {
            Id = Guid.Empty;
            Activo = true;
            DebeRenovarClave = false;
            CambiarClave = false;
            FamiliasAsignadasIds = new List<Guid>();
            PatentesAsignadasIds = new List<Guid>();
        }

        /// <summary>
        /// Validates that user has at least one permission assigned
        /// </summary>
        public bool TienePermisos()
        {
            return (FamiliasAsignadasIds?.Count > 0) || (PatentesAsignadasIds?.Count > 0);
        }

        /// <summary>
        /// Validates that password is provided for new users
        /// </summary>
        public bool ValidatePassword()
        {
            if (IsNewUser)
            {
                return !string.IsNullOrWhiteSpace(Clave);
            }

            if (CambiarClave)
            {
                return !string.IsNullOrWhiteSpace(Clave);
            }

            return true;
        }
    }
}