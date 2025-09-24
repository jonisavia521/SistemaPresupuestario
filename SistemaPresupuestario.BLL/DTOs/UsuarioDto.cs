using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaPresupuestario.BLL.DTOs
{
    /// <summary>
    /// Data Transfer Object for Usuario entity (read operations)
    /// </summary>
    public class UsuarioDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreUsuario { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        public bool Activo { get; set; }

        public bool DebeRenovarClave { get; set; }

        public DateTime? UltimoLogin { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        /// <summary>
        /// Families assigned to this user
        /// </summary>
        public List<FamiliaDto> Familias { get; set; }

        /// <summary>
        /// Patents directly assigned to this user
        /// </summary>
        public List<PatenteDto> Patentes { get; set; }

        /// <summary>
        /// Effective permissions summary
        /// </summary>
        public PermisoEfectivoDto PermisosEfectivos { get; set; }

        /// <summary>
        /// Row version for concurrency control
        /// </summary>
        public byte[] RowVersion { get; set; }

        public UsuarioDto()
        {
            Familias = new List<FamiliaDto>();
            Patentes = new List<PatenteDto>();
        }
    }
}