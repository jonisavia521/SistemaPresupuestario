using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs.Seguridad
{
    /// <summary>
    /// DTO para visualización de usuarios en listados y consultas
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Identificador único del usuario
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre completo del usuario
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Nombre de usuario para login
        /// </summary>
        public string Usuario { get; set; }

        /// <summary>
        /// Cantidad de familias asignadas directamente al usuario
        /// </summary>
        public int CantidadFamiliasDirectas { get; set; }

        /// <summary>
        /// Cantidad de patentes asignadas directamente al usuario
        /// </summary>
        public int CantidadPatentesDirectas { get; set; }

        /// <summary>
        /// Cantidad total de permisos efectivos (directos + heredados)
        /// </summary>
        public int CantidadPermisosEfectivos { get; set; }

        /// <summary>
        /// Versión para control de concurrencia (rowversion convertido a base64)
        /// </summary>
        public string VersionConcurrencia { get; set; }

        /// <summary>
        /// Indica si el usuario está activo
        /// </summary>
        public bool Activo { get; set; } = true;
    }
}