namespace DAL.Implementation.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Comprobantes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Comprobantes()
        {
            Comprobante_Detalle = new HashSet<Comprobante_Detalle>();
        }

        public Guid ID { get; set; }

        public Guid? IdVendedor { get; set; }

        public Guid? IdCliente { get; set; }

        [StringLength(50)]
        public string Estado { get; set; }

        public DateTime? FechaMovimiento { get; set; }

        [StringLength(50)]
        public string TipoComprobante { get; set; }

        [StringLength(50)]
        public string NumeroComprobante { get; set; }

        public decimal? Total { get; set; }

        public decimal? ImporteGravado { get; set; }

        public decimal? ImporteIVA { get; set; }

        public virtual Cliente Cliente { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comprobante_Detalle> Comprobante_Detalle { get; set; }

        public virtual Vendedor Vendedor { get; set; }
    }
}
