using System;
using System.Collections.Generic;

namespace BLL.DTOs.Seguridad
{
    /// <summary>
    /// DTO que representa un permiso efectivo de un usuario con información
    /// sobre su origen (directo o heredado)
    /// </summary>
    public class PermisoEfectivoDto
    {
        /// <summary>
        /// Información de la patente
        /// </summary>
        public PatenteDto Patente { get; set; }

        /// <summary>
        /// Indica si es un permiso asignado directamente al usuario
        /// </summary>
        public bool EsDirecto { get; set; }

        /// <summary>
        /// Indica si es un permiso heredado de alguna familia
        /// </summary>
        public bool EsHeredado { get; set; }

        /// <summary>
        /// Lista de orígenes de donde proviene el permiso
        /// (puede ser múltiple si está asignado directamente Y heredado)
        /// </summary>
        public List<OrigenPermisoDto> Origenes { get; set; } = new List<OrigenPermisoDto>();

        /// <summary>
        /// Cadena descriptiva del origen del permiso para mostrar al usuario
        /// </summary>
        public string DescripcionOrigen
        {
            get
            {
                if (EsDirecto && EsHeredado)
                    return $"Directo + Heredado ({string.Join(", ", Origenes.ConvertAll(o => o.Nombre))})";
                else if (EsDirecto)
                    return "Directo";
                else if (EsHeredado)
                    return $"Heredado ({string.Join(", ", Origenes.ConvertAll(o => o.Nombre))})";
                else
                    return "Sin origen";
            }
        }
    }

    /// <summary>
    /// Representa el origen específico de un permiso
    /// </summary>
    public class OrigenPermisoDto
    {
        /// <summary>
        /// Identificador del origen
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del origen (nombre de familia o "Directo")
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Tipo de origen
        /// </summary>
        public TipoOrigenPermiso Tipo { get; set; }
    }

    /// <summary>
    /// Tipos de origen de permisos
    /// </summary>
    public enum TipoOrigenPermiso
    {
        /// <summary>
        /// Permiso asignado directamente al usuario
        /// </summary>
        Directo,

        /// <summary>
        /// Permiso heredado de una familia
        /// </summary>
        Familia
    }
}