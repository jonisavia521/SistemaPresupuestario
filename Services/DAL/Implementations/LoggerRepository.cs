using Services.DAL.Implementations.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Services.DAL.Implementations
{
    public sealed class LoggerRepository:ILoggerRepository
    {

        private string pathLog;

        private string pathFile;
        public LoggerRepository(NameValueCollection app)
        {
            // ✅ Obtener la ruta donde se está ejecutando el .exe
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var carpeta = app["PathLog"] ?? "Logs";
            // ✅ Combinar con la carpeta "Logs"
            pathLog = Path.Combine(exePath, carpeta);

            // ✅ Crear la carpeta si no existe
            if (!Directory.Exists(pathLog))
            {
                Directory.CreateDirectory(pathLog);
            }

            // ✅ Obtener solo el nombre del archivo desde configuración
            pathFile = app["LogFileName"] ?? "info.log";

            // ✅ Asegurar que pathLog termine con separador de directorios
            if (!pathLog.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                pathLog += Path.DirectorySeparatorChar;
            }
        }

        /// <summary>
        /// Escribe un mensaje de log en el archivo correspondiente
        /// El archivo se crea con formato: YYYYMMDD + nombre_archivo
        /// Ejemplo: C:\MiApp\bin\Debug\Logs\20251002info.log
        /// </summary>
        public void WriteLog(string message, EventLevel level, string user)
        {
            try
            {
                // Construir ruta completa: pathLog + fecha + nombre_archivo
                // Ejemplo: "C:\MiApp\Logs\" + "20251002" + "info.log"
                string fileName = pathLog + DateTime.Now.ToString("yyyyMMdd") + pathFile;

                using (StreamWriter streamWriter = new StreamWriter(fileName, true))
                {
                    string formattedMessage = $"{DateTime.Now.ToString("yyyyMMdd hh:mm:ss tt")} [LEVEL {level.ToString()}] User: {user}, Mensaje: {message}";
                    streamWriter.WriteLine(formattedMessage);
                }
            }
            catch (Exception ex) { 
                System.Diagnostics.EventLog.WriteEntry("Application",
                    $"Error al escribir log: {ex.Message}\nMensaje original: {message}",
                    System.Diagnostics.EventLogEntryType.Error);
            }
        }
    }
}
