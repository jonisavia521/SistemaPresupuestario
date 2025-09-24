namespace DAL.Implementation.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Cliente_Impuesto
    {
        [Key]
        [Column(Order = 0)]
        public Guid IdImpuesto { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid IdTipoImpuesto { get; set; }

        [Key]
        [Column(Order = 2)]
        public Guid IdCliente { get; set; }

        public virtual Impuesto Impuesto { get; set; }

        public virtual TipoImpuesto TipoImpuesto { get; set; }
    }
}
