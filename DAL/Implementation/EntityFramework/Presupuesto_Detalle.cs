namespace DAL.Implementation.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Presupuesto_Detalle
    {
        public Guid ID { get; set; }

        [StringLength(50)]
        public string Numero { get; set; }

        public Guid? IdPresupuesto { get; set; }

        public Guid? IdProducto { get; set; }

        public decimal? Cantidad { get; set; }

        public decimal? Precio { get; set; }

        public decimal? Descuento { get; set; }

        public int? Renglon { get; set; }

        public virtual Presupuesto Presupuesto { get; set; }

        public virtual Producto Producto { get; set; }
    }
}
