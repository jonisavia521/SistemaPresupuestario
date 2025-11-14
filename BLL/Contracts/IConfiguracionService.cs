using BLL.DTOs;
using System;

namespace BLL.Contracts
{
    /// <summary>
    /// Contrato para el servicio de lógica de negocio de Configuración
    /// Define las operaciones disponibles desde la UI
    /// </summary>
    public interface IConfiguracionService
    {
        /// <summary>
        /// Obtiene la configuración única del sistema
        /// Si no existe, devuelve null
        /// </summary>
        ConfiguracionDTO ObtenerConfiguracion();

        /// <summary>
        /// Crea o actualiza la configuración del sistema
        /// Como solo existe un registro, siempre actualiza el existente o crea uno nuevo
        /// </summary>
        void GuardarConfiguracion(ConfiguracionDTO dto);

        /// <summary>
        /// Verifica si ya existe configuración en el sistema
        /// </summary>
        bool ExisteConfiguracion();
    }
}
