namespace DAL.Implementation.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Vendedor")]
    public partial class Vendedor
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Vendedor()
        {
            Cliente = new HashSet<Cliente>();
            Comprobantes = new HashSet<Comprobantes>();
            Presupuesto = new HashSet<Presupuesto>();
        }

        public Guid ID { get; set; }

        [StringLength(50)]
        public string Nombre { get; set; }

        [StringLength(50)]
        public string Mail { get; set; }

        [StringLength(50)]
        public string Direccion { get; set; }

        [StringLength(50)]
        public string Telefono { get; set; }
        
        // Nuevos campos para ABM
        [StringLength(20)]
        public string CodigoVendedor { get; set; }
        
        [StringLength(11)]
        public string CUIT { get; set; }
        
        public decimal? PorcentajeComision { get; set; }
        
        public bool Activo { get; set; }
        
        public DateTime FechaAlta { get; set; }
        
        public DateTime? FechaModificacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cliente> Cliente { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comprobantes> Comprobantes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Presupuesto> Presupuesto { get; set; }
    }
}
