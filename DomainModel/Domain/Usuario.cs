using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Domain
{
    public partial class Usuario
    {
        
        public Guid IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Usuario_ { get; set; }
        public string Clave { get; set; }

    }
}
