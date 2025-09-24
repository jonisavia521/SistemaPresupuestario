namespace DAL.Implementation.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Familia")]
    public partial class Familia
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Familia()
        {
            RelacionesComoPadre = new HashSet<Familia_Familia>();
            RelacionesComoHijo = new HashSet<Familia_Familia>();
            Familia_Patente = new HashSet<Familia_Patente>();
            Usuario_Familia = new HashSet<Usuario_Familia>();
        }

        [StringLength(1000)]
        public string Nombre { get; set; }

        [Key]
        public Guid IdFamilia { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Familia_Familia> RelacionesComoPadre { get; set; }   // Hijos
      


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Familia_Familia> RelacionesComoHijo { get; set; }    // Padres

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Familia_Patente> Familia_Patente { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Usuario_Familia> Usuario_Familia { get; set; }
    }
}
