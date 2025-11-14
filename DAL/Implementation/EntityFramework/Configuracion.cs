using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Implementation.EntityFramework
{
    /// <summary>
    /// Entidad EF para la tabla Configuracion
    /// Esta tabla siempre tendrá un único registro
    /// </summary>
    [Table("Configuracion")]
    public partial class Configuracion
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string RazonSocial { get; set; }

        [Required]
        [StringLength(11)]
        public string CUIT { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoIva { get; set; }

        [StringLength(200)]
        public string Direccion { get; set; }

        [StringLength(100)]
        public string Localidad { get; set; }

        public Guid? IdProvincia { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Telefono { get; set; }

        [Required]
        [StringLength(10)]
        public string Idioma { get; set; }

        public DateTime FechaAlta { get; set; }

        public DateTime? FechaModificacion { get; set; }

        // Navegación a Provincia
        public virtual Provincia Provincia { get; set; }
    }
}
