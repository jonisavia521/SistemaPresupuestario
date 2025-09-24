using BLL.DTOs;
using BLL.Exceptions;
using Services.Services.Seguridad;
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
    /// <summary>
    /// Formulario principal para gestión de usuarios
    /// DECISIÓN: Implementar paginación en memoria como fallback, idealmente en BLL/DAL
    /// </summary>
    public partial class frmUsuarios : Form
    {
        private readonly IUsuarioService _usuarioService;
        private int _currentPage = 1;
        private int _pageSize = 20;
        private int _totalRecords = 0;
        private int _totalPages = 0;
        private List<UserDto> _currentUsers = new List<UserDto>();

        public frmUsuarios(IUsuarioService usuarioService)
        {
            InitializeComponent();
            _usuarioService = usuarioService;
            ConfigureForm();
        }

        private void ConfigureForm()
        {
            // Configurar DataGridView
            dgvUsuarios.AutoGenerateColumns = false;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.MultiSelect = false;
            
            // Configurar eventos de teclado para filtros
            txtFiltroUsuario.KeyPress += txtFiltros_KeyPress;
            txtFiltroNombre.KeyPress += txtFiltros_KeyPress;
        }

        private async void frmUsuarios_Load(object sender, EventArgs e)
        {
            try
            {
                await LoadUsuariosAsync();
            }
            catch (Exception ex)
            {
                ShowError("Error al cargar usuarios", ex);
            }
        }

        private async Task LoadUsuariosAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                
                var filtroUsuario = string.IsNullOrWhiteSpace(txtFiltroUsuario.Text) ? null : txtFiltroUsuario.Text.Trim();
                var filtroNombre = string.IsNullOrWhiteSpace(txtFiltroNombre.Text) ? null : txtFiltroNombre.Text.Trim();

                var result = await _usuarioService.GetUsuariosPaginatedAsync(filtroUsuario, filtroNombre, _currentPage, _pageSize);
                
                _currentUsers = result.usuarios;
                _totalRecords = result.total;
                _totalPages = (int)Math.Ceiling((double)_totalRecords / _pageSize);

                dgvUsuarios.DataSource = _currentUsers;
                UpdatePaginationInfo();
                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                ShowError("Error al cargar datos", ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void UpdatePaginationInfo()
        {
            if (_totalRecords == 0)
            {
                lblPaginacion.Text = "No se encontraron registros";
            }
            else
            {
                lblPaginacion.Text = $"Página {_currentPage} de {_totalPages} ({_totalRecords} registros)";
            }

            // Habilitar/deshabilitar botones de paginación
            btnPrimero.Enabled = _currentPage > 1;
            btnAnterior.Enabled = _currentPage > 1;
            btnSiguiente.Enabled = _currentPage < _totalPages;
            btnUltimo.Enabled = _currentPage < _totalPages;
        }

        private void UpdateButtonStates()
        {
            bool hasSelection = dgvUsuarios.SelectedRows.Count > 0;
            btnEditar.Enabled = hasSelection;
            btnEliminar.Enabled = hasSelection;
        }

        private void dgvUsuarios_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void dgvUsuarios_DoubleClick(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count > 0)
            {
                btnEditar_Click(sender, e);
            }
        }

        private UserDto GetSelectedUser()
        {
            if (dgvUsuarios.SelectedRows.Count == 0)
                return null;

            return dgvUsuarios.SelectedRows[0].DataBoundItem as UserDto;
        }

        #region Event Handlers

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            try
            {
                var frmEdit = new frmUsuarioEdit(_usuarioService);
                if (frmEdit.ShowDialog() == DialogResult.OK)
                {
                    // Recargar datos después de crear
                    LoadUsuariosAsync();
                }
            }
            catch (Exception ex)
            {
                ShowError("Error al abrir formulario de usuario", ex);
            }
        }

        private async void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedUser = GetSelectedUser();
                if (selectedUser == null)
                {
                    MessageBox.Show("Seleccione un usuario para editar.", "Información", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var frmEdit = new frmUsuarioEdit(_usuarioService, selectedUser.Id);
                if (frmEdit.ShowDialog() == DialogResult.OK)
                {
                    // Recargar datos después de editar
                    await LoadUsuariosAsync();
                }
            }
            catch (Exception ex)
            {
                ShowError("Error al editar usuario", ex);
            }
        }

        private async void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedUser = GetSelectedUser();
                if (selectedUser == null)
                {
                    MessageBox.Show("Seleccione un usuario para eliminar.", "Información", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var result = MessageBox.Show(
                    $"¿Está seguro que desea eliminar el usuario '{selectedUser.Usuario}'?\n\n" +
                    "Esta acción no se puede deshacer.",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    Cursor = Cursors.WaitCursor;
                    await _usuarioService.DeleteUsuarioAsync(selectedUser.Id, selectedUser.TimestampBase64);
                    
                    MessageBox.Show("Usuario eliminado exitosamente.", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    await LoadUsuariosAsync();
                }
            }
            catch (ConcurrencyException ex)
            {
                MessageBox.Show(ex.CreateUserFriendlyMessage(), "Conflicto de concurrencia", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                await LoadUsuariosAsync(); // Recargar datos actualizados
            }
            catch (NotFoundException ex)
            {
                MessageBox.Show("El usuario ya no existe.", "Usuario no encontrado", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                await LoadUsuariosAsync();
            }
            catch (Exception ex)
            {
                ShowError("Error al eliminar usuario", ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async void btnRefrescar_Click(object sender, EventArgs e)
        {
            await LoadUsuariosAsync();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void btnBuscar_Click(object sender, EventArgs e)
        {
            _currentPage = 1; // Resetear a primera página
            await LoadUsuariosAsync();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtFiltroUsuario.Clear();
            txtFiltroNombre.Clear();
            txtFiltroUsuario.Focus();
        }

        private void txtFiltros_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                btnBuscar_Click(sender, e);
            }
        }

        #endregion

        #region Pagination Event Handlers

        private async void btnPrimero_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage = 1;
                await LoadUsuariosAsync();
            }
        }

        private async void btnAnterior_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                await LoadUsuariosAsync();
            }
        }

        private async void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                await LoadUsuariosAsync();
            }
        }

        private async void btnUltimo_Click(object sender, EventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage = _totalPages;
                await LoadUsuariosAsync();
            }
        }

        #endregion

        #region Helper Methods

        private void ShowError(string message, Exception ex)
        {
            string fullMessage = message;
            
            if (ex is DomainValidationException domainEx)
            {
                fullMessage = domainEx.Message;
            }
            else if (ex is ConcurrencyException concEx)
            {
                fullMessage = concEx.CreateUserFriendlyMessage();
            }
            else if (ex is PermissionIntegrityException permEx)
            {
                fullMessage = permEx.CreateUserFriendlyMessage();
            }
            else
            {
                fullMessage += $"\n\nDetalle: {ex.Message}";
            }

            MessageBox.Show(fullMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion
    }
}
