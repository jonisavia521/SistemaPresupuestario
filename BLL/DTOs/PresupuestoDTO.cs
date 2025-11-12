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
        [Range(0, 6, ErrorMessage = "Estado inválido")]
        public int Estado { get; set; }

        public DateTime? FechaVencimiento { get; set; }

        public Guid? IdPresupuestoPadre { get; set; }

        public Guid? IdVendedor { get; set; }

        public Guid? IdListaPrecio { get; set; }

        // Propiedades navegacionales (para visualización)
        public string ClienteRazonSocial { get; set; }
        public string VendedorNombre { get; set; }
        public string EstadoDescripcion { get; set; }
        public string ListaPrecioCodigo { get; set; }
        public string ListaPrecioNombre { get; set; }

        /// <summary>
        /// Propiedad auxiliar para mostrar el texto del estado en grillas
        /// </summary>
        public string EstadoTexto
        {
            get
            {
                switch (Estado)
                {
                    case 1: return "Borrado";
                    case 2: return "Emitido";
                    case 3: return "Aprobado";
                    case 4: return "Rechazado";
                    case 5: return "Vencido";
                    case 6: return "Facturado";
                    default: return "Desconocido";
                }
            }
        }

        // Detalles
        public List<PresupuestoDetalleDTO> Detalles { get; set; } = new List<PresupuestoDetalleDTO>();

        // Totales calculados
        public decimal Subtotal { get; set; }
        public decimal TotalIva { get; set; }
        public decimal Total { get; set; }

        // Propiedades adicionales para compatibilidad con el formulario
        public decimal TotalBruto { get; set; }
        public decimal TotalDescuentos { get; set; }
        public decimal TotalNeto { get; set; }
        public decimal TotalFinal { get; set; }

        public PresupuestoDTO()
        {
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

        private decimal _cantidad = 1M;
        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(0.01, 999999, ErrorMessage = "La cantidad debe estar entre 0.01 y 999,999")]
        public decimal Cantidad 
        { 
            get => _cantidad;
            set => _cantidad = value;
        }

        private decimal _precio = 0M;
        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0, 999999999, ErrorMessage = "El precio debe estar entre 0 y 999,999,999")]
        public decimal Precio 
        { 
            get => _precio;
            set => _precio = value;
        }

        private decimal _descuento = 0M;
        [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100")]
        public decimal Descuento 
        { 
            get => _descuento;
            set => _descuento = value;
        }

        public int Renglon { get; set; }

        private decimal _porcentajeIVA = 21M;
        [Range(0, 100, ErrorMessage = "El porcentaje de IVA debe estar entre 0 y 100")]
        public decimal PorcentajeIVA 
        { 
            get => _porcentajeIVA;
            set => _porcentajeIVA = value;
        }

        // Propiedades navegacionales (para visualización)
        public string Codigo { get; set; }
        public string Descripcion { get; set; }

        // Campos calculados
        public decimal Subtotal => Cantidad * Precio;
        public decimal DescuentoMonto => Subtotal * (Descuento / 100);
        public decimal Total => Subtotal - DescuentoMonto;
        public decimal Iva => Total * (PorcentajeIVA / 100);
        public decimal TotalConIva => Total + Iva;

        // Propiedades adicionales para compatibilidad con el formulario
        public decimal PrecioUnitario
        {
            get => Precio;
            set => Precio = value;
        }

        public decimal ImporteTotal => TotalConIva;
        public decimal ImporteNeto => Total;
        public decimal ImporteBruto => Subtotal;

        public PresupuestoDetalleDTO()
        {
            // Valores por defecto
            Codigo = string.Empty;
            Descripcion = string.Empty;
        }
    }
}
