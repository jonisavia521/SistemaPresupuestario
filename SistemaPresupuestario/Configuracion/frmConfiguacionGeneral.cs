using BLL.Contracts;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using BLL.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using Services.Services.Contracts;

namespace SistemaPresupuestario.Configuracion
{
    /// <summary>
    /// Formulario para gestionar configuración general del sistema, idioma y Backup/Restore
    /// Dividido en 3 pestañas para mejor organización
    /// REFACTORIZADO: Ahora usa inyección de dependencias
    /// </summary>
    public partial class frmConfiguacionGeneral : Form
    {
        private readonly IBackupRestoreService _servicio;
        private readonly IProvinciaService _provinciaService;
        private readonly IConfiguracionService _configuracionService;
        private ErrorProvider _errorProvider;

        public frmConfiguacionGeneral(
            IBackupRestoreService servicio, 
            IProvinciaService provinciaService,
            IConfiguracionService configuracionService)
        {
            InitializeComponent();
            _servicio = servicio ?? throw new ArgumentNullException(nameof(servicio));
            _provinciaService = provinciaService ?? throw new ArgumentNullException(nameof(provinciaService));
            _configuracionService = configuracionService ?? throw new ArgumentNullException(nameof(configuracionService));
            
            // Inicializar ErrorProvider
            _errorProvider = new ErrorProvider();
            _errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        }

        /// <summary>
        /// Carga inicial del formulario
        /// </summary>
        private void frmConfiguacionGeneral_Load(object sender, EventArgs e)
        {
            try
            {
                // Cargar configuración general
                CargarTiposIva();
                CargarProvincias();
                CargarConfiguracion();
                
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

        private void CargarConfiguracion()
        {
            try
            {
                var configuracion = _configuracionService.ObtenerConfiguracion();
                
                if (configuracion != null)
                {
                    txtRazonSocial.Text = configuracion.RazonSocial;
                    txtCUIT.Text = configuracion.CUIT;
                    txtDireccion.Text = configuracion.Direccion;
                    txtLocalidad.Text = configuracion.Localidad;
                    txtEmail.Text = configuracion.Email;
                    txtTelefono.Text = configuracion.Telefono;
                    
                    // Seleccionar tipo de IVA
                    if (!string.IsNullOrEmpty(configuracion.TipoIva))
                    {
                        cboTipoIva.SelectedItem = configuracion.TipoIva;
                    }
                    
                    // Seleccionar provincia
                    if (configuracion.IdProvincia.HasValue)
                    {
                        for (int i = 0; i < cboProvincia.Items.Count; i++)
                        {
                            var item = cboProvincia.Items[i];
                            var idProvincia = (Guid?)item.GetType().GetProperty("Id").GetValue(item);
                            if (idProvincia == configuracion.IdProvincia)
                            {
                                cboProvincia.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la configuración: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarTiposIva()
        {
            cboTipoIva.Items.Clear();
            cboTipoIva.Items.AddRange(new object[]
            {
                "RESPONSABLE INSCRIPTO",
                "MONOTRIBUTISTA",
                "EXENTO",
                "CONSUMIDOR FINAL",
                "NO RESPONSABLE"
            });
            cboTipoIva.SelectedIndex = 0; // Responsable Inscripto por defecto
        }

        private void CargarProvincias()
        {
            try
            {
                var provincias = _provinciaService.GetAllOrdenadas();

                cboProvincia.Items.Clear();
                cboProvincia.Items.Add(new { Id = (Guid?)null, Text = "(Sin provincia)" });

                foreach (var provincia in provincias)
                {
                    cboProvincia.Items.Add(new { Id = (Guid?)provincia.Id, Text = provincia.NombreCompleto });
                }

                cboProvincia.DisplayMember = "Text";
                cboProvincia.ValueMember = "Id";
                cboProvincia.SelectedIndex = 0; // Sin provincia por defecto
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar provincias: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Guarda la configuración de la empresa en la base de datos
        /// </summary>
        private void btnGuardarConfiguracion_Click(object sender, EventArgs e)
        {
            try
            {
                // Limpiar errores previos
                _errorProvider.Clear();

                // Crear el DTO
                var dto = new ConfiguracionDTO
                {
                    RazonSocial = txtRazonSocial.Text?.Trim(),
                    CUIT = txtCUIT.Text?.Replace("-", "").Replace(" ", "").Trim(),
                    TipoIva = cboTipoIva.SelectedItem?.ToString(),
                    Direccion = txtDireccion.Text?.Trim(),
                    Localidad = txtLocalidad.Text?.Trim(),
                    Email = txtEmail.Text?.Trim(),
                    Telefono = txtTelefono.Text?.Trim(),
                    Idioma = rbEspanol.Checked ? "es-AR" : "en-US"
                };

                // Obtener provincia seleccionada
                if (cboProvincia.SelectedIndex > 0)
                {
                    var itemSeleccionado = cboProvincia.SelectedItem;
                    dto.IdProvincia = (Guid?)itemSeleccionado.GetType().GetProperty("Id").GetValue(itemSeleccionado);
                }

                // Validar con DataAnnotations
                var contextoValidacion = new ValidationContext(dto);
                var resultadosValidacion = new List<ValidationResult>();
                
                if (!Validator.TryValidateObject(dto, contextoValidacion, resultadosValidacion, true))
                {
                    // Mostrar errores con ErrorProvider
                    foreach (var resultado in resultadosValidacion)
                    {
                        foreach (var nombrePropiedad in resultado.MemberNames)
                        {
                            Control control = null;
                            
                            switch (nombrePropiedad)
                            {
                                case "RazonSocial":
                                    control = txtRazonSocial;
                                    break;
                                case "CUIT":
                                    control = txtCUIT;
                                    break;
                                case "TipoIva":
                                    control = cboTipoIva;
                                    break;
                                case "Email":
                                    control = txtEmail;
                                    break;
                            }
                            
                            if (control != null)
                            {
                                _errorProvider.SetError(control, resultado.ErrorMessage);
                            }
                        }
                    }
                    return;
                }

                // Guardar en base de datos
                _configuracionService.GuardarConfiguracion(dto);

                MessageBox.Show("Configuración guardada exitosamente", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar configuración: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                // Primero intentar cargar desde la configuración guardada
                var configuracion = _configuracionService.ObtenerConfiguracion();
                
                if (configuracion != null && !string.IsNullOrEmpty(configuracion.Idioma))
                {
                    if (configuracion.Idioma.StartsWith("es"))
                    {
                        rbEspanol.Checked = true;
                        lblIdiomaActual.Text = "Español";
                    }
                    else if (configuracion.Idioma.StartsWith("en"))
                    {
                        rbIngles.Checked = true;
                        lblIdiomaActual.Text = "English";
                    }
                }
                else
                {
                    // Si no hay configuración, usar el idioma actual del thread
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

                // Obtener la configuración actual
                var configuracion = _configuracionService.ObtenerConfiguracion();
                
                if (configuracion != null)
                {
                    // Actualizar solo el idioma
                    configuracion.Idioma = nuevoIdioma;
                    
                    // Guardar en base de datos
                    _configuracionService.GuardarConfiguracion(configuracion);
                }
                else
                {
                    // Si no existe configuración, mostrar mensaje de error
                    MessageBox.Show(
                        "No se encontró configuración del sistema.\n\n" +
                        "Por favor, complete primero los datos de configuración en la pestaña 'Configuración General'.",
                        "Configuración Requerida",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Actualizar el label
                lblIdiomaActual.Text = nombreIdioma;

                MessageBox.Show(
                    $"Idioma cambiado a: {nombreIdioma}\n\n" +
                    "El cambio se ha guardado en la configuración del sistema.\n\n" +
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
                var historial = _servicio.ObtenerHistorial();
                
                // Asignar el DataTable al DataSource
                // Las columnas ya están definidas en el Designer con DataPropertyName
                dgvHistorial.DataSource = historial;
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
