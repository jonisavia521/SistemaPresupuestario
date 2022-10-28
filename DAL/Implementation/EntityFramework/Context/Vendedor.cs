using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Implementation.EntityFramework.Context
{
    public partial class Vendedor
    {
        public Vendedor()
        {
            Cliente = new HashSet<Cliente>();
            Comprobantes = new HashSet<Comprobantes>();
            Presupuesto = new HashSet<Presupuesto>();
        }

        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Mail { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }

        public virtual ICollection<Cliente> Cliente { get; set; }
        public virtual ICollection<Comprobantes> Comprobantes { get; set; }
        public virtual ICollection<Presupuesto> Presupuesto { get; set; }
    }
}
