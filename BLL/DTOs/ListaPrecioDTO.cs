using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// DTO para transferencia de datos de Lista de Precios entre UI y BLL
    /// </summary>
    public class ListaPrecioDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El código es obligatorio")]
        [RegularExpression(@"^\d{2}$", ErrorMessage = "El código debe tener exactamente 2 dígitos")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Nombre { get; set; }

        public bool Activo { get; set; }

        public bool IncluyeIva { get; set; }

        public DateTime FechaAlta { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public List<ListaPrecioDetalleDTO> Detalles { get; set; }

        public string EstadoTexto => Activo ? "Activo" : "Inactivo";

        public ListaPrecioDTO()
        {
            Activo = true;
            IncluyeIva = false;
            FechaAlta = DateTime.Now;
            Detalles = new List<ListaPrecioDetalleDTO>();
        }
    }

    /// <summary>
    /// DTO para transferencia de datos de Detalle de Lista de Precios
    /// </summary>
    public class ListaPrecioDetalleDTO
    {
        public Guid Id { get; set; }

        public Guid IdListaPrecio { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un producto")]
        public Guid IdProducto { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0, 999999999, ErrorMessage = "El precio debe estar entre 0 y 999,999,999")]
        public decimal Precio { get; set; }

        // Propiedades navegacionales (para visualización)
        public string Codigo { get; set; }

        public string Descripcion { get; set; }

        public ListaPrecioDetalleDTO()
        {
            Precio = 0M;
        }
    }
}
