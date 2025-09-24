using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaPresupuestario.BLL.DTOs
{
    /// <summary>
    /// Data Transfer Object for effective permissions calculation
    /// Provides detailed information about user's permissions
    /// </summary>
    public class PermisoEfectivoDto
    {
        public Guid UsuarioId { get; set; }

        /// <summary>
        /// Patents directly assigned to the user
        /// </summary>
        public List<PatenteDto> PatentesDirectas { get; set; }

        /// <summary>
        /// Patents inherited from families
        /// </summary>
        public List<PatenteDto> PatentesHeredadas { get; set; }

        /// <summary>
        /// All effective patents (union of direct and inherited)
        /// </summary>
        public List<PatenteDto> PatentesEfectivas { get; set; }

        /// <summary>
        /// Families assigned to the user
        /// </summary>
        public List<FamiliaDto> FamiliasAsignadas { get; set; }

        /// <summary>
        /// Detailed breakdown by family showing which patents come from which family
        /// </summary>
        public List<FamiliaPermisoDto> DesglosePorFamilia { get; set; }

        /// <summary>
        /// Summary counts
        /// </summary>
        public PermisoResumenDto Resumen { get; set; }

        public PermisoEfectivoDto()
        {
            PatentesDirectas = new List<PatenteDto>();
            PatentesHeredadas = new List<PatenteDto>();
            PatentesEfectivas = new List<PatenteDto>();
            FamiliasAsignadas = new List<FamiliaDto>();
            DesglosePorFamilia = new List<FamiliaPermisoDto>();
            Resumen = new PermisoResumenDto();
        }

        /// <summary>
        /// Calculates and updates the summary information
        /// </summary>
        public void CalcularResumen()
        {
            Resumen.PatentesDirectasCount = PatentesDirectas?.Count ?? 0;
            Resumen.PatentesHeredadasCount = PatentesHeredadas?.Count ?? 0;
            Resumen.PatentesEfectivasCount = PatentesEfectivas?.Count ?? 0;
            Resumen.FamiliasCount = FamiliasAsignadas?.Count ?? 0;
            Resumen.TienePermisos = Resumen.PatentesEfectivasCount > 0;
        }
    }

    /// <summary>
    /// DTO for family-specific permission breakdown
    /// </summary>
    public class FamiliaPermisoDto
    {
        public Guid FamiliaId { get; set; }
        public string FamiliaNombre { get; set; }
        public int Nivel { get; set; }
        public List<PatenteDto> PatentesDirectas { get; set; }
        public List<PatenteDto> PatentesDeHijos { get; set; }
        public int TotalPatentes => (PatentesDirectas?.Count ?? 0) + (PatentesDeHijos?.Count ?? 0);

        public FamiliaPermisoDto()
        {
            PatentesDirectas = new List<PatenteDto>();
            PatentesDeHijos = new List<PatenteDto>();
        }
    }

    /// <summary>
    /// Summary information for permissions
    /// </summary>
    public class PermisoResumenDto
    {
        public int PatentesDirectasCount { get; set; }
        public int PatentesHeredadasCount { get; set; }
        public int PatentesEfectivasCount { get; set; }
        public int FamiliasCount { get; set; }
        public bool TienePermisos { get; set; }

        /// <summary>
        /// Percentage of permissions that are inherited vs direct
        /// </summary>
        public double PorcentajeHeredadas
        {
            get
            {
                if (PatentesEfectivasCount == 0) return 0;
                return (double)PatentesHeredadasCount / PatentesEfectivasCount * 100;
            }
        }

        /// <summary>
        /// Gets a summary text description
        /// </summary>
        public string DescripcionResumen
        {
            get
            {
                if (!TienePermisos)
                    return "Usuario sin permisos asignados";

                var partes = new List<string>();
                
                if (PatentesDirectasCount > 0)
                    partes.Add($"{PatentesDirectasCount} patente(s) directa(s)");
                
                if (PatentesHeredadasCount > 0)
                    partes.Add($"{PatentesHeredadasCount} patente(s) heredada(s)");
                
                if (FamiliasCount > 0)
                    partes.Add($"desde {FamiliasCount} familia(s)");

                return string.Join(", ", partes);
            }
        }
    }
}