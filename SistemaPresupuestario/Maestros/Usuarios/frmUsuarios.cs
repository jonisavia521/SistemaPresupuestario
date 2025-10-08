using Services.Services.Contracts;
using System;
using System.Linq;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros
{
    public partial class frmUsuarios : Form
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IServiceProvider _serviceProvider;

        public frmUsuarios(IUsuarioService usuarioService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _usuarioService = usuarioService;
            _serviceProvider = serviceProvider;
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
                MessageBox.Show($"Error al cargar usuarios: {ex.Message}",
                    "Error",
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
                MessageBox.Show("Seleccione un usuario para editar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("Seleccione un usuario para eliminar", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var usuarioNombre = dgvUsuarios.SelectedRows[0].Cells["Nombre"].Value.ToString();
            var confirmResult = MessageBox.Show(
                $"¿Está seguro que desea eliminar al usuario '{usuarioNombre}'?",
                "Confirmar eliminación",
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

                    MessageBox.Show("Usuario eliminado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarUsuariosAsync(); // Refrescar grilla
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }
    }
}