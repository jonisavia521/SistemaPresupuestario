using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Implementation.EntityFramework.Context
{
    public partial class ComprobanteDetalle
    {
        public Guid Id { get; set; }
        public Guid IdComprobante { get; set; }
        public string TipoComprobante { get; set; }
        public string NumeroComprobante { get; set; }
        public Guid? IdProducto { get; set; }
        public DateTime? FechaMovimiento { get; set; }
        public string Renglon { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? ImporteNeto { get; set; }
        public decimal? PrecioNeto { get; set; }

        public virtual Comprobantes IdComprobanteNavigation { get; set; }
        public virtual Producto IdProductoNavigation { get; set; }
    }
}
