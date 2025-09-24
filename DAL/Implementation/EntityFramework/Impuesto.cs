namespace DAL.Implementation.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Impuesto")]
    public partial class Impuesto
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Impuesto()
        {
            Cliente_Impuesto = new HashSet<Cliente_Impuesto>();
        }

        public Guid ID { get; set; }

        [Required]
        [StringLength(50)]
        public string Codigo { get; set; }

        [Required]
        [StringLength(50)]
        public string Descripcion { get; set; }

        public double Alicuota { get; set; }

        public Guid? IdProvincia { get; set; }

        public Guid IdTipoImpuesto { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cliente_Impuesto> Cliente_Impuesto { get; set; }

        public virtual Provincia Provincia { get; set; }

        public virtual TipoImpuesto TipoImpuesto { get; set; }
    }
}
