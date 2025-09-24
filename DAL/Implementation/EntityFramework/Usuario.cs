namespace DAL.Implementation.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Usuario")]
    public partial class Usuario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Usuario()
        {
            Usuario_Familia = new HashSet<Usuario_Familia>();
            Usuario_Patente = new HashSet<Usuario_Patente>();
        }

        [Key]
        public Guid IdUsuario { get; set; }

        [StringLength(1000)]
        public string Nombre { get; set; }

        [Column("Usuario")]
        [Required]
        [StringLength(20)]
        public string Usuario1 { get; set; }

        [Required]
        [StringLength(50)]
        public string Clave { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Usuario_Familia> Usuario_Familia { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Usuario_Patente> Usuario_Patente { get; set; }
    }
}
