using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace DAL.Implementation.EntityFramework.Context
{
    public partial class Cliente
    {
        public Cliente()
        {
            Comprobantes = new HashSet<Comprobantes>();
            Presupuesto = new HashSet<Presupuesto>();
        }

        public Guid Id { get; set; }
        public string CodigoCliente { get; set; }
        public string RazonSocial { get; set; }
        public string IdProvincia { get; set; }
        public string Localidad { get; set; }
        public string DireccionLegal { get; set; }
        public string DireccionComercial { get; set; }
        public string Cuit { get; set; }
        public Guid? IdVendedor { get; set; }

        public virtual Vendedor IdVendedorNavigation { get; set; }
        public virtual ICollection<Comprobantes> Comprobantes { get; set; }
        public virtual ICollection<Presupuesto> Presupuesto { get; set; }
    }
}
