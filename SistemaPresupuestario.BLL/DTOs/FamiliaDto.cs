using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaPresupuestario.BLL.DTOs
{
    /// <summary>
    /// Data Transfer Object for Familia entity
    /// </summary>
    public class FamiliaDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Parent family ID (null for root families)
        /// </summary>
        public Guid? FamiliaPadreId { get; set; }

        /// <summary>
        /// Parent family name (for display)
        /// </summary>
        public string FamiliaPadreNombre { get; set; }

        /// <summary>
        /// Level in hierarchy (0 = root)
        /// </summary>
        public int Nivel { get; set; }

        /// <summary>
        /// Full path from root (for display)
        /// </summary>
        public string RutaCompleta { get; set; }

        /// <summary>
        /// Whether this family has child families
        /// </summary>
        public bool TieneHijos { get; set; }

        /// <summary>
        /// Child families
        /// </summary>
        public List<FamiliaDto> FamiliasHijas { get; set; }

        /// <summary>
        /// Patents directly assigned to this family
        /// </summary>
        public List<PatenteDto> Patentes { get; set; }

        /// <summary>
        /// Count of direct patents
        /// </summary>
        public int PatentesDirectasCount { get; set; }

        /// <summary>
        /// Count of effective patents (including inherited)
        /// </summary>
        public int PatentesEfectivasCount { get; set; }

        /// <summary>
        /// Count of users assigned to this family
        /// </summary>
        public int UsuariosCount { get; set; }

        /// <summary>
        /// Row version for concurrency control
        /// </summary>
        public byte[] RowVersion { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public FamiliaDto()
        {
            FamiliasHijas = new List<FamiliaDto>();
            Patentes = new List<PatenteDto>();
            Nivel = 0;
            TieneHijos = false;
        }

        /// <summary>
        /// Gets display name with hierarchy indication
        /// </summary>
        public string NombreConJerarquia
        {
            get
            {
                var indent = new string('-', Nivel * 2);
                return $"{indent} {Nombre}";
            }
        }

        /// <summary>
        /// Gets full display text with counts
        /// </summary>
        public string NombreCompleto
        {
            get
            {
                return $"{Nombre} ({PatentesDirectasCount} patentes directas, {PatentesEfectivasCount} efectivas, {UsuariosCount} usuarios)";
            }
        }
    }
}