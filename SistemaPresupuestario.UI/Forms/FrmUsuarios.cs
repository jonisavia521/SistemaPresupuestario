using System;
using System.Linq;
using System.Windows.Forms;
using SistemaPresupuestario.BLL.DTOs;
using SistemaPresupuestario.Service.Seguridad;

namespace SistemaPresupuestario.UI.Forms
{
    /// <summary>
    /// Main form for user management (ABM Usuarios)
    /// Provides listing, search, and CRUD operations for users
    /// </summary>
    public partial class FrmUsuarios : Form
    {
        private readonly IUsuarioService _usuarioService;
        private BindingSource _usuariosBindingSource;

        public FrmUsuarios()
        {
            InitializeComponent();
            _usuarioService = new UsuarioService();
            _usuariosBindingSource = new BindingSource();
            
            ConfigureGrid();
            LoadUsers();
        }

        private void ConfigureGrid()
        {
            // Configure DataGridView
            dgvUsuarios.AutoGenerateColumns = false;
            dgvUsuarios.DataSource = _usuariosBindingSource;

            // Add columns
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNombre",
                HeaderText = "Nombre",
                DataPropertyName = "Nombre",
                Width = 200
            });

            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNombreUsuario",
                HeaderText = "Usuario",
                DataPropertyName = "NombreUsuario",
                Width = 150
            });

            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEmail",
                HeaderText = "Email",
                DataPropertyName = "Email",
                Width = 200
            });

            dgvUsuarios.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colActivo",
                HeaderText = "Activo",
                DataPropertyName = "Activo",
                Width = 80
            });

            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFamilias",
                HeaderText = "Familias",
                DataPropertyName = "FamiliasCount",
                Width = 100
            });

            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPatentes",
                HeaderText = "Patentes",
                DataPropertyName = "PatentesCount",
                Width = 100
            });

            // Configure selection
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.MultiSelect = false;
        }

        private void LoadUsers()
        {
            try
            {
                var searchText = string.IsNullOrWhiteSpace(txtBuscar.Text) ? null : txtBuscar.Text.Trim();
                var usuarios = _usuarioService.GetUsers(searchText);
                
                // Add calculated properties
                var usuariosDisplay = usuarios.Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.NombreUsuario,
                    u.Email,
                    u.Activo,
                    FamiliasCount = u.Familias?.Count ?? 0,
                    PatentesCount = u.Patentes?.Count ?? 0,
                    u.CreatedAt,
                    u.ModifiedAt
                }).ToList();

                _usuariosBindingSource.DataSource = usuariosDisplay;
                
                // Update status
                lblStatus.Text = $"Usuarios encontrados: {usuariosDisplay.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            var editForm = new FrmUsuarioEdit();
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadUsers();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            var selectedUser = GetSelectedUser();
            if (selectedUser == null)
            {
                MessageBox.Show("Please select a user to edit.", "Information", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var editForm = new FrmUsuarioEdit(selectedUser.Id);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                LoadUsers();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            var selectedUser = GetSelectedUser();
            if (selectedUser == null)
            {
                MessageBox.Show("Please select a user to delete.", "Information", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Are you sure you want to delete user '{selectedUser.NombreUsuario}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    _usuarioService.DeleteUser(selectedUser.Id);
                    LoadUsers();
                    MessageBox.Show("User deleted successfully.", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting user: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRefrescar_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = "";
            LoadUsers();
        }

        private void dgvUsuarios_DoubleClick(object sender, EventArgs e)
        {
            btnEditar_Click(sender, e);
        }

        private dynamic GetSelectedUser()
        {
            if (dgvUsuarios.SelectedRows.Count > 0)
            {
                return dgvUsuarios.SelectedRows[0].DataBoundItem;
            }
            return null;
        }

        private void txtBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnBuscar_Click(sender, e);
                e.Handled = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _usuarioService?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}