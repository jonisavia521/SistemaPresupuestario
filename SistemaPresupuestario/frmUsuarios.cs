using BLL.Contracts;
using BLL.DTOs;
using BLL.Exceptions;
using Services.Services.Contracts;
using SistemaPresupuestario.Maestros.Usuarios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros
{
    public partial class frmUsuarios : Form
    {
        private readonly IUsuarioService usuarioService;
        private BindingList<UserListItemDto> usuarios;
        private int currentPage = 1;
        private int pageSize = 50;
        private int totalRecords = 0;

        public frmUsuarios(IUsuarioService usuarioService)
        {
            InitializeComponent();
            this.usuarioService = usuarioService;
            usuarios = new BindingList<UserListItemDto>();
            dgvUsuarios.DataSource = usuarios;
        }
       
        private async void frmUsuarios_Load(object sender, EventArgs e)
        {
            await LoadUsuarios();
        }

        private async Task LoadUsuarios(string filtro = null)
        {
            try
            {
                lblStatus.Text = "Cargando usuarios...";
                Application.DoEvents();

                // Obtener usuarios paginados
                var usuariosResult = await usuarioService.GetPagedUsersAsync(filtro, currentPage, pageSize);
                totalRecords = usuariosResult.TotalItems;
                
                // Crear DTOs para la vista con información adicional
                var usuariosParaVista = new List<UserListItemDto>();
                
                foreach (var usuario in usuariosResult.Items)
                {
                    // Calcular permisos efectivos para mostrar en la grilla
                    var permisosEfectivos = await usuarioService.GetEffectivePermissionsAsync(usuario.Id);
                    
                    usuariosParaVista.Add(new UserListItemDto
                    {
                        Id = usuario.Id,
                        Nombre = usuario.Nombre,
                        Usuario = usuario.Usuario,
                        CantPermisosDirectos = usuario.FamiliasAsignadas.Count + usuario.PatentesAsignadas.Count,
                        CantPermisosEfectivos = permisosEfectivos.TotalPermisosUnicos,
                        Timestamp = usuario.Timestamp
                    });
                }

                usuarios.Clear();
                foreach (var item in usuariosParaVista)
                {
                    usuarios.Add(item);
                }

                lblTotal.Text = $"Total: {totalRecords} usuario(s)";
                lblStatus.Text = "Listo";
                
                // Habilitar/deshabilitar botones según selección
                UpdateButtonsState();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error al cargar usuarios";
                MessageBox.Show($"Error al cargar los usuarios: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateButtonsState()
        {
            bool hasSelection = dgvUsuarios.SelectedRows.Count > 0;
            btnEditar.Enabled = hasSelection;
            btnEliminar.Enabled = hasSelection;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            try
            {
                var form = new frmUsuarioEdit(usuarioService);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // Recargar la lista
                    Task.Run(async () => await LoadUsuarios(txtFiltro.Text));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el formulario de nuevo usuario: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un usuario para editar.", "Información", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var selectedUser = (UserListItemDto)dgvUsuarios.SelectedRows[0].DataBoundItem;
                var form = new frmUsuarioEdit(usuarioService, selectedUser.Id);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // Recargar la lista
                    Task.Run(async () => await LoadUsuarios(txtFiltro.Text));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el formulario de edición: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un usuario para eliminar.", "Información", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedUser = (UserListItemDto)dgvUsuarios.SelectedRows[0].DataBoundItem;

            if (MessageBox.Show($"¿Está seguro que desea eliminar el usuario '{selectedUser.Nombre}'?\n\nEsta acción no se puede deshacer.", 
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    lblStatus.Text = "Eliminando usuario...";
                    Application.DoEvents();

                    await usuarioService.DeleteUserAsync(selectedUser.Id, selectedUser.Timestamp);
                    
                    lblStatus.Text = "Usuario eliminado exitosamente";
                    MessageBox.Show("Usuario eliminado exitosamente.", "Información", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    await LoadUsuarios(txtFiltro.Text);
                }
                catch (ConcurrencyException ex)
                {
                    lblStatus.Text = "Error de concurrencia";
                    MessageBox.Show(ex.Message, "Error de Concurrencia", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    await LoadUsuarios(txtFiltro.Text); // Recargar para obtener datos actuales
                }
                catch (DomainValidationException ex)
                {
                    lblStatus.Text = "Error de validación";
                    MessageBox.Show(ex.Message, "Error de Validación", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    lblStatus.Text = "Error al eliminar usuario";
                    MessageBox.Show($"Error al eliminar el usuario: {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void btnBuscar_Click(object sender, EventArgs e)
        {
            currentPage = 1; // Reset a primera página al buscar
            await LoadUsuarios(txtFiltro.Text.Trim());
        }

        private async void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtFiltro.Clear();
            currentPage = 1;
            await LoadUsuarios();
        }

        private async void btnRefrescar_Click(object sender, EventArgs e)
        {
            await LoadUsuarios(txtFiltro.Text.Trim());
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvUsuarios_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnEditar_Click(sender, e);
            }
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtonsState();
        }

        private async void txtFiltro_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                await btnBuscar_Click(sender, e);
            }
        }
    }

    /// <summary>
    /// DTO para mostrar usuarios en la lista
    /// </summary>
    public class UserListItemDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public int CantPermisosDirectos { get; set; }
        public int CantPermisosEfectivos { get; set; }
        public byte[] Timestamp { get; set; }
    }
}
