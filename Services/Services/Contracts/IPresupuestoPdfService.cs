using System;

namespace Services.Services.Contracts
{
    /// <summary>
    /// Contrato para el servicio de generación de PDFs de presupuestos
    /// La UI consume este servicio sin conocer los detalles de implementación
    /// </summary>
    public interface IPresupuestoPdfService
    {
        /// <summary>
        /// Genera un PDF del presupuesto y lo guarda en el sistema
        /// </summary>
        /// <param name="idPresupuesto">ID del presupuesto a generar</param>
        /// <returns>Ruta completa del archivo PDF generado</returns>
        string GenerarPdf(Guid idPresupuesto);

        /// <summary>
        /// Genera el PDF y lo abre automáticamente
        /// </summary>
        /// <param name="idPresupuesto">ID del presupuesto a generar</param>
        void GenerarYAbrirPdf(Guid idPresupuesto);
    }
}
