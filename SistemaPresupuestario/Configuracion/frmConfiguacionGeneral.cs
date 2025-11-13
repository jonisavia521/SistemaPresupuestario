using Services.BLL;
using Services.BLL.Contracts;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SistemaPresupuestario.Configuracion
{
    /// <summary>
    /// Formulario para gestionar configuración general del sistema, idioma y Backup/Restore
    /// Dividido en 3 pestañas para mejor organización
    /// </summary>
    public partial class frmConfiguacionGeneral : Form
    {
        private readonly IBackupRestoreService _servicio;

        public frmConfiguacionGeneral()
        {
            InitializeComponent();
            _servicio = new BackupRestoreService();
        }

        /// <summary>
        /// Carga inicial del formulario
        /// </summary>
        private void frmConfiguacionGeneral_Load(object sender, EventArgs e)
        {
            try
            {
                // Cargar configuración general
                CargarConfiguracionEmpresa();
                
                // Cargar configuración de idioma
                CargarConfiguracionIdioma();
                
                // Cargar historial de backups
                ActualizarGrilla();
                
                lblEstado.Text = "Listo";
                lblEstado.Visible = false;
                progressBar.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el formulario: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Configuración General de la Empresa

        /// <summary>
        /// Carga los datos de configuración de la empresa desde el archivo de configuración
        /// </summary>
        private void CargarConfiguracionEmpresa()
        {
            try
            {
                var config = ConfigurationManager.AppSettings;
                
                txtRazonSocial.Text = config["EmpresaRazonSocial"] ?? "";
                txtCUIT.Text = config["EmpresaCUIT"] ?? "";
                txtProvincia.Text = config["EmpresaProvincia"] ?? "";
                txtLocalidad.Text = config["EmpresaLocalidad"] ?? "";
                txtDireccion.Text = config["EmpresaDireccion"] ?? "";
                txtEmail.Text = config["EmpresaEmail"] ?? "";
                txtTelefono.Text = config["EmpresaTelefono"] ?? "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar configuración de la empresa: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Guarda la configuración de la empresa en el archivo de configuración
        /// </summary>
        private void btnGuardarConfiguracion_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar datos requeridos
                if (string.IsNullOrWhiteSpace(txtRazonSocial.Text))
                {
                    MessageBox.Show("La Razón Social es obligatoria", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtRazonSocial.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtCUIT.Text))
                {
                    MessageBox.Show("El CUIT es obligatorio", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCUIT.Focus();
                    return;
                }

                // Validar formato CUIT (XX-XXXXXXXX-X)
                if (txtCUIT.Text.Replace("-", "").Length != 11)
                {
                    MessageBox.Show("El CUIT debe tener 11 dígitos (formato: XX-XXXXXXXX-X)", 
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCUIT.Focus();
                    return;
                }

                // Guardar en App.config
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;

                ActualizarOAgregarSetting(settings, "EmpresaRazonSocial", txtRazonSocial.Text.Trim());
                ActualizarOAgregarSetting(settings, "EmpresaCUIT", txtCUIT.Text.Trim());
                ActualizarOAgregarSetting(settings, "EmpresaProvincia", txtProvincia.Text.Trim());
                ActualizarOAgregarSetting(settings, "EmpresaLocalidad", txtLocalidad.Text.Trim());
                ActualizarOAgregarSetting(settings, "EmpresaDireccion", txtDireccion.Text.Trim());
                ActualizarOAgregarSetting(settings, "EmpresaEmail", txtEmail.Text.Trim());
                ActualizarOAgregarSetting(settings, "EmpresaTelefono", txtTelefono.Text.Trim());

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                MessageBox.Show("Configuración guardada exitosamente", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar configuración: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Helper para actualizar o agregar un setting en el archivo de configuración
        /// </summary>
        private void ActualizarOAgregarSetting(KeyValueConfigurationCollection settings, string key, string value)
        {
            if (settings[key] == null)
            {
                settings.Add(key, value);
            }
            else
            {
                settings[key].Value = value;
            }
        }

        #endregion

        #region Configuración de Idioma

        /// <summary>
        /// Carga la configuración actual de idioma
        /// </summary>
        private void CargarConfiguracionIdioma()
        {
            try
            {
                var idiomaActual = Thread.CurrentThread.CurrentUICulture.Name;
                
                if (idiomaActual.StartsWith("es"))
                {
                    rbEspanol.Checked = true;
                    lblIdiomaActual.Text = "Español";
                }
                else if (idiomaActual.StartsWith("en"))
                {
                    rbIngles.Checked = true;
                    lblIdiomaActual.Text = "English";
                }
                else
                {
                    rbEspanol.Checked = true;
                    lblIdiomaActual.Text = "Español (por defecto)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar configuración de idioma: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Cambia el idioma de la aplicación y lo guarda en la configuración
        /// </summary>
        private void btnCambiarIdioma_Click(object sender, EventArgs e)
        {
            try
            {
                string nuevoIdioma = "";
                string nombreIdioma = "";

                if (rbEspanol.Checked)
                {
                    nuevoIdioma = "es-AR";
                    nombreIdioma = "Español";
                }
                else if (rbIngles.Checked)
                {
                    nuevoIdioma = "en-US";
                    nombreIdioma = "English";
                }

                // Guardar en App.config
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;

                ActualizarOAgregarSetting(settings, "IdiomaActual", nuevoIdioma);

                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                // Actualizar el label
                lblIdiomaActual.Text = nombreIdioma;

                MessageBox.Show(
                    $"Idioma cambiado a: {nombreIdioma}\n\n" +
                    "La aplicación se reiniciará para aplicar los cambios.",
                    "Cambio de Idioma",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Reiniciar la aplicación
                Application.Restart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cambiar idioma: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Backup y Restore

        /// <summary>
        /// Evento para crear un backup
        /// </summary>
        private void btnCrearBackup_Click(object sender, EventArgs e)
        {
            // Configurar SaveFileDialog
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Archivos de Backup (*.bak)|*.bak|Todos los archivos (*.*)|*.*";
                saveFileDialog.Title = "Guardar Backup";
                saveFileDialog.DefaultExt = "bak";
                saveFileDialog.FileName = $"SistemaPresupuestario_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string rutaArchivo = saveFileDialog.FileName;

                    SetTrabajando(true, "Creando backup... Por favor espere.");

                    try
                    {
                        // Obtener usuario actual
                        string usuarioActual = Environment.UserName;

                        // Llamar al servicio de forma síncrona
                        _servicio.CrearBackup(rutaArchivo, usuarioActual);

                        MessageBox.Show($"Backup creado exitosamente en:\n{rutaArchivo}", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al crear el backup:\n{ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        SetTrabajando(false);
                        ActualizarGrilla();
                    }
                }
            }
        }

        /// <summary>
        /// Evento para restaurar un backup
        /// </summary>
        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            // Verificar que haya una fila seleccionada
            if (dgvHistorial.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Por favor, seleccione un backup del historial para restaurar.",
                    "Advertencia",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Obtener la ruta del backup seleccionado
            var filaSeleccionada = dgvHistorial.SelectedRows[0];
            string rutaArchivo = filaSeleccionada.Cells["RutaArchivo"].Value?.ToString();

            if (string.IsNullOrEmpty(rutaArchivo))
            {
                MessageBox.Show(
                    "No se pudo obtener la ruta del archivo de backup.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Verificar que el archivo existe
            if (!File.Exists(rutaArchivo))
            {
                MessageBox.Show(
                    $"El archivo de backup no existe en la ruta:\n{rutaArchivo}\n\n" +
                    "Es posible que haya sido movido o eliminado.",
                    "Archivo no encontrado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Mostrar advertencia crítica
            var result = MessageBox.Show(
                "⚠️ ADVERTENCIA CRÍTICA ⚠️\n\n" +
                "Esta operación reemplazará TODOS los datos actuales de la base de datos.\n\n" +
                "Se perderán todos los cambios realizados desde el backup seleccionado.\n\n" +
                $"Archivo a restaurar:\n{rutaArchivo}\n\n" +
                "La aplicación se reiniciará automáticamente después de la restauración.\n\n" +
                "¿Está SEGURO de que desea continuar?",
                "Confirmar Restauración",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
                return;

            SetTrabajando(true, "Restaurando backup... Por favor espere. NO CIERRE LA APLICACIÓN.");

            try
            {
                // Llamar al servicio de forma síncrona
                _servicio.RestaurarBackup(rutaArchivo);

                MessageBox.Show(
                    "Restauración completada exitosamente.\n\n" +
                    "La aplicación se reiniciará ahora.",
                    "Restauración Exitosa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Reiniciar la aplicación
                Application.Restart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al restaurar el backup:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetTrabajando(false);
            }
        }

        /// <summary>
        /// Actualiza la grilla con el historial de backups
        /// </summary>
        private void ActualizarGrilla()
        {
            try
            {
                dgvHistorial.DataSource = _servicio.ObtenerHistorial();

                // Configurar apariencia de la grilla
                if (dgvHistorial.Columns.Count > 0)
                {
                    dgvHistorial.Columns["ID"].Width = 50;
                    dgvHistorial.Columns["FechaHora"].HeaderText = "Fecha/Hora";
                    dgvHistorial.Columns["FechaHora"].Width = 150;
                    dgvHistorial.Columns["RutaArchivo"].HeaderText = "Ruta del Archivo";
                    dgvHistorial.Columns["RutaArchivo"].Width = 300;
                    dgvHistorial.Columns["Estado"].Width = 100;
                    dgvHistorial.Columns["MensajeError"].HeaderText = "Mensaje de Error";
                    dgvHistorial.Columns["MensajeError"].Width = 200;
                    dgvHistorial.Columns["UsuarioApp"].HeaderText = "Usuario";
                    dgvHistorial.Columns["UsuarioApp"].Width = 150;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar la grilla: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Helper para habilitar/deshabilitar controles durante operaciones largas
        /// </summary>
        /// <param name="trabajando">True si está trabajando, False si terminó</param>
        /// <param name="mensaje">Mensaje a mostrar en el label de estado</param>
        private void SetTrabajando(bool trabajando, string mensaje = "")
        {
            // Habilitar/deshabilitar botones
            btnCrearBackup.Enabled = !trabajando;
            btnRestaurar.Enabled = !trabajando;

            // Mostrar/ocultar progressBar
            progressBar.Visible = trabajando;
            progressBar.Style = trabajando ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;

            // Actualizar label de estado
            lblEstado.Visible = trabajando;
            lblEstado.Text = trabajando ? mensaje : "Listo";

            // Actualizar cursor
            this.Cursor = trabajando ? Cursors.WaitCursor : Cursors.Default;

            // Forzar actualización de la UI
            Application.DoEvents();
        }

        #endregion
    }
}
