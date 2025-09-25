using System;
using System.Collections.Generic;

namespace BLL.DTOs.Seguridad
{
    /// <summary>
    /// DTO para familias de permisos (grupos jerárquicos)
    /// </summary>
    public class FamiliaDto
    {
        /// <summary>
        /// Identificador único de la familia
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre descriptivo de la familia
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Lista de familias padre (puede tener múltiples padres)
        /// </summary>
        public List<FamiliaDto> FamiliasPadre { get; set; } = new List<FamiliaDto>();

        /// <summary>
        /// Lista de familias hijas
        /// </summary>
        public List<FamiliaDto> FamiliasHijas { get; set; } = new List<FamiliaDto>();

        /// <summary>
        /// Lista de patentes directas de esta familia
        /// </summary>
        public List<PatenteDto> PatentesDirectas { get; set; } = new List<PatenteDto>();

        /// <summary>
        /// Cantidad total de patentes efectivas (directas + heredadas)
        /// </summary>
        public int CantidadPatentesEfectivas { get; set; }

        /// <summary>
        /// Versión para control de concurrencia
        /// </summary>
        public string VersionConcurrencia { get; set; }

        /// <summary>
        /// Indica si está asignada directamente al usuario actual
        /// (usado en contexto de asignación de permisos)
        /// </summary>
        public bool AsignadaDirectamente { get; set; } = false;

        /// <summary>
        /// Nivel de profundidad en la jerarquía (usado para indentación en TreeView)
        /// </summary>
        public int NivelJerarquia { get; set; } = 0;
    }
}