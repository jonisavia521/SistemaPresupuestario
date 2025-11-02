using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// DTO para transferencia de datos de Vendedor entre UI y BLL
    /// Contiene DataAnnotations para validaciones de entrada superficiales
    /// </summary>
    public class VendedorDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El código de vendedor es obligatorio")]
        [RegularExpression(@"^\d{2}$", ErrorMessage = "El código debe tener exactamente 2 dígitos")]
        public string CodigoVendedor { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 200 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El CUIT es obligatorio")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "El CUIT debe tener 11 dígitos")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "El CUIT solo puede contener dígitos")]
        public string CUIT { get; set; }

        [Range(0, 100, ErrorMessage = "La comisión debe estar entre 0 y 100")]
        public decimal PorcentajeComision { get; set; }

        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres")]
        public string Email { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        public string Telefono { get; set; }

        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres")]
        public string Direccion { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaAlta { get; set; }

        public DateTime? FechaModificacion { get; set; }

        // Propiedades calculadas para mostrar en la UI
        public string CUITFormateado
        {
            get
            {
                if (string.IsNullOrEmpty(CUIT) || CUIT.Length != 11)
                    return CUIT;
                return $"{CUIT.Substring(0, 2)}-{CUIT.Substring(2, 8)}-{CUIT.Substring(10, 1)}";
            }
        }

        public string EstadoTexto => Activo ? "Activo" : "Inactivo";
    }
}
