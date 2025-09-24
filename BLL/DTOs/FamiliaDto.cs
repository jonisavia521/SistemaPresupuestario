using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BLL.DTOs
{
    /// <summary>
    /// DTO para representar familias de permisos en estructura jerárquica
    /// Soporta visualización en TreeView con checkboxes
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
        [DisplayName("Nombre")]
        public string Nombre { get; set; }

        /// <summary>
        /// Lista de familias hijas (jerarquía recursiva)
        /// </summary>
        public List<FamiliaDto> Hijos { get; set; }

        /// <summary>
        /// Lista de patentes directamente asignadas a esta familia
        /// </summary>
        public List<PatenteDto> Patentes { get; set; }

        /// <summary>
        /// Indica si esta familia está seleccionada/asignada al usuario
        /// Usado para control de checkboxes en UI
        /// </summary>
        [Browsable(false)]
        public bool EsSeleccionada { get; set; }

        /// <summary>
        /// Nivel de profundidad en la jerarquía (0 = raíz)
        /// Usado para indentación visual
        /// </summary>
        [Browsable(false)]
        public int Nivel { get; set; }

        /// <summary>
        /// ID de la familia padre (null si es raíz)
        /// </summary>
        [Browsable(false)]
        public Guid? IdPadre { get; set; }

        /// <summary>
        /// Timestamp para control de concurrencia
        /// </summary>
        [Browsable(false)]
        public string TimestampBase64 { get; set; }

        public FamiliaDto()
        {
            Hijos = new List<FamiliaDto>();
            Patentes = new List<PatenteDto>();
        }

        /// <summary>
        /// Calcula recursivamente la cantidad total de patentes en esta familia y subfamilias
        /// </summary>
        /// <returns>Número total de patentes únicas</returns>
        public int ContarPatentesTotales()
        {
            var patentesUnicas = new HashSet<Guid>();
            
            // Agregar patentes directas
            foreach (var patente in Patentes)
            {
                patentesUnicas.Add(patente.Id);
            }
            
            // Agregar patentes de hijas recursivamente
            foreach (var hijo in Hijos)
            {
                var patentesHijo = hijo.ObtenerTodasPatentesRecursivo();
                foreach (var patente in patentesHijo)
                {
                    patentesUnicas.Add(patente.Id);
                }
            }
            
            return patentesUnicas.Count;
        }

        /// <summary>
        /// Obtiene todas las patentes de esta familia y subfamilias recursivamente
        /// </summary>
        /// <returns>Lista de patentes únicas</returns>
        public List<PatenteDto> ObtenerTodasPatentesRecursivo()
        {
            var todasPatentes = new List<PatenteDto>(Patentes);
            
            foreach (var hijo in Hijos)
            {
                todasPatentes.AddRange(hijo.ObtenerTodasPatentesRecursivo());
            }
            
            return todasPatentes;
        }
    }
}