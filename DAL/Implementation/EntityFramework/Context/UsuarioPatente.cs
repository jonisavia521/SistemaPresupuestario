using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Implementation.EntityFramework.Context
{
    public partial class UsuarioPatente
    {
        public string IdUsuario { get; set; }
        public string IdPatente { get; set; }
        public byte[] Timestamp { get; set; }

        public virtual Patente IdPatenteNavigation { get; set; }
        public virtual Usuario IdUsuarioNavigation { get; set; }
    }
}
