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
            Presupuesto = new HashSet<Presupuesto>();
        }

        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [StringLength(50)]
        public string Mail { get; set; }

        [StringLength(50)]
        public string Direccion { get; set; }

        [StringLength(50)]
        public string Telefono { get; set; }
        
        // Campos obligatorios según el script SQL
        [Required]
        [StringLength(20)]
        public string CodigoVendedor { get; set; }
        
        [Required]
        [StringLength(11)]
        public string CUIT { get; set; }
        
        [Required]
        public decimal PorcentajeComision { get; set; }
        
        [Required]
        public bool Activo { get; set; }
        
        [Required]
        public DateTime FechaAlta { get; set; }
        
        public DateTime? FechaModificacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cliente> Cliente { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Presupuesto> Presupuesto { get; set; }
    }
}
