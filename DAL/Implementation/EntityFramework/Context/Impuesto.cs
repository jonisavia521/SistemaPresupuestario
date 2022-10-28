using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Implementation.EntityFramework.Context
{
    public partial class Impuesto
    {
        public Impuesto()
        {
            ClienteImpuesto = new HashSet<ClienteImpuesto>();
        }

        public Guid Id { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public double Alicuota { get; set; }
        public Guid? IdProvincia { get; set; }
        public Guid IdTipoImpuesto { get; set; }

        public virtual Provincia IdProvinciaNavigation { get; set; }
        public virtual TipoImpuesto IdTipoImpuestoNavigation { get; set; }
        public virtual ICollection<ClienteImpuesto> ClienteImpuesto { get; set; }
    }
}
