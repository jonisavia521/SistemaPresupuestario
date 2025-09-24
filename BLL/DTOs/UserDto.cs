using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// DTO para visualización de usuarios en listados
    /// Incluye información resumida y conteos de permisos
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Identificador único del usuario
        /// </summary>
        [Browsable(false)]
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre de usuario para login (único en el sistema)
        /// </summary>
        [DisplayName("Usuario")]
        public string Usuario { get; set; }

        /// <summary>
        /// Nombre completo del usuario
        /// </summary>
        [DisplayName("Nombre")]
        public string Nombre { get; set; }

        /// <summary>
        /// Cantidad de permisos asignados directamente (patentes individuales)
        /// </summary>
        [DisplayName("Permisos Directos")]
        public int CantPermisosDirectos { get; set; }

        /// <summary>
        /// Cantidad total de permisos efectivos (directos + heredados de familias)
        /// </summary>
        [DisplayName("Permisos Efectivos")]
        public int CantPermisosEfectivos { get; set; }

        /// <summary>
        /// Estado del usuario (opcional para futuras implementaciones)
        /// </summary>
        [Browsable(false)]
        public bool? Estado { get; set; }

        /// <summary>
        /// Timestamp para control de concurrencia (codificado en Base64)
        /// </summary>
        [Browsable(false)]
        public string TimestampBase64 { get; set; }
    }
}