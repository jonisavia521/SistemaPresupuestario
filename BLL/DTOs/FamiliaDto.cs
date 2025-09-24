using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// DTO para representar una Familia de permisos en el sistema
    /// </summary>
    public class FamiliaDto
    {
        public Guid IdFamilia { get; set; }

        [Required(ErrorMessage = "El nombre de la familia es obligatorio")]
        [StringLength(1000, ErrorMessage = "El nombre no puede exceder 1000 caracteres")]
        public string Nombre { get; set; }

        /// <summary>
        /// Lista de IDs de familias hijas (para jerarquía)
        /// </summary>
        public List<Guid> FamiliasHijas { get; set; } = new List<Guid>();

        /// <summary>
        /// Lista de IDs de patentes asignadas directamente
        /// </summary>
        public List<Guid> PatentesAsignadas { get; set; } = new List<Guid>();

        /// <summary>
        /// Control de concurrencia
        /// </summary>
        public byte[] Timestamp { get; set; }
    }
}