using Services.BLL;
using Services.BLL.Contracts;
using System;
using System.IO;
using System.Windows.Forms;

namespace SistemaPresupuestario.Configuracion
{
    /// <summary>
    /// Formulario para gestionar Backup y Restore de la base de datos
    /// Este formulario es "tonto" - solo llama al servicio de BLL
    /// No contiene lógica de negocio ni acceso directo a datos
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
        private void frmBackupRestore_Load(object sender, EventArgs e)
        {
            try
            {
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

        /// <summary>
        /// Evento para crear un backup
        /// </summary>
        private async void btnCrearBackup_Click(object sender, EventArgs e)
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
                        // Obtener usuario actual (puedes reemplazar esto con el usuario real del sistema)
                        string usuarioActual = Environment.UserName;

                        // Llamar al servicio de forma asíncrona
                        await _servicio.CrearBackupAsync(rutaArchivo, usuarioActual);

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
        private async void btnRestaurar_Click(object sender, EventArgs e)
        {
            // Mostrar advertencia crítica
            var result = MessageBox.Show(
                "⚠️ ADVERTENCIA CRÍTICA ⚠️\n\n" +
                "Esta operación reemplazará TODOS los datos actuales de la base de datos.\n\n" +
                "Se perderán todos los cambios realizados desde el backup seleccionado.\n\n" +
                "La aplicación se reiniciará automáticamente después de la restauración.\n\n" +
                "¿Está SEGURO de que desea continuar?",
                "Confirmar Restauración",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
                return;

            // Configurar OpenFileDialog
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Archivos de Backup (*.bak)|*.bak|Todos los archivos (*.*)|*.*";
                openFileDialog.Title = "Seleccionar Backup para Restaurar";
                openFileDialog.DefaultExt = "bak";
                openFileDialog.CheckFileExists = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string rutaArchivo = openFileDialog.FileName;

                    SetTrabajando(true, "Restaurando backup... Por favor espere. NO CIERRE LA APLICACIÓN.");

                    try
                    {
                        // Llamar al servicio de forma asíncrona
                        await _servicio.RestaurarBackupAsync(rutaArchivo);

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
    }
}
