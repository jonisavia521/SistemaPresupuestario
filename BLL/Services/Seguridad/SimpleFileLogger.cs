using BLL.Contracts.Seguridad;
using System;
using System.Diagnostics;
using System.IO;

namespace BLL.Services.Seguridad
{
    /// <summary>
    /// Implementación simple de logger que escribe a archivo y Debug
    /// </summary>
    public class SimpleFileLogger : ILogger
    {
        private readonly string _logPath;
        private readonly object _lock = new object();

        public SimpleFileLogger(string logPath = null)
        {
            _logPath = logPath ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SistemaPresupuestario", "security.log");
            EnsureLogDirectory();
        }

        public void LogInfo(string message)
        {
            WriteLog("INFO", message);
        }

        public void LogWarning(string message)
        {
            WriteLog("WARN", message);
        }

        public void LogError(string message, Exception exception = null)
        {
            var fullMessage = exception != null ? $"{message} - Exception: {exception}" : message;
            WriteLog("ERROR", fullMessage);
        }

        public void LogDebug(string message)
        {
            WriteLog("DEBUG", message);
            Debug.WriteLine($"[SECURITY] {message}");
        }

        private void WriteLog(string level, string message)
        {
            lock (_lock)
            {
                try
                {
                    var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    var logEntry = $"[{timestamp}] [{level}] {message}";
                    
                    File.AppendAllText(_logPath, logEntry + Environment.NewLine);
                    Debug.WriteLine($"[{level}] {message}");
                }
                catch
                {
                    // Si no se puede escribir al archivo, solo escribir a Debug
                    Debug.WriteLine($"[{level}] {message}");
                }
            }
        }

        private void EnsureLogDirectory()
        {
            try
            {
                var directory = Path.GetDirectoryName(_logPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch
            {
                // Ignorar errores de creación de directorio
            }
        }
    }
}