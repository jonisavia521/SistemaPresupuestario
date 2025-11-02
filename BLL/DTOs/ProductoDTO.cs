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

        // Propiedad calculada para mostrar en la UI
        public string EstadoTexto => Inhabilitado ? "Inactivo" : "Activo";
    }
}
