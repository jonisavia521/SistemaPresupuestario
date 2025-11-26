using Services.Services.Contracts;
using SistemaPresupuestario.Helpers;
using System;
using System.Linq;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros
{
    public partial class frmUsuarios : FormBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IServiceProvider _serviceProvider;

        public frmUsuarios(IUsuarioService usuarioService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _usuarioService = usuarioService;
            _serviceProvider = serviceProvider;
            
            base.InitializeTranslation();
        }

        private void OnLanguageChanged(object sender, EventArgs e)
        {
            if (dgvUsuarios.Columns.Count > 0)
            {
                ActualizarColumnasGrilla();
            }
        }
        
        /// <summary>
        /// Actualiza los encabezados de columnas de la grilla
        /// </summary>
        private void ActualizarColumnasGrilla()
        {
            foreach (DataGridViewColumn column in dgvUsuarios.Columns)
            {
                if (column.Tag == null && !string.IsNullOrWhiteSpace(column.HeaderText))
                {
                    column.Tag = column.HeaderText;
                }

                if (column.Tag is string key)
                {
                    column.HeaderText = I18n.T(key);
                }
            }
        }

        private void frmUsuarios_Load(object sender, EventArgs e)
        {
            CargarUsuariosAsync();
        }

        private void CargarUsuariosAsync()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                dgvUsuarios.DataSource = null; // Limpiar antes de cargar

                // Llamada al servicio (async/await pattern)
                var usuarios = _usuarioService.GetAll();

                dgvUsuarios.DataSource = usuarios.ToList();
                dgvUsuarios.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{I18n.T("Error al cargar usuarios")}: {ex.Message}",
                    I18n.T("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            // Patrón existente: verificar si ya está abierto
            frmAlta formAbierto = Application.OpenForms.OfType<frmAlta>().FirstOrDefault();

            if (formAbierto == null)
            {
                // ✅ Obtener desde DI con serviceProvider
                var frmAlta = _serviceProvider.GetService(typeof(frmAlta)) as frmAlta;
                frmAlta.MdiParent = this.MdiParent;


                frmAlta.Show();
            }
            else
            {
                formAbierto.BringToFront();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show(I18n.T("Seleccione un usuario para editar"), I18n.T("Advertencia"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var usuarioId = (Guid)dgvUsuarios.SelectedRows[0].Cells["Id"].Value;

            frmAlta formAbierto = Application.OpenForms.OfType<frmAlta>().FirstOrDefault();

            if (formAbierto == null)
            {
                var frmEditar = _serviceProvider.GetService(typeof(frmAlta)) as frmAlta;
                frmEditar.CargarUsuario(usuarioId); // Método nuevo a crear en frmAlta
                frmEditar.MdiParent = this.MdiParent;

                frmEditar.Show();
            }
            else
            {
                formAbierto.BringToFront();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show(I18n.T("Seleccione un usuario para eliminar"), I18n.T("Advertencia"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var usuarioNombre = dgvUsuarios.SelectedRows[0].Cells["Nombre"].Value.ToString();
            var confirmResult = MessageBox.Show(
                $"{I18n.T("¿Está seguro que desea eliminar al usuario")} '{usuarioNombre}'?",
                I18n.T("Confirmar eliminación"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    var usuarioId = (Guid)dgvUsuarios.SelectedRows[0].Cells["Id"].Value;

                    _usuarioService.Delete(usuarioId);

                    MessageBox.Show(I18n.T("Usuario eliminado exitosamente"), I18n.T("Éxito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarUsuariosAsync(); // Refrescar grilla
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{I18n.T("Error al eliminar usuario")}: {ex.Message}", I18n.T("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }
    }
}