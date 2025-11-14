using System;
using System.Windows.Forms;

namespace SistemaPresupuestario.Helpers
{
    /// <summary>
    /// Servicio centralizado para traducir automáticamente todos los controles de un formulario
    /// Recorre recursivamente todos los controles y traduce sus propiedades Text usando I18n
    /// 
    /// TRADUCCIÓN DINÁMICA:
    /// - En la primera ejecución, guarda la clave original en control.Tag
    /// - En ejecuciones subsiguientes, lee la clave desde control.Tag
    /// - Esto permite re-ejecutar el traductor cuando cambia el idioma
    /// </summary>
    public static class FormTranslator
    {
        /// <summary>
        /// Traduce automáticamente todos los controles de un formulario
        /// Debe ser llamado en el constructor del formulario DESPUÉS de InitializeComponent()
        /// También se puede llamar cuando cambia el idioma para re-traducir
        /// </summary>
        /// <param name="form">El formulario a traducir</param>
        public static void Translate(Form form)
        {
            try
            {
                // Traducir el título del formulario
                if (!string.IsNullOrWhiteSpace(form.Text))
                {
                    // Primera vez: guardar clave original en Tag
                    if (form.Tag == null || !(form.Tag is string))
                    {
                        form.Tag = form.Text;
                    }
                    
                    // Traducir usando la clave guardada
                    form.Text = I18n.T(form.Tag.ToString());
                }

                // Traducir todos los controles recursivamente
                TranslateControls(form.Controls);
            }
            catch (Exception ex)
            {
                // No bloquear la carga del formulario si hay error en traducción
                System.Diagnostics.Debug.WriteLine($"Error al traducir formulario {form.Name}: {ex.Message}");
            }
        }

        /// <summary>
        /// Traduce recursivamente una colección de controles
        /// </summary>
        private static void TranslateControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                try
                {
                    // Traducir el texto del control si no está vacío
                    if (!string.IsNullOrWhiteSpace(control.Text))
                    {
                        // Primera vez: guardar clave original en Tag (si Tag no está siendo usado)
                        if (control.Tag == null || !(control.Tag is string))
                        {
                            control.Tag = control.Text;
                        }
                        
                        // Traducir usando la clave guardada
                        control.Text = I18n.T(control.Tag.ToString());
                    }

                    // Casos especiales para diferentes tipos de controles

                    // DataGridView: traducir encabezados de columnas
                    if (control is DataGridView dgv)
                    {
                        TranslateDataGridView(dgv);
                    }
                    // ComboBox: traducir items si son strings
                    else if (control is ComboBox cmb)
                    {
                        TranslateComboBox(cmb);
                    }
                    // ListBox: traducir items si son strings
                    else if (control is ListBox lst)
                    {
                        TranslateListBox(lst);
                    }
                    // MenuStrip: traducir items de menú
                    else if (control is MenuStrip menu)
                    {
                        TranslateMenuStrip(menu);
                    }
                    // ToolStrip: traducir items de toolbar
                    else if (control is ToolStrip toolbar)
                    {
                        TranslateToolStrip(toolbar);
                    }
                    // StatusStrip: traducir items de barra de estado
                    else if (control is StatusStrip status)
                    {
                        TranslateStatusStrip(status);
                    }
                    // TabControl: traducir pestañas
                    else if (control is TabControl tab)
                    {
                        TranslateTabControl(tab);
                    }

                    // Recursión: traducir controles hijos si existen
                    if (control.HasChildren)
                    {
                        TranslateControls(control.Controls);
                    }
                }
                catch (Exception ex)
                {
                    // Continuar con el siguiente control si hay error
                    System.Diagnostics.Debug.WriteLine($"Error al traducir control {control.Name}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Traduce los encabezados de las columnas de un DataGridView
        /// </summary>
        private static void TranslateDataGridView(DataGridView dgv)
        {
            foreach (DataGridViewColumn column in dgv.Columns)
            {
                if (!string.IsNullOrWhiteSpace(column.HeaderText))
                {
                    // Primera vez: guardar clave original en Tag
                    if (column.Tag == null || !(column.Tag is string))
                    {
                        column.Tag = column.HeaderText;
                    }
                    
                    // Traducir usando la clave guardada
                    column.HeaderText = I18n.T(column.Tag.ToString());
                }
            }
        }

        /// <summary>
        /// Traduce los items de un ComboBox (solo si son strings)
        /// </summary>
        private static void TranslateComboBox(ComboBox cmb)
        {
            // Solo traducir si los items son strings simples
            // Si son objetos con DisplayMember, no traducir automáticamente
            if (cmb.Items.Count > 0 && cmb.Items[0] is string)
            {
                // Guardar estado actual
                int selectedIndex = cmb.SelectedIndex;
                
                // Guardar claves originales si no existen (usando una estructura especial en Tag)
                if (cmb.Tag == null || !(cmb.Tag is string[]))
                {
                    string[] originalKeys = new string[cmb.Items.Count];
                    for (int i = 0; i < cmb.Items.Count; i++)
                    {
                        originalKeys[i] = cmb.Items[i].ToString();
                    }
                    cmb.Tag = originalKeys;
                }
                
                // Traducir items usando las claves guardadas
                string[] keys = (string[])cmb.Tag;
                cmb.Items.Clear();
                
                foreach (string key in keys)
                {
                    cmb.Items.Add(I18n.T(key));
                }

                cmb.SelectedIndex = selectedIndex;
            }
        }

        /// <summary>
        /// Traduce los items de un ListBox (solo si son strings)
        /// </summary>
        private static void TranslateListBox(ListBox lst)
        {
            // Solo traducir si los items son strings simples
            if (lst.Items.Count > 0 && lst.Items[0] is string)
            {
                // Guardar estado actual
                int selectedIndex = lst.SelectedIndex;
                
                // Guardar claves originales si no existen
                if (lst.Tag == null || !(lst.Tag is string[]))
                {
                    string[] originalKeys = new string[lst.Items.Count];
                    for (int i = 0; i < lst.Items.Count; i++)
                    {
                        originalKeys[i] = lst.Items[i].ToString();
                    }
                    lst.Tag = originalKeys;
                }
                
                // Traducir items usando las claves guardadas
                string[] keys = (string[])lst.Tag;
                lst.Items.Clear();

                foreach (string key in keys)
                {
                    lst.Items.Add(I18n.T(key));
                }

                lst.SelectedIndex = selectedIndex;
            }
        }

        /// <summary>
        /// Traduce los items de un MenuStrip
        /// </summary>
        private static void TranslateMenuStrip(MenuStrip menu)
        {
            foreach (ToolStripItem item in menu.Items)
            {
                TranslateToolStripItem(item);
            }
        }

        /// <summary>
        /// Traduce los items de un ToolStrip
        /// </summary>
        private static void TranslateToolStrip(ToolStrip toolbar)
        {
            foreach (ToolStripItem item in toolbar.Items)
            {
                TranslateToolStripItem(item);
            }
        }

        /// <summary>
        /// Traduce los items de un StatusStrip
        /// </summary>
        private static void TranslateStatusStrip(StatusStrip status)
        {
            foreach (ToolStripItem item in status.Items)
            {
                TranslateToolStripItem(item);
            }
        }

        /// <summary>
        /// Traduce recursivamente un ToolStripItem y sus subitems
        /// </summary>
        private static void TranslateToolStripItem(ToolStripItem item)
        {
            if (!string.IsNullOrWhiteSpace(item.Text))
            {
                // Primera vez: guardar clave original en Tag
                if (item.Tag == null || !(item.Tag is string))
                {
                    item.Tag = item.Text;
                }
                
                // Traducir usando la clave guardada
                item.Text = I18n.T(item.Tag.ToString());
            }

            // Si es un ToolStripMenuItem, traducir sus subitems
            if (item is ToolStripMenuItem menuItem)
            {
                foreach (ToolStripItem subItem in menuItem.DropDownItems)
                {
                    TranslateToolStripItem(subItem);
                }
            }
        }

        /// <summary>
        /// Traduce las pestañas de un TabControl
        /// </summary>
        private static void TranslateTabControl(TabControl tab)
        {
            foreach (TabPage page in tab.TabPages)
            {
                if (!string.IsNullOrWhiteSpace(page.Text))
                {
                    // Primera vez: guardar clave original en Tag
                    if (page.Tag == null || !(page.Tag is string))
                    {
                        page.Tag = page.Text;
                    }
                    
                    // Traducir usando la clave guardada
                    page.Text = I18n.T(page.Tag.ToString());
                }

                // Traducir controles dentro de la pestaña
                TranslateControls(page.Controls);
            }
        }
    }
}
