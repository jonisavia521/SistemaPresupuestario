using System;

namespace BLL.Contracts.Seguridad
{
    /// <summary>
    /// Interfaz simple para logging básico
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Registra un mensaje informativo
        /// </summary>
        /// <param name="message">Mensaje a registrar</param>
        void LogInfo(string message);

        /// <summary>
        /// Registra un mensaje de advertencia
        /// </summary>
        /// <param name="message">Mensaje a registrar</param>
        void LogWarning(string message);

        /// <summary>
        /// Registra un error
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="exception">Excepción opcional</param>
        void LogError(string message, Exception exception = null);

        /// <summary>
        /// Registra información de debug
        /// </summary>
        /// <param name="message">Mensaje de debug</param>
        void LogDebug(string message);
    }
}