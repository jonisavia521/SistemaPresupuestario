using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Implementation.EntityFramework.Context
{
    public partial class Usuario
    {
        public Usuario()
        {
            UsuarioFamilia = new HashSet<UsuarioFamilia>();
            UsuarioPatente = new HashSet<UsuarioPatente>();
        }

        public string IdUsuario { get; set; }
        public string Nombre { get; set; }
        public byte[] Timestamp { get; set; }
        public string Usuario1 { get; set; }
        public string Clave { get; set; }

        public virtual ICollection<UsuarioFamilia> UsuarioFamilia { get; set; }
        public virtual ICollection<UsuarioPatente> UsuarioPatente { get; set; }
    }
}
