using System;
using System.Collections.Generic;

namespace BLL.DTOs
{
    /// <summary>
    /// DTO para representar los permisos efectivos de un usuario
    /// </summary>
    public class PermisoEfectivoDto
    {
        public Guid IdUsuario { get; set; }

        /// <summary>
        /// Patentes asignadas directamente al usuario
        /// </summary>
        public List<PatenteDto> PatentesDirectas { get; set; } = new List<PatenteDto>();

        /// <summary>
        /// Patentes heredadas a través de familias
        /// </summary>
        public List<PatenteHeredadaDto> PatentesHeredadas { get; set; } = new List<PatenteHeredadaDto>();

        /// <summary>
        /// Familias asignadas directamente al usuario
        /// </summary>
        public List<FamiliaDto> FamiliasAsignadas { get; set; } = new List<FamiliaDto>();

        /// <summary>
        /// Total de permisos únicos (sin duplicados)
        /// </summary>
        public int TotalPermisosUnicos { get; set; }
    }

    /// <summary>
    /// Representa una patente heredada con información sobre su origen
    /// </summary>
    public class PatenteHeredadaDto
    {
        public PatenteDto Patente { get; set; }
        
        /// <summary>
        /// Nombre de la familia de la cual se hereda
        /// </summary>
        public string OrigenFamilia { get; set; }
        
        /// <summary>
        /// ID de la familia de la cual se hereda
        /// </summary>
        public Guid IdFamiliaOrigen { get; set; }
    }
}