using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// DTO para transferencia de datos de Provincia entre UI y BLL
    /// </summary>
    public class ProvinciaDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El código AFIP es obligatorio")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "El código AFIP debe tener exactamente 2 caracteres")]
        public string CodigoAFIP { get; set; }

        [Required(ErrorMessage = "El nombre de la provincia es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres")]
        public string Nombre { get; set; }

        // Propiedad calculada para mostrar en ComboBox
        public string NombreCompleto => $"{CodigoAFIP} - {Nombre}";
    }
}
