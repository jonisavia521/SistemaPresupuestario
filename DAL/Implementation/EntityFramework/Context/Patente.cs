using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Implementation.EntityFramework.Context
{
    public partial class Patente
    {
        public Patente()
        {
            FamiliaPatente = new HashSet<FamiliaPatente>();
            UsuarioPatente = new HashSet<UsuarioPatente>();
        }

        public string IdPatente { get; set; }
        public string Nombre { get; set; }
        public string Vista { get; set; }
        public byte[] Timestamp { get; set; }

        public virtual ICollection<FamiliaPatente> FamiliaPatente { get; set; }
        public virtual ICollection<UsuarioPatente> UsuarioPatente { get; set; }
    }
}
