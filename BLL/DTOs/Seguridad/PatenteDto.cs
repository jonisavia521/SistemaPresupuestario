using System;

namespace BLL.DTOs.Seguridad
{
    /// <summary>
    /// DTO para patentes (permisos individuales)
    /// </summary>
    public class PatenteDto
    {
        /// <summary>
        /// Identificador único de la patente
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre descriptivo de la patente
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Vista o formulario asociado a la patente
        /// </summary>
        public string Vista { get; set; }

        /// <summary>
        /// Versión para control de concurrencia
        /// </summary>
        public string VersionConcurrencia { get; set; }

        /// <summary>
        /// Indica si está asignada directamente al usuario
        /// (usado en contexto de asignación de permisos)
        /// </summary>
        public bool AsignadaDirectamente { get; set; } = false;

        /// <summary>
        /// Indica si es un permiso heredado de alguna familia
        /// </summary>
        public bool EsHeredada { get; set; } = false;

        /// <summary>
        /// Nombre de la familia de origen (si es heredada)
        /// </summary>
        public string FamiliaOrigen { get; set; }
    }
}