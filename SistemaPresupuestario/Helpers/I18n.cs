using Services.Services;
using System;

namespace SistemaPresupuestario.Helpers
{
    /// <summary>
    /// Clase helper para facilitar el uso de traducciones en los formularios
    /// </summary>
    public static class I18n
    {
        /// <summary>
        /// Evento que se dispara cuando el idioma cambia
        /// Los formularios deben suscribirse a este evento para retraducirse automáticamente
        /// </summary>
        public static event EventHandler LanguageChanged;

        /// <summary>
        /// Traduce un texto según el idioma actual configurado
        /// </summary>
        /// <param name="key">Clave del texto a traducir</param>
        /// <returns>Texto traducido</returns>
        public static string T(string key)
        {
            return TranslationService.Translate(key);
        }
        
        /// <summary>
        /// Cambia el idioma de la aplicación y notifica a todos los formularios suscritos
        /// </summary>
        /// <param name="culture">Código de cultura (ej: "es-AR", "en-US")</param>
        public static void SetLanguage(string culture)
        {
            // Cambiar el idioma en el servicio subyacente
            TranslationService.ChangeLanguage(culture);
            
            // Notificar a todos los formularios suscritos
            LanguageChanged?.Invoke(null, EventArgs.Empty);
        }
        
        /// <summary>
        /// Obtiene el idioma actual
        /// </summary>
        public static string GetCurrentLanguage()
        {
            return TranslationService.GetCurrentLanguage();
        }
        
        /// <summary>
        /// Verifica si el idioma actual es español
        /// </summary>
        public static bool IsSpanish()
        {
            return GetCurrentLanguage().StartsWith("es");
        }
        
        /// <summary>
        /// Verifica si el idioma actual es inglés
        /// </summary>
        public static bool IsEnglish()
        {
            return GetCurrentLanguage().StartsWith("en");
        }
    }
}
