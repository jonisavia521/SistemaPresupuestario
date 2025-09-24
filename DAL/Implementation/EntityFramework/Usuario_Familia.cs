namespace DAL.Implementation.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Usuario_Familia
    {
        [Key]
        [Column(Order = 0)]
        public Guid IdUsuario { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid IdFamilia { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        public virtual Familia Familia { get; set; }

        public virtual Usuario Usuario { get; set; }
    }
}
