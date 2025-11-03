using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// DTO para transferencia de datos de Presupuesto entre capas
    /// Contiene validaciones de entrada con DataAnnotations
    /// </summary>
    public class PresupuestoDTO
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El número de presupuesto es obligatorio")]
        [MaxLength(50, ErrorMessage = "El número no puede exceder los 50 caracteres")]
        public string Numero { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un cliente")]
        public Guid IdCliente { get; set; }

        [Required(ErrorMessage = "La fecha de emisión es obligatoria")]
        public DateTime FechaEmision { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [Range(0, 4, ErrorMessage = "Estado inválido")]
        public int Estado { get; set; }

        public DateTime? FechaVencimiento { get; set; }

        public Guid? IdPresupuestoPadre { get; set; }

        public Guid? IdVendedor { get; set; }

        // Propiedades navegacionales (para visualización)
        public string ClienteRazonSocial { get; set; }
        public string VendedorNombre { get; set; }
        public string EstadoDescripcion { get; set; }

        // Detalles
        public List<PresupuestoDetalleDTO> Detalles { get; set; }

        // Totales calculados
        public decimal Subtotal { get; set; }
        public decimal TotalIva { get; set; }
        public decimal Total { get; set; }

        public PresupuestoDTO()
        {
            Detalles = new List<PresupuestoDetalleDTO>();
        }
    }

    /// <summary>
    /// DTO para transferencia de datos de Detalle de Presupuesto
    /// </summary>
    public class PresupuestoDetalleDTO
    {
        public Guid Id { get; set; }

        public string Numero { get; set; }

        public Guid? IdPresupuesto { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un producto")]
        public Guid? IdProducto { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(0.01, 999999, ErrorMessage = "La cantidad debe estar entre 0.01 y 999,999")]
        public decimal Cantidad { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0, 999999999, ErrorMessage = "El precio debe estar entre 0 y 999,999,999")]
        public decimal Precio { get; set; }

        [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100")]
        public decimal Descuento { get; set; }

        public int Renglon { get; set; }

        [Range(0, 100, ErrorMessage = "El porcentaje de IVA debe estar entre 0 y 100")]
        public decimal PorcentajeIVA { get; set; }

        // Propiedades navegacionales (para visualización)
        public string Codigo { get; set; }
        public string Descripcion { get; set; }

        // Campos calculados
        public decimal Subtotal => Cantidad * Precio;
        public decimal DescuentoMonto => Subtotal * (Descuento / 100);
        public decimal Total => Subtotal - DescuentoMonto;
        public decimal Iva => Total * (PorcentajeIVA / 100);
        public decimal TotalConIva => Total + Iva;
    }
}
