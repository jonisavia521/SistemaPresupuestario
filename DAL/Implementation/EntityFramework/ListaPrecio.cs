namespace DAL.Implementation.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ListaPrecio")]
    public partial class ListaPrecio
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ListaPrecio()
        {
            ListaPrecio_Detalle = new HashSet<ListaPrecio_Detalle>();
            Presupuesto = new HashSet<Presupuesto>();
        }

        public Guid ID { get; set; }

        [Required]
        [StringLength(2)]
        public string Codigo { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        public bool Activo { get; set; }

        [Required]
        public bool IncluyeIva { get; set; }

        [Required]
        public DateTime FechaAlta { get; set; }

        public DateTime? FechaModificacion { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ListaPrecio_Detalle> ListaPrecio_Detalle { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Presupuesto> Presupuesto { get; set; }
    }
}
