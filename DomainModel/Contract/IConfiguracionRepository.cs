using DomainModel.Domain;
using System;

namespace DomainModel.Contract
{
    /// <summary>
    /// Contrato para el repositorio de Configuracion
    /// Esta tabla siempre tendrá un único registro
    /// </summary>
    public interface IConfiguracionRepository : IRepository<ConfiguracionDM>
    {
        /// <summary>
        /// Obtiene la configuración única del sistema
        /// Si no existe, devuelve null
        /// </summary>
        ConfiguracionDM ObtenerConfiguracion();

        /// <summary>
        /// Verifica si ya existe un registro de configuración
        /// </summary>
        bool ExisteConfiguracion();
    }
}
