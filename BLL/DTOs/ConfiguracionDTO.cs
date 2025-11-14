using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// DTO para transferencia de datos de Configuración entre UI y BLL
    /// </summary>
    public class ConfiguracionDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "La razón social es obligatoria")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "La razón social debe tener entre 3 y 200 caracteres")]
        public string RazonSocial { get; set; }

        [Required(ErrorMessage = "El CUIT es obligatorio")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "El CUIT debe tener 11 dígitos")]
        public string CUIT { get; set; }

        [Required(ErrorMessage = "El tipo de IVA es obligatorio")]
        [StringLength(50, ErrorMessage = "El tipo de IVA no puede exceder los 50 caracteres")]
        public string TipoIva { get; set; }

        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres")]
        public string Direccion { get; set; }

        [StringLength(100, ErrorMessage = "La localidad no puede exceder los 100 caracteres")]
        public string Localidad { get; set; }

        public Guid? IdProvincia { get; set; }

        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres")]
        public string Email { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El idioma es obligatorio")]
        public string Idioma { get; set; }

        public DateTime FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
