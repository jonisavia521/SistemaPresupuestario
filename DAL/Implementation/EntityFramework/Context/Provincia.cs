using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Implementation.EntityFramework.Context
{
    public partial class Provincia
    {
        public Provincia()
        {
            Impuesto = new HashSet<Impuesto>();
        }

        public Guid Id { get; set; }
        public string CodigoAfip { get; set; }
        public string Nombre { get; set; }

        public virtual ICollection<Impuesto> Impuesto { get; set; }
    }
}
