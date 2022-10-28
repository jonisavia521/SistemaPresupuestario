using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Implementation.EntityFramework.Context
{
    public partial class PresupuestoDetalle
    {
        public Guid Id { get; set; }
        public string Numero { get; set; }
        public Guid? IdPresupuesto { get; set; }
        public Guid? IdProducto { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? Precio { get; set; }
        public decimal? Descuento { get; set; }
        public int? Renglon { get; set; }

        public virtual Presupuesto IdPresupuestoNavigation { get; set; }
        public virtual Producto IdProductoNavigation { get; set; }
    }
}
