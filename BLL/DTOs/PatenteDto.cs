using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// DTO para representar una Patente (permiso individual) en el sistema
    /// </summary>
    public class PatenteDto
    {
        public Guid IdPatente { get; set; }

        [Required(ErrorMessage = "El nombre de la patente es obligatorio")]
        [StringLength(1000, ErrorMessage = "El nombre no puede exceder 1000 caracteres")]
        public string Nombre { get; set; }

        [StringLength(1000, ErrorMessage = "La vista no puede exceder 1000 caracteres")]
        public string Vista { get; set; }

        /// <summary>
        /// Control de concurrencia
        /// </summary>
        public byte[] Timestamp { get; set; }
    }
}