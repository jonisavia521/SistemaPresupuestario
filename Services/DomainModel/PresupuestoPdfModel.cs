using System;
using System.Collections.Generic;

namespace Services.DomainModel
{
    /// <summary>
    /// Modelo de dominio para representar un presupuesto completo para PDF
    /// Contiene solo los datos necesarios para generar el documento
    /// </summary>
    public class PresupuestoPdfModel
    {
        public Guid Id { get; set; }
        public string Numero { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string Estado { get; set; }

        // Cliente
        public string ClienteCodigo { get; set; }
        public string ClienteRazonSocial { get; set; }

        // Vendedor
        public string VendedorNombre { get; set; }

        // Totales
        public decimal Subtotal { get; set; }
        public decimal TotalIva { get; set; }
        public decimal ImporteArba { get; set; }
        public decimal Total { get; set; }

        // Detalles
        public List<PresupuestoDetallePdfModel> Detalles { get; set; }

        public PresupuestoPdfModel()
        {
            Detalles = new List<PresupuestoDetallePdfModel>();
        }
    }

    /// <summary>
    /// Modelo de dominio para representar un detalle de presupuesto para PDF
    /// </summary>
    public class PresupuestoDetallePdfModel
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
    }
}
