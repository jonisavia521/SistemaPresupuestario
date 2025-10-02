using Services.Services.Contracts;
using SistemaPresupuestario.Maestros.Usuarios;
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

    }
}