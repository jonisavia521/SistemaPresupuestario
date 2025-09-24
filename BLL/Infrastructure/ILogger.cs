using System;

namespace BLL.Infrastructure
{
    /// <summary>
    /// Interfaz simple para logging en el módulo de usuarios
    /// DECISIÓN: Logger básico para no introducir dependencias externas
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Registra mensaje informativo
        /// </summary>
        /// <param name="message">Mensaje a registrar</param>
        /// <param name="context">Contexto opcional (nombre de método, clase, etc.)</param>
        void Info(string message, string context = null);

        /// <summary>
        /// Registra advertencia
        /// </summary>
        /// <param name="message">Mensaje de advertencia</param>
        /// <param name="context">Contexto opcional</param>
        void Warn(string message, string context = null);

        /// <summary>
        /// Registra error
        /// </summary>
        /// <param name="message">Mensaje de error</param>
        /// <param name="exception">Excepción asociada (opcional)</param>
        /// <param name="context">Contexto opcional</param>
        void Error(string message, Exception exception = null, string context = null);

        /// <summary>
        /// Registra mensaje de debug (solo en modo desarrollo)
        /// </summary>
        /// <param name="message">Mensaje de debug</param>
        /// <param name="context">Contexto opcional</param>
        void Debug(string message, string context = null);
    }
}