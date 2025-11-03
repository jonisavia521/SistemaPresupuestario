using System;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// Data Transfer Object para Producto
    /// Modelo de vista con validaciones de entrada (DataAnnotations)
    /// </summary>
    public class ProductoDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(50, ErrorMessage = "El código no puede exceder los 50 caracteres")]
        public string Codigo { get; set; }

        [StringLength(50, ErrorMessage = "La descripción no puede exceder los 50 caracteres")]
        public string Descripcion { get; set; }

        public bool Inhabilitado { get; set; }

        public DateTime FechaAlta { get; set; }

        public int UsuarioAlta { get; set; }

        [Required(ErrorMessage = "El porcentaje de IVA es obligatorio")]
        [Range(0, 21, ErrorMessage = "El porcentaje de IVA debe estar entre 0 y 21")]
        public decimal PorcentajeIVA { get; set; }

        // Propiedad calculada para mostrar en la UI
        public string EstadoTexto => Inhabilitado ? "Inactivo" : "Activo";

        // Propiedad calculada para mostrar el IVA formateado
        public string IVATexto
        {
            get
            {
                if (PorcentajeIVA == 0)
                    return "Exento";
                else if (PorcentajeIVA == 10.50m)
                    return "10.5%";
                else if (PorcentajeIVA == 21)
                    return "21%";
                else
                    return $"{PorcentajeIVA}%";
            }
        }
    }
}
