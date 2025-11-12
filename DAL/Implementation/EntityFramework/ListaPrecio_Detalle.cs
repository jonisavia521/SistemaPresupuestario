namespace DAL.Implementation.EntityFramework
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ListaPrecio_Detalle")]
    public partial class ListaPrecio_Detalle
    {
        public Guid ID { get; set; }

        [Required]
        public Guid IdListaPrecio { get; set; }

        [Required]
        public Guid IdProducto { get; set; }

        [Required]
        public decimal Precio { get; set; }

        public virtual ListaPrecio ListaPrecio { get; set; }

        public virtual Producto Producto { get; set; }
    }
}
