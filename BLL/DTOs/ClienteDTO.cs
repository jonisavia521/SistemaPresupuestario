using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// DTO para transferencia de datos de Cliente entre UI y BLL
    /// Contiene DataAnnotations para validaciones de entrada superficiales
    /// </summary>
    public class ClienteDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El código de cliente es obligatorio")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "El código debe tener entre 3 y 20 caracteres")]
        public string CodigoCliente { get; set; }

        [Required(ErrorMessage = "La razón social es obligatoria")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "La razón social debe tener entre 3 y 200 caracteres")]
        public string RazonSocial { get; set; }

        [Required(ErrorMessage = "El tipo de documento es obligatorio")]
        public string TipoDocumento { get; set; }

        [Required(ErrorMessage = "El número de documento es obligatorio")]
        public string NumeroDocumento { get; set; }

        // IdVendedor ahora es Guid? - FK a Vendedor
        public Guid? IdVendedor { get; set; }

        // Propiedad auxiliar para mostrar el código del vendedor en la UI
        public string CodigoVendedor { get; set; }
        
        // Propiedad auxiliar para mostrar el nombre del vendedor
        public string NombreVendedor { get; set; }

        // IdProvincia - FK a Provincia
        public Guid? IdProvincia { get; set; }

        // Propiedad auxiliar para mostrar el nombre de la provincia
        public string NombreProvincia { get; set; }

        [Required(ErrorMessage = "El tipo de IVA es obligatorio")]
        public string TipoIva { get; set; }

        [Required(ErrorMessage = "La condición de pago es obligatoria")]
        [RegularExpression(@"^\d{2}$", ErrorMessage = "La condición de pago debe tener 2 dígitos")]
        public string CondicionPago { get; set; }

        // NUEVO: Alícuota ARBA
        [Required(ErrorMessage = "La alícuota ARBA es obligatoria")]
        [Range(0, 100, ErrorMessage = "La alícuota ARBA debe estar entre 0 y 100")]
        public decimal AlicuotaArba { get; set; }

        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres")]
        public string Email { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        public string Telefono { get; set; }

        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres")]
        public string Direccion { get; set; }

        [StringLength(100, ErrorMessage = "La localidad no puede exceder los 100 caracteres")]
        public string Localidad { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaAlta { get; set; }

        public DateTime? FechaModificacion { get; set; }

        // Propiedades calculadas para mostrar en la UI
        public string DocumentoCompleto => $"{TipoDocumento}: {NumeroDocumento}";

        public string EstadoTexto => Activo ? "Activo" : "Inactivo";
    }
}
