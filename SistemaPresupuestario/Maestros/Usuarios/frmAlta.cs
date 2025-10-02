using Services.DomainModel.Security.Composite;
using Services.Services.Contracts;
using System;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros.Usuarios
{
    public partial class frmAlta : Form
    {
        private readonly IUsuarioService _usuarioService;

        public frmAlta(IUsuarioService usuarioService)
        {
            InitializeComponent();
            _usuarioService = usuarioService;
        }

        private void frmAlta_Load(object sender, EventArgs e)
        {
            // Configurar controles si es necesario
        }

        private async void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                // VALIDACIONES en UI
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    MessageBox.Show("El nombre es requerido", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNombre.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(btnUsuario.Text))
                {
                    MessageBox.Show("El usuario es requerido", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnUsuario.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(btnClave.Text))
                {
                    MessageBox.Show("La contraseña es requerida", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    btnClave.Focus();
                    return;
                }

                this.Cursor = Cursors.WaitCursor;

                // Crear objeto Usuario
                var nuevoUsuario = new Usuario
                {
                    Id = Guid.NewGuid(), // Generar nuevo ID
                    Nombre = txtNombre.Text.Trim(),
                    User = btnUsuario.Text.Trim(),
                    Password = btnClave.Text // El hasheado se hace automáticamente en la propiedad
                };

                // Llamar al servicio (capa de negocio)
                bool resultado =  _usuarioService.Add(nuevoUsuario);

                if (resultado)
                {
                    MessageBox.Show("Usuario creado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}