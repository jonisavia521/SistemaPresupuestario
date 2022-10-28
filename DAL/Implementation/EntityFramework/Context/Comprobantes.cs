using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Implementation.EntityFramework.Context
{
    public partial class Comprobantes
    {
        public Comprobantes()
        {
            ComprobanteDetalle = new HashSet<ComprobanteDetalle>();
        }

        public Guid Id { get; set; }
        public Guid? IdVendedor { get; set; }
        public Guid? IdCliente { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaMovimiento { get; set; }
        public string TipoComprobante { get; set; }
        public string NumeroComprobante { get; set; }
        public decimal? Total { get; set; }
        public decimal? ImporteGravado { get; set; }
        public decimal? ImporteIva { get; set; }

        public virtual Cliente IdClienteNavigation { get; set; }
        public virtual Vendedor IdVendedorNavigation { get; set; }
        public virtual ICollection<ComprobanteDetalle> ComprobanteDetalle { get; set; }
    }
}
