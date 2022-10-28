using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Implementation.EntityFramework.Context
{
    public partial class Familia
    {
        public Familia()
        {
            FamiliaFamiliaIdFamiliaHijoNavigation = new HashSet<FamiliaFamilia>();
            FamiliaFamiliaIdFamiliaNavigation = new HashSet<FamiliaFamilia>();
            FamiliaPatente = new HashSet<FamiliaPatente>();
            UsuarioFamilia = new HashSet<UsuarioFamilia>();
        }

        public string IdFamilia { get; set; }
        public string Nombre { get; set; }
        public byte[] Timestamp { get; set; }

        public virtual ICollection<FamiliaFamilia> FamiliaFamiliaIdFamiliaHijoNavigation { get; set; }
        public virtual ICollection<FamiliaFamilia> FamiliaFamiliaIdFamiliaNavigation { get; set; }
        public virtual ICollection<FamiliaPatente> FamiliaPatente { get; set; }
        public virtual ICollection<UsuarioFamilia> UsuarioFamilia { get; set; }
    }
}
