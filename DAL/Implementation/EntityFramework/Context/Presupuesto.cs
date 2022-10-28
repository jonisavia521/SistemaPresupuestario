using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Implementation.EntityFramework.Context
{
    public partial class Presupuesto
    {
        public Presupuesto()
        {
            InverseIdPresupuestoPadreNavigation = new HashSet<Presupuesto>();
            PresupuestoDetalle = new HashSet<PresupuestoDetalle>();
        }

        public Guid Id { get; set; }
        public string Numero { get; set; }
        public Guid IdCliente { get; set; }
        public DateTime FechaEmision { get; set; }
        public int Estado { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public Guid? IdPresupuestoPadre { get; set; }
        public Guid? IdVendedor { get; set; }

        public virtual Cliente IdClienteNavigation { get; set; }
        public virtual Presupuesto IdPresupuestoPadreNavigation { get; set; }
        public virtual Vendedor IdVendedorNavigation { get; set; }
        public virtual ICollection<Presupuesto> InverseIdPresupuestoPadreNavigation { get; set; }
        public virtual ICollection<PresupuestoDetalle> PresupuestoDetalle { get; set; }
    }
}
