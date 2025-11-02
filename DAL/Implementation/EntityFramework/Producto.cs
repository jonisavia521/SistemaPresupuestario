using DAL.Implementation.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

public partial class Producto
{
    public Producto()
    {
        this.Comprobante_Detalle = new HashSet<Comprobante_Detalle>();
        this.Presupuesto_Detalle = new HashSet<Presupuesto_Detalle>();
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid ID { get; set; } 

    [Required] 
    [StringLength(50)] 
    public string Codigo { get; set; } 

    [StringLength(50)] 
    public string Descripcion { get; set; }

    [Required] 
    public bool Inhabilitado { get; set; }

    [Required]
    public DateTime FechaAlta { get; set; } 

    [Required] 
    public int UsuarioAlta { get; set; }

    public virtual ICollection<Comprobante_Detalle> Comprobante_Detalle { get; set; }
    public virtual ICollection<Presupuesto_Detalle> Presupuesto_Detalle { get; set; }
}