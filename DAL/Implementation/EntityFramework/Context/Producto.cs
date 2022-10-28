using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Implementation.EntityFramework.Context
{
    public partial class Producto
    {
        public Producto()
        {
            ComprobanteDetalle = new HashSet<ComprobanteDetalle>();
            PresupuestoDetalle = new HashSet<PresupuestoDetalle>();
        }

        public Guid Id { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int? Estado { get; set; }
        public DateTime? FechaAlta { get; set; }
        public int? UsuarioAlta { get; set; }

        public virtual ICollection<ComprobanteDetalle> ComprobanteDetalle { get; set; }
        public virtual ICollection<PresupuestoDetalle> PresupuestoDetalle { get; set; }
    }
}
