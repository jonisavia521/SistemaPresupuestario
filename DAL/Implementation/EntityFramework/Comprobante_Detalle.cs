namespace DAL.Implementation.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Comprobante_Detalle
    {
        public Guid ID { get; set; }

        public Guid IdComprobante { get; set; }

        [StringLength(50)]
        public string TipoComprobante { get; set; }

        [StringLength(50)]
        public string NumeroComprobante { get; set; }

        public Guid? IdProducto { get; set; }

        public DateTime? FechaMovimiento { get; set; }

        [StringLength(50)]
        public string Renglon { get; set; }

        public decimal? Cantidad { get; set; }

        public decimal? ImporteNeto { get; set; }

        public decimal? PrecioNeto { get; set; }

        public virtual Comprobantes Comprobantes { get; set; }

        public virtual Producto Producto { get; set; }
    }
}
