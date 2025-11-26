using BLL.DTOs;
using System;

namespace BLL.Contracts
{
    /// <summary>
    /// Contrato que define las operaciones de lógica de negocio disponibles para la gestión de configuración del sistema.
    /// 
    /// La configuración es un conjunto único de parámetros globales que afectan el funcionamiento de toda la aplicación,
    /// como los datos de la empresa (razón social, CUIT, domicilio), el idioma del sistema, y otros parámetros técnicos.
    /// Como solo existe un registro de configuración por sistema, las operaciones están optimizadas para esta singularidad.
    /// </summary>
    public interface IConfiguracionService
    {
        /// <summary>
        /// Obtiene la configuración única del sistema.
        /// 
        /// Este método recupera todos los parámetros de configuración global del sistema.
        /// Como existe un único registro de configuración, no requiere parámetros de búsqueda.
        /// </summary>
        /// <returns>
        /// Un objeto ConfiguracionDTO que contiene todos los parámetros de configuración del sistema.
        /// Retorna null si no existe configuración previa (sistema no inicializado).
        /// </returns>
        ConfiguracionDTO ObtenerConfiguracion();

        /// <summary>
        /// Crea o actualiza la configuración del sistema.
        /// 
        /// Como solo existe un registro único de configuración, esta operación realiza un upsert:
        /// - Si no existe configuración previa, crea un nuevo registro.
        /// - Si existe, actualiza el registro existente con los nuevos datos.
        /// 
        /// Esta operación es utilizada cuando el usuario modifica parámetros de configuración
        /// en la pantalla de "Configuración General" de la aplicación.
        /// </summary>
        /// <param name="dto">
        /// Objeto ConfiguracionDTO que contiene los parámetros de configuración a guardar.
        /// Típicamente incluye: nombre de empresa, CUIT, dirección, teléfono, email, idioma, etc.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Puede lanzarse si ocurre un error al persistir los datos en la base de datos.
        /// </exception>
        void GuardarConfiguracion(ConfiguracionDTO dto);

        /// <summary>
        /// Verifica si ya existe configuración registrada en el sistema.
        /// 
        /// Este método es útil para validar en el proceso de inicio si el sistema ha sido
        /// inicializado. Si retorna false, se debe solicitar al usuario que complete
        /// la configuración inicial antes de proceder.
        /// </summary>
        /// <returns>
        /// true si existe un registro de configuración en la base de datos;
        /// false si no existe configuración previa (sistema no ha sido inicializado).
        /// </returns>
        bool ExisteConfiguracion();
    }
}
