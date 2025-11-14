using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Services.Services
{
    /// <summary>
    /// Servicio de traducción dinámica basado en archivo de textos
    /// </summary>
    public class TranslationService
    {
        private static Dictionary<string, TranslationPair> _translations;
        private static string _currentLanguage = "es-AR"; // Idioma por defecto
        private static readonly object _lockObject = new object();

        /// <summary>
        /// Par de traducciones (español|inglés)
        /// </summary>
        private class TranslationPair
        {
            public string Spanish { get; set; }
            public string English { get; set; }
        }

        /// <summary>
        /// Inicializa el servicio de traducción cargando el archivo
        /// </summary>
        /// <param name="languageCode">Código de idioma (es-AR, en-US)</param>
        /// <param name="translationFilePath">Ruta al archivo de traducciones (opcional)</param>
        public static void Initialize(string languageCode = "es-AR", string translationFilePath = null)
        {
            lock (_lockObject)
            {
                _currentLanguage = languageCode ?? "es-AR";
                
                // DEBUG: Log del idioma que se está configurando
                System.Diagnostics.Debug.WriteLine($"[TranslationService] Inicializando con idioma: {_currentLanguage}");
                
                // Si no se proporciona ruta, usar la ruta por defecto
                if (string.IsNullOrEmpty(translationFilePath))
                {
                    // Ruta relativa al ejecutable
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    translationFilePath = Path.Combine(basePath, "Textos_Controles_UI.txt");
                }

                System.Diagnostics.Debug.WriteLine($"[TranslationService] Ruta del archivo: {translationFilePath}");
                System.Diagnostics.Debug.WriteLine($"[TranslationService] ¿Archivo existe?: {File.Exists(translationFilePath)}");

                LoadTranslations(translationFilePath);
                
                System.Diagnostics.Debug.WriteLine($"[TranslationService] Traducciones cargadas: {_translations?.Count ?? 0}");
                
                // Establecer la cultura del thread actual
                SetCulture(_currentLanguage);
            }
        }

        /// <summary>
        /// Carga las traducciones desde el archivo
        /// </summary>
        private static void LoadTranslations(string filePath)
        {
            // ? CORRECCIÓN: Usar un comparador que normaliza acentos y mayúsculas
            _translations = new Dictionary<string, TranslationPair>(new AccentInsensitiveComparer());

            if (!File.Exists(filePath))
            {
                // Si el archivo no existe, continuar con diccionario vacío
                // Las traducciones devolverán la clave original
                System.Diagnostics.Debug.WriteLine($"[TranslationService] ADVERTENCIA: Archivo de traducciones no encontrado: {filePath}");
                return;
            }

            try
            {
                var lines = File.ReadAllLines(filePath);
                
                System.Diagnostics.Debug.WriteLine($"[TranslationService] Leyendo {lines.Length} líneas del archivo");
                
                foreach (var line in lines)
                {
                    // Ignorar líneas vacías o comentarios
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue;

                    // Formato: clave=español|inglés
                    var parts = line.Split('=');
                    if (parts.Length != 2)
                        continue;

                    string key = parts[0].Trim();
                    string values = parts[1];

                    // Separar español e inglés
                    var translations = values.Split('|');
                    if (translations.Length < 2)
                        continue;

                    _translations[key] = new TranslationPair
                    {
                        Spanish = translations[0].Trim(),
                        English = translations[1].Trim()
                    };
                }
                
                System.Diagnostics.Debug.WriteLine($"[TranslationService] Total de traducciones cargadas: {_translations.Count}");
            }
            catch (Exception ex)
            {
                // En caso de error, continuar con diccionario vacío
                System.Diagnostics.Debug.WriteLine($"[TranslationService] ERROR al cargar traducciones: {ex.Message}");
            }
        }

        /// <summary>
        /// Traduce un texto según el idioma actual
        /// </summary>
        /// <param name="key">Clave del texto a traducir</param>
        /// <returns>Texto traducido o la clave si no se encuentra traducción</returns>
        public static string Translate(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return key;

            // Si no hay traducciones cargadas, devolver la clave
            if (_translations == null || _translations.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine($"[TranslationService] ADVERTENCIA: No hay traducciones cargadas para '{key}'");
                return key;
            }

            // Buscar traducción
            if (_translations.TryGetValue(key, out var pair))
            {
                string resultado;
                // Devolver según el idioma actual
                if (_currentLanguage.StartsWith("en", StringComparison.OrdinalIgnoreCase))
                {
                    resultado = pair.English;
                }
                else
                {
                    resultado = pair.Spanish;
                }
                
                // DEBUG: Log solo para claves específicas para no saturar
                if (key == "Login" || key == "Ingresar")
                {
                    System.Diagnostics.Debug.WriteLine($"[TranslationService] Traduciendo '{key}' -> '{resultado}' (idioma: {_currentLanguage})");
                }
                
                return resultado;
            }

            // Si no se encuentra, devolver la clave original
            System.Diagnostics.Debug.WriteLine($"[TranslationService] Traducción no encontrada para: '{key}'");
            return key;
        }

        /// <summary>
        /// Cambia el idioma actual
        /// </summary>
        /// <param name="languageCode">Código de idioma (es-AR, en-US)</param>
        public static void ChangeLanguage(string languageCode)
        {
            lock (_lockObject)
            {
                _currentLanguage = languageCode ?? "es-AR";
                System.Diagnostics.Debug.WriteLine($"[TranslationService] Cambiando idioma a: {_currentLanguage}");
                SetCulture(_currentLanguage);
            }
        }

        /// <summary>
        /// Obtiene el idioma currente
        /// </summary>
        public static string GetCurrentLanguage()
        {
            return _currentLanguage;
        }

        /// <summary>
        /// Establece la cultura del thread actual
        /// </summary>
        private static void SetCulture(string languageCode)
        {
            try
            {
                var culture = new CultureInfo(languageCode);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                System.Diagnostics.Debug.WriteLine($"[TranslationService] Cultura establecida: {culture.Name}");
            }
            catch (Exception ex)
            {
                // Si hay error, usar cultura por defecto
                System.Diagnostics.Debug.WriteLine($"[TranslationService] Error al establecer cultura: {ex.Message}");
            }
        }

        /// <summary>
        /// ? NUEVO: Comparador personalizado que ignora acentos y mayúsculas
        /// Normaliza las cadenas usando Unicode NFD (Normalization Form Canonical Decomposition)
        /// para separar los caracteres base de los diacríticos (acentos)
        /// </summary>
        private class AccentInsensitiveComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;

                return string.Equals(
                    RemoveAccents(x),
                    RemoveAccents(y),
                    StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(string obj)
            {
                if (obj == null) return 0;
                return RemoveAccents(obj).ToUpperInvariant().GetHashCode();
            }

            /// <summary>
            /// Elimina los acentos de una cadena normalizándola
            /// Ejemplo: "Configuración" -> "Configuracion"
            /// </summary>
            private static string RemoveAccents(string text)
            {
                if (string.IsNullOrWhiteSpace(text))
                    return text;

                // Normalizar a FormD (descompone caracteres acentuados en base + acento)
                var normalizedString = text.Normalize(NormalizationForm.FormD);
                var stringBuilder = new StringBuilder();

                // Filtrar solo los caracteres que NO son marcas diacríticas
                foreach (var c in normalizedString)
                {
                    var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                    if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    {
                        stringBuilder.Append(c);
                    }
                }

                // Renormalizar a FormC (forma compuesta estándar)
                return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
            }
        }
    }
}
