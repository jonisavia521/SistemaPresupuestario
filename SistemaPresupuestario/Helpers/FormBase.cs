using System;
using System.Windows.Forms;

namespace SistemaPresupuestario.Helpers
{
    /// <summary>
    /// Clase base para todos los formularios de la aplicación
    /// Proporciona funcionalidad automática de traducción dinámica
    /// 
    /// CÓMO USAR:
    /// 1. Hacer que tu formulario herede de FormBase en lugar de Form
    /// 2. En el constructor, después de InitializeComponent(), llamar a base.InitializeTranslation()
    /// 3. El formulario se traducirá automáticamente cuando cambie el idioma
    /// 
    /// EJEMPLO:
    /// public partial class frmMiFormulario : FormBase
    /// {
    ///     public frmMiFormulario()
    ///     {
    ///         InitializeComponent();
    ///         base.InitializeTranslation();
    ///     }
    /// }
    /// </summary>
    public class FormBase : Form
    {
        /// <summary>
        /// Indica si la traducción ya fue inicializada para este formulario
        /// </summary>
        private bool _translationInitialized = false;

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public FormBase()
        {
            // Suscribirse al evento de cierre para limpiar recursos
            this.FormClosed += FormBase_FormClosed;
        }

        /// <summary>
        /// Inicializa la traducción del formulario y se suscribe a cambios de idioma
        /// Debe ser llamado en el constructor del formulario derivado DESPUÉS de InitializeComponent()
        /// </summary>
        protected void InitializeTranslation()
        {
            if (_translationInitialized)
                return;

            // Traducir el formulario por primera vez
            FormTranslator.Translate(this);

            // Suscribirse al evento de cambio de idioma
            I18n.LanguageChanged += OnLanguageChanged;

            _translationInitialized = true;
        }

        /// <summary>
        /// Manejador del evento de cambio de idioma
        /// Re-ejecuta el traductor para actualizar todos los controles
        /// </summary>
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            try
            {
                // Re-traducir todo el formulario
                FormTranslator.Translate(this);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al re-traducir formulario {this.Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Manejador del evento FormClosed
        /// Des-suscribe del evento de cambio de idioma para evitar fugas de memoria
        /// </summary>
        private void FormBase_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Des-suscribirse del evento estático para evitar fugas de memoria
            I18n.LanguageChanged -= OnLanguageChanged;
        }
    }
}
