using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaPresupuestario.BLL.DTOs
{
    /// <summary>
    /// Data Transfer Object for Patente entity
    /// </summary>
    public class PatenteDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Nombre { get; set; }

        [StringLength(200, ErrorMessage = "La vista no puede exceder los 200 caracteres")]
        public string Vista { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Count of users who have this patent directly assigned
        /// </summary>
        public int UsuariosDirectosCount { get; set; }

        /// <summary>
        /// Count of families that contain this patent
        /// </summary>
        public int FamiliasCount { get; set; }

        /// <summary>
        /// Total count of users who have this patent (direct + through families)
        /// </summary>
        public int UsuariosEfectivosCount { get; set; }

        /// <summary>
        /// Whether this patent is inherited (for display in user context)
        /// </summary>
        public bool EsHeredada { get; set; }

        /// <summary>
        /// Source of inheritance (family name, if inherited)
        /// </summary>
        public string FuenteHerencia { get; set; }

        /// <summary>
        /// Row version for concurrency control
        /// </summary>
        public byte[] RowVersion { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public PatenteDto()
        {
            EsHeredada = false;
        }

        /// <summary>
        /// Gets display name with view information
        /// </summary>
        public string NombreCompleto
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Vista))
                {
                    return $"{Nombre} ({Vista})";
                }
                return Nombre;
            }
        }

        /// <summary>
        /// Gets display text with usage information
        /// </summary>
        public string NombreConUso
        {
            get
            {
                return $"{Nombre} - {UsuariosDirectosCount} usuarios directos, {FamiliasCount} familias, {UsuariosEfectivosCount} usuarios efectivos";
            }
        }

        /// <summary>
        /// Gets display text for user context (shows inheritance source)
        /// </summary>
        public string NombreParaUsuario
        {
            get
            {
                if (EsHeredada && !string.IsNullOrWhiteSpace(FuenteHerencia))
                {
                    return $"{Nombre} (heredada de {FuenteHerencia})";
                }
                return Nombre;
            }
        }
    }
}