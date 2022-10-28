using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Implementation.EntityFramework.Context
{
    public partial class TipoImpuesto
    {
        public TipoImpuesto()
        {
            ClienteImpuesto = new HashSet<ClienteImpuesto>();
            Impuesto = new HashSet<Impuesto>();
        }

        public Guid Id { get; set; }
        public string Descripcion { get; set; }
        public bool? Definible { get; set; }

        public virtual ICollection<ClienteImpuesto> ClienteImpuesto { get; set; }
        public virtual ICollection<Impuesto> Impuesto { get; set; }
    }
}
