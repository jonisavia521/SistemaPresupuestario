namespace DAL.Implementation.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TipoImpuesto")]
    public partial class TipoImpuesto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TipoImpuesto()
        {
            Cliente_Impuesto = new HashSet<Cliente_Impuesto>();
            Impuesto = new HashSet<Impuesto>();
        }

        public Guid ID { get; set; }

        [StringLength(50)]
        public string Descripcion { get; set; }

        public bool? Definible { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cliente_Impuesto> Cliente_Impuesto { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Impuesto> Impuesto { get; set; }
    }
}
