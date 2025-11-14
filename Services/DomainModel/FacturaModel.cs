using System;
using System.Collections.Generic;

namespace Services.DomainModel
{
    /// <summary>
    /// Modelo de dominio para representar una factura completa con CAE
    /// Contiene datos necesarios para generar el documento PDF de factura legal
    /// </summary>
    public class FacturaModel
    {
        // Datos de resultado de facturación
        public bool Exitosa { get; set; }
        public string NumeroFactura { get; set; } // Formato: A00001-00000001 (Punto de Venta + Número)
        public string CAE { get; set; } // Código de Autorización Electrónico (14 dígitos)
        public DateTime? VencimientoCae { get; set; }
        public string ErrorMessage { get; set; }

        // Datos de la factura
        public DateTime FechaEmision { get; set; }
        public string TipoFactura { get; set; } // "A", "B", "C"
        public int PuntoVenta { get; set; } // Ejemplo: 1
        public int NumeroDesde { get; set; } // Primer número de factura
        public int NumeroHasta { get; set; } // Último número de factura (normalmente igual a NumeroDesde)

        // Datos de la empresa emisora
        public string EmpresaRazonSocial { get; set; }
        public string EmpresaCUIT { get; set; }
        public string EmpresaDireccion { get; set; }
        public string EmpresaLocalidad { get; set; }
        public string EmpresaProvincia { get; set; }
        public string EmpresaTelefono { get; set; }
        public string EmpresaEmail { get; set; }
        public string EmpresaCondicionIva { get; set; }
        public DateTime? EmpresaInicioActividades { get; set; }

        // Datos del cliente
        public string ClienteCodigo { get; set; }
        public string ClienteRazonSocial { get; set; }
        public string ClienteCUIT { get; set; }
        public string ClienteDireccion { get; set; }
        public string ClienteLocalidad { get; set; }
        public string ClienteProvincia { get; set; }
        public string ClienteCondicionIva { get; set; }
        public string ClienteCondicionPago { get; set; }

        // Datos del vendedor (opcional)
        public string VendedorNombre { get; set; }

        // Presupuestos que originan esta factura
        public List<string> NumerosPresupuestos { get; set; }

        // Detalles (artículos acumulados de todos los presupuestos)
        public List<FacturaDetalleModel> Detalles { get; set; }

        // Totales (acumulados de todos los presupuestos)
        public decimal Subtotal { get; set; } // Base imponible (sin IVA)
        public decimal TotalIva { get; set; } // Total de IVA
        public decimal ImporteArba { get; set; } // Percepción IIBB ARBA
        public decimal Total { get; set; } // Total final

        // Datos adicionales para el PDF
        public string CodigoBarras { get; set; } // Código de barras generado a partir del CAE
        public string QRCodeData { get; set; } // Datos para generar código QR (opcional)

        public FacturaModel()
        {
            NumerosPresupuestos = new List<string>();
            Detalles = new List<FacturaDetalleModel>();
            FechaEmision = DateTime.Now;
        }
    }

    /// <summary>
    /// Modelo de dominio para representar un detalle (artículo) de factura
    /// Puede acumular cantidades de múltiples presupuestos
    /// </summary>
    public class FacturaDetalleModel
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Descuento { get; set; } // Porcentaje de descuento
        public decimal PorcentajeIVA { get; set; } // Porcentaje de IVA del producto
        public decimal Total { get; set; } // Subtotal sin IVA (después de descuento)

        // Campos calculados para mostrar en el PDF
        public decimal Subtotal => Cantidad * PrecioUnitario;
        public decimal ImporteDescuento => Subtotal * (Descuento / 100);
        public decimal ImporteNeto => Subtotal - ImporteDescuento;
        public decimal ImporteIVA => Total * (PorcentajeIVA / 100);
        public decimal ImporteTotal => Total + ImporteIVA;
    }
}
