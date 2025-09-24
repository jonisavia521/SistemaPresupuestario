using System;
using System.IO;

namespace BLL.Infrastructure
{
    /// <summary>
    /// Implementación simple de ILogger que escribe a archivo
    /// DECISIÓN: Logger de archivos simple para no introducir dependencias externas
    /// </summary>
    public class SimpleFileLogger : ILogger
    {
        private readonly string _logFilePath;
        private readonly object _lockObject = new object();

        /// <summary>
        /// Constructor que especifica la ruta del archivo de log
        /// </summary>
        /// <param name="logFilePath">Ruta completa del archivo de log</param>
        public SimpleFileLogger(string logFilePath = null)
        {
            _logFilePath = logFilePath ?? GetDefaultLogPath();
            EnsureLogDirectoryExists();
        }

        /// <summary>
        /// Registra mensaje informativo
        /// </summary>
        public void Info(string message, string context = null)
        {
            WriteLog("INFO", message, context);
        }

        /// <summary>
        /// Registra advertencia
        /// </summary>
        public void Warn(string message, string context = null)
        {
            WriteLog("WARN", message, context);
        }

        /// <summary>
        /// Registra error
        /// </summary>
        public void Error(string message, Exception exception = null, string context = null)
        {
            string fullMessage = message;
            if (exception != null)
            {
                fullMessage += $" | Exception: {exception.Message} | StackTrace: {exception.StackTrace}";
            }
            WriteLog("ERROR", fullMessage, context);
        }

        /// <summary>
        /// Registra mensaje de debug (solo en modo DEBUG)
        /// </summary>
        public void Debug(string message, string context = null)
        {
#if DEBUG
            WriteLog("DEBUG", message, context);
#endif
        }

        /// <summary>
        /// Escribe línea de log al archivo
        /// </summary>
        /// <param name="level">Nivel de log</param>
        /// <param name="message">Mensaje</param>
        /// <param name="context">Contexto opcional</param>
        private void WriteLog(string level, string message, string context)
        {
            try
            {
                lock (_lockObject)
                {
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    string contextPart = string.IsNullOrEmpty(context) ? "" : $" [{context}]";
                    string logLine = $"{timestamp} [{level}]{contextPart}: {message}";

                    File.AppendAllText(_logFilePath, logLine + Environment.NewLine);
                }
            }
            catch (Exception)
            {
                // Si no se puede escribir al log, no hacer nada para evitar excepciones en cascada
                // En producción se podría escribir al Event Log de Windows como fallback
            }
        }

        /// <summary>
        /// Obtiene la ruta por defecto del archivo de log
        /// </summary>
        /// <returns>Ruta del archivo de log</returns>
        private string GetDefaultLogPath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string logDirectory = Path.Combine(appDataPath, "SistemaPresupuestario", "Logs");
            string fileName = $"Usuarios_{DateTime.Now:yyyyMMdd}.log";
            return Path.Combine(logDirectory, fileName);
        }

        /// <summary>
        /// Asegura que el directorio de logs existe
        /// </summary>
        private void EnsureLogDirectoryExists()
        {
            try
            {
                string directory = Path.GetDirectoryName(_logFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch (Exception)
            {
                // Si no se puede crear el directorio, el WriteLog fallará silenciosamente
            }
        }

        /// <summary>
        /// Limpia logs antiguos (mantiene solo los últimos 30 días)
        /// </summary>
        public void CleanOldLogs()
        {
            try
            {
                string directory = Path.GetDirectoryName(_logFilePath);
                if (Directory.Exists(directory))
                {
                    string[] logFiles = Directory.GetFiles(directory, "Usuarios_*.log");
                    DateTime cutoffDate = DateTime.Now.AddDays(-30);

                    foreach (string file in logFiles)
                    {
                        if (File.GetCreationTime(file) < cutoffDate)
                        {
                            File.Delete(file);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Fallo en limpieza no es crítico
            }
        }
    }
}