using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Implementation.EntityFramework.Context
{
    public partial class UsuarioFamilia
    {
        public string IdUsuario { get; set; }
        public string IdFamilia { get; set; }
        public byte[] Timestamp { get; set; }

        public virtual Familia IdFamiliaNavigation { get; set; }
        public virtual Usuario IdUsuarioNavigation { get; set; }
    }
}
