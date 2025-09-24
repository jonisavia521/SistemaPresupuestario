namespace DAL.Implementation.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Familia_Patente
    {
        [Key]
        [Column(Order = 0)]
        public Guid IdFamilia { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid IdPatente { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        public virtual Familia Familia { get; set; }

        public virtual Patente Patente { get; set; }
    }
}
