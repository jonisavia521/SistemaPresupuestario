namespace DAL.Implementation.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Cliente")]
    public partial class Cliente
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cliente()
        {
            Comprobantes = new HashSet<Comprobantes>();
            Presupuesto = new HashSet<Presupuesto>();
        }

        public Guid ID { get; set; }

        [StringLength(50)]
        public string CodigoCliente { get; set; }

        [StringLength(50)]
        public string RazonSocial { get; set; }

        [StringLength(50)]
        public string IdProvincia { get; set; }

        [StringLength(50)]
        public string Localidad { get; set; }

        [StringLength(50)]
        public string DireccionLegal { get; set; }

        [StringLength(50)]
        public string DireccionComercial { get; set; }

        [StringLength(50)]
        public string CUIT { get; set; }

        public Guid? IdVendedor { get; set; }
        
        // Nuevos campos para el ABM
        [StringLength(20)]
        public string CodigoVendedor { get; set; }
        
        [StringLength(50)]
        public string TipoIva { get; set; }
        
        [StringLength(2)]
        public string CondicionPago { get; set; }
        
        [StringLength(100)]
        public string Email { get; set; }
        
        [StringLength(20)]
        public string Telefono { get; set; }
        
        public bool Activo { get; set; }
        
        public DateTime FechaAlta { get; set; }
        
        public DateTime? FechaModificacion { get; set; }

        public virtual Vendedor Vendedor { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comprobantes> Comprobantes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Presupuesto> Presupuesto { get; set; }
    }
}
