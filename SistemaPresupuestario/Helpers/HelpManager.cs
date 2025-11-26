using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SistemaPresupuestario.Helpers
{
    /// <summary>
    /// Gestor centralizado de ayuda (F1) que abre el CHM en la página correspondiente
    /// según el formulario actual
    /// </summary>
    public static class HelpManager
    {
        private static string _chmFilePath;
        private static readonly Dictionary<string, string> _formToHelpTopic = new Dictionary<string, string>
        {
            // Mapeo de nombres de tipos de formularios a temas de ayuda (archivo HTML del CHM)
            // ============================================================================
            // FORMULARIOS PRINCIPALES DE SEGURIDAD
            { "frmLogin", "03_configuracion.html" },
            
            // FORMULARIO PRINCIPAL
            { "frmMain", "index.html" },
            
            // MAESTROS: USUARIOS Y SEGURIDAD
            { "frmUsuarios", "03_usuarios_seguridad.html" },
            { "frmAlta", "03_usuarios_seguridad.html" },
            
            // MAESTROS: CLIENTES
            { "frmClientes", "04_maestro_clientes.html" },
            { "frmClienteAlta", "04_maestro_clientes.html" },
            
            // MAESTROS: VENDEDORES
            { "frmVendedores", "05_maestro_vendedores.html" },
            { "frmVendedorAlta", "05_maestro_vendedores.html" },
            
            // MAESTROS: PRODUCTOS
            { "frmProductos", "06_maestro_productos.html" },
            { "frmProductoAlta", "06_maestro_productos.html" },
            
            // MAESTROS: LISTAS DE PRECIOS
            { "frmListaPrecios", "07_lista_precios.html" },
            { "frmListaPrecioAlta", "07_lista_precios.html" },
            
            // PRESUPUESTOS / COTIZACIONES
            { "frmPresupuesto", "08_presupuestos.html" },
            
            // FACTURACIÓN
            { "frmFacturar", "09_facturacion.html" },
            
            // CONFIGURACIÓN DEL SISTEMA
            { "frmConfiguacionGeneral", "03_configuracion.html" },
            
            // IMPUESTOS Y ARBA
            { "frmActualizarPadronArba", "11_padron_arba.html" },
            
            // DEMOSTRACIÓN ACADÉMICA: DÍGITOS VERIFICADORES
            { "frmDemoVerificadorProductos", "17_digitosVerificadores.html" },
            
            // FORMULARIOS AUXILIARES
            { "frmSelector", "02_conceptos_generales.html" },
        };

        /// <summary>
        /// Inicializa el HelpManager con la ruta del archivo CHM
        /// Debe ser llamado una vez en Program.Main()
        /// </summary>
        public static void Initialize()
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                _chmFilePath = Path.Combine(basePath, "SistemaPresupuestario_Ayuda.chm");

                if (!File.Exists(_chmFilePath))
                {
                    System.Diagnostics.Debug.WriteLine($"[HelpManager] Advertencia: Archivo CHM no encontrado en: {_chmFilePath}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[HelpManager] CHM inicializado correctamente: {_chmFilePath}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[HelpManager] Error al inicializar: {ex.Message}");
            }
        }

    /// <summary>
    /// Abre la ayuda para el formulario especificado
    /// </summary>
    /// <param name="form">El formulario para el cual se desea mostrar ayuda</param>
    public static void ShowHelp(Form form)
    {
        try
        {
            if (form == null)
                return;

            if (string.IsNullOrEmpty(_chmFilePath) || !File.Exists(_chmFilePath))
            {
                MessageBox.Show("El archivo de ayuda no está disponible.", "Ayuda",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Type formType = form.GetType();
            string topic = GetHelpTopic(formType, form);

            // Abre el CHM con el tema específico
            OpenCHMWithTopic(topic);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HelpManager] Error al mostrar ayuda: {ex.Message}");
            MessageBox.Show("No se pudo abrir el archivo de ayuda.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

        /// <summary>
        /// Obtiene el tema de ayuda asociado al tipo de formulario
        /// Detecta si el formulario tiene TabControl con pestaña activa y mapea dinámicamente
        /// </summary>
        private static string GetHelpTopic(Type formType, Form form = null)
        {
            string formName = formType.Name;
            
            // CASO ESPECIAL: frmConfiguacionGeneral con TabControl
            if (formName == "frmConfiguacionGeneral" && form != null)
            {
                try
                {
                    // Buscar el TabControl en el formulario
                    var tabControl = form.Controls.Find("tabControl1", true).FirstOrDefault() as TabControl;
                    
                    if (tabControl != null && tabControl.SelectedTab != null)
                    {
                        string selectedTabName = tabControl.SelectedTab.Name;
                        
                        // Mapear según la pestaña activa
                        switch (selectedTabName)
                        {
                            case "tabConfiguracionGeneral":
                                return "03_configuracion.html";
                            case "tabIdioma":
                                return "14_idiomas.html";
                            case "tabBackup":
                                return "15_backup.html";
                            default:
                                return "03_configuracion.html";
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[HelpManager] Error al detectar pestaña activa: {ex.Message}");
                }
            }
            
            // CASO ESTÁNDAR: Usar el mapeo predefinido
            if (_formToHelpTopic.ContainsKey(formName))
            {
                return _formToHelpTopic[formName];
            }

            // Por defecto, abre el índice
            System.Diagnostics.Debug.WriteLine($"[HelpManager] Tipo de formulario no mapeado: {formType.Name}, usando índice");
            return "index.html";
        }

        /// <summary>
        /// Obtiene el tema de ayuda asociado al tipo de formulario (versión sobrecargada sin form)
        /// </summary>
        private static string GetHelpTopic(Type formType)
        {
            return GetHelpTopic(formType, null);
        }

        /// <summary>
        /// Abre el archivo CHM con el tema especificado
        /// </summary>
        private static void OpenCHMWithTopic(string topic)
        {
            try
            {
                // Usar Help.ShowHelp para que el CHM se abra en la página correcta
                // Nota: ShowHelp() requiere que se especifique la página sin "ms-its://"
                Help.ShowHelp(null, _chmFilePath, HelpNavigator.Topic, topic);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[HelpManager] Error al abrir CHM con Help.ShowHelp: {ex.Message}");
                
                // Si Help.ShowHelp falla, intentar con hh.exe directamente
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "hh.exe",
                        Arguments = $"\"ms-its:{_chmFilePath}::{topic}\"",
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                catch (Exception ex2)
                {
                    System.Diagnostics.Debug.WriteLine($"[HelpManager] Error al abrir con hh.exe: {ex2.Message}");
                    
                    // Como última opción, abrir directamente el CHM
                    Process.Start(_chmFilePath);
                }
            }
        }

        /// <summary>
        /// Registra un formulario personalizado con su tema de ayuda
        /// Útil para formularios que no están en el mapeo predeterminado
        /// </summary>
        public static void RegisterFormHelpTopic(Type formType, string helpTopic)
        {
            if (formType != null && !string.IsNullOrEmpty(helpTopic))
            {
                _formToHelpTopic[formType.Name] = helpTopic;
                System.Diagnostics.Debug.WriteLine($"[HelpManager] Formulario registrado: {formType.Name} -> {helpTopic}");
            }
        }
    }
}
