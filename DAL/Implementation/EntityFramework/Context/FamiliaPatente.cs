using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Implementation.EntityFramework.Context
{
    public partial class FamiliaPatente
    {
        public Guid IdFamilia { get; set; }
        public Guid IdPatente { get; set; }
        public byte[] Timestamp { get; set; }

        public virtual Familia IdFamiliaNavigation { get; set; }
        public virtual Patente IdPatenteNavigation { get; set; }
    }
}
