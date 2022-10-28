using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Implementation.EntityFramework.Context
{
    public partial class ClienteImpuesto
    {
        public Guid IdImpuesto { get; set; }
        public Guid IdTipoImpuesto { get; set; }
        public Guid IdCliente { get; set; }

        public virtual Impuesto IdImpuestoNavigation { get; set; }
        public virtual TipoImpuesto IdTipoImpuestoNavigation { get; set; }
    }
}
