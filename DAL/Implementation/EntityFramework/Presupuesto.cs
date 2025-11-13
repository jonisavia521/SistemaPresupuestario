namespace DAL.Implementation.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Presupuesto")]
    public partial class Presupuesto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Presupuesto()
        {
            Presupuesto_Detalle = new HashSet<Presupuesto_Detalle>();
            Presupuesto1 = new HashSet<Presupuesto>();
        }

        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Numero { get; set; }

        public Guid IdCliente { get; set; }

        public DateTime FechaEmision { get; set; }

        public int Estado { get; set; }

        public DateTime? FechaVencimiento { get; set; }

        public Guid? IdPresupuestoPadre { get; set; }

        public Guid? IdVendedor { get; set; }

        public Guid? IdListaPrecio { get; set; }

        // NUEVOS CAMPOS: Totales persistidos
        public decimal Subtotal { get; set; }

        public decimal TotalIva { get; set; }

        public decimal Total { get; set; }

        public virtual Cliente Cliente { get; set; }

        public virtual ListaPrecio ListaPrecio { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Presupuesto_Detalle> Presupuesto_Detalle { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Presupuesto> Presupuesto1 { get; set; }

        public virtual Presupuesto Presupuesto2 { get; set; }

        public virtual Vendedor Vendedor { get; set; }
    }
}
