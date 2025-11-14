using Services.DomainModel;
using System;

namespace Services.Services.Contracts
{
    /// <summary>
    /// Contrato para el servicio de facturación electrónica
    /// Define las operaciones de generación de CAE y facturación
    /// </summary>
    public interface IFacturacionService
    {
        /// <summary>
        /// Genera una factura con CAE para uno o varios presupuestos aprobados del mismo cliente
        /// </summary>
        /// <param name="idsPresupuestos">IDs de los presupuestos aprobados a facturar</param>
        /// <returns>Modelo de factura con CAE y todos los datos</returns>
        FacturaModel GenerarFacturaConCAE(Guid[] idsPresupuestos);

        /// <summary>
        /// Genera el PDF de la factura y lo guarda en el sistema
        /// </summary>
        /// <param name="factura">Modelo de factura con todos los datos</param>
        /// <returns>Ruta completa del archivo PDF generado</returns>
        string GenerarPdfFactura(FacturaModel factura);

        /// <summary>
        /// Genera el PDF de la factura y lo abre automáticamente
        /// </summary>
        /// <param name="factura">Modelo de factura con todos los datos</param>
        void GenerarYAbrirPdfFactura(FacturaModel factura);
    }
}
