using BLL.Contracts.Seguridad;
using BLL.DTOs.Seguridad;
using BLL.Exceptions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros.Seguridad
{
    /// <summary>
    /// Formulario de edición de usuarios con asignación de permisos
    /// DECISIÓN: Llamada directa a BLL bypass Service Layer por restricción
    /// </summary>
    public partial class FrmUsuarioEdit : Form
    {
        private readonly IUsuarioBusinessService _usuarioService;
        private readonly IPermisosBusinessService _permisosService;
        
        private UserEditDto _currentUser;
        private bool _isEditing = false;
        private bool _isDirty = false;

        // Collections para los controles de permisos
        private List<FamiliaDto> _todasFamilias = new List<FamiliaDto>();
        private List<PatenteDto> _todasPatentes = new List<PatenteDto>();

        public FrmUsuarioEdit(IUsuarioBusinessService usuarioService, IPermisosBusinessService permisosService)
        {
            _usuarioService = usuarioService ?? throw new ArgumentNullException(nameof(usuarioService));
            _permisosService = permisosService ?? throw new ArgumentNullException(nameof(permisosService));
            
            InitializeComponent();
            InitializeForm();
        }

        public FrmUsuarioEdit(IUsuarioBusinessService usuarioService, IPermisosBusinessService permisosService, Guid usuarioId)
            : this(usuarioService, permisosService)
        {
            _isEditing = true;
            LoadUser(usuarioId);
        }

        private void InitializeForm()
        {
            // Configurar validación
            txtNombre.Validating += TxtNombre_Validating;
            txtUsuario.Validating += TxtUsuario_Validating;
            txtPassword.Validating += TxtPassword_Validating;
            txtConfirmPassword.Validating += TxtConfirmPassword_Validating;

            // Eventos de cambio para marcar como dirty
            txtNombre.TextChanged += (s, e) => MarkDirty();
            txtUsuario.TextChanged += (s, e) => MarkDirty();
            txtPassword.TextChanged += (s, e) => MarkDirty();
            txtConfirmPassword.TextChanged += (s, e) => MarkDirty();
            chkCambiarPassword.CheckedChanged += ChkCambiarPassword_CheckedChanged;

            // Configurar TreeView de familias
            tvFamilias.CheckBoxes = true;
            tvFamilias.AfterCheck += TvFamilias_AfterCheck;

            // Configurar ListBox de patentes (dual list)
            lstPatentesDisponibles.SelectionMode = SelectionMode.MultiExtended;
            lstPatentesAsignadas.SelectionMode = SelectionMode.MultiExtended;

            // Botones para mover patentes
            btnAsignarPatente.Click += BtnAsignarPatente_Click;
            btnDesasignarPatente.Click += BtnDesasignarPatente_Click;

            // Configurar form
            if (_isEditing)
            {
                this.Text = "Editar Usuario";
                chkCambiarPassword.Visible = true;
                lblPassword.Text = "Nueva Contraseña:";
                lblConfirmPassword.Text = "Confirmar Nueva:";
                txtPassword.Enabled = false;
                txtConfirmPassword.Enabled = false;
            }
            else
            {
                this.Text = "Nuevo Usuario";
                chkCambiarPassword.Visible = false;
                lblPassword.Text = "Contraseña:";
                lblConfirmPassword.Text = "Confirmar Contraseña:";
            }
        }

        private async void LoadUser(Guid usuarioId)
        {
            try
            {
                ShowLoading(true);
                
                _currentUser = await _usuarioService.GetUserForEditAsync(usuarioId);
                
                // Cargar datos básicos
                txtNombre.Text = _currentUser.Nombre;
                txtUsuario.Text = _currentUser.Usuario;
                chkActivo.Checked = _currentUser.Activo;

                // Cargar familias y patentes
                await LoadFamiliasAsync();
                await LoadPatentesAsync();

                _isDirty = false;
                UpdateUI();
            }
            catch (Exception ex)
            {
                ShowError($"Error al cargar usuario: {ex.Message}");
                this.Close();
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private async Task LoadFamiliasAsync()
        {
            try
            {
                // TODO: Implementar IFamiliaBusinessService para obtener familias con jerarquía
                // Por ahora usar lista vacía
                _todasFamilias = new List<FamiliaDto>();
                
                PopulateFamiliasTreeView();
            }
            catch (Exception ex)
            {
                ShowError($"Error al cargar familias: {ex.Message}");
            }
        }

        private async Task LoadPatentesAsync()
        {
            try
            {
                var patentes = await _permisosService.GetAllPatentesWithAssignmentStatusAsync(_currentUser?.Id ?? Guid.Empty);
                _todasPatentes = patentes.ToList();
                
                PopulatePatentesLists();
            }
            catch (Exception ex)
            {
                ShowError($"Error al cargar patentes: {ex.Message}");
            }
        }

        private void PopulateFamiliasTreeView()
        {
            tvFamilias.Nodes.Clear();
            
            // TODO: Construir TreeView jerárquico con familias
            // Marcar como checked las familias asignadas
            foreach (var familia in _todasFamilias)
            {
                var node = new TreeNode(familia.Nombre) { Tag = familia };
                node.Checked = _currentUser?.FamiliasAsignadas?.Contains(familia.Id) == true;
                tvFamilias.Nodes.Add(node);
            }
        }

        private void PopulatePatentesLists()
        {
            lstPatentesDisponibles.Items.Clear();
            lstPatentesAsignadas.Items.Clear();

            foreach (var patente in _todasPatentes)
            {
                var displayText = $"{patente.Nombre}";
                if (!string.IsNullOrEmpty(patente.Vista))
                    displayText += $" ({patente.Vista})";

                var item = new ListItem { Text = displayText, Value = patente };

                if (patente.AsignadaDirectamente)
                {
                    lstPatentesAsignadas.Items.Add(item);
                }
                else
                {
                    // Mostrar información si es heredada
                    if (patente.EsHeredada)
                        displayText += $" [Heredada de: {patente.FamiliaOrigen}]";
                    
                    lstPatentesDisponibles.Items.Add(item);
                }
            }
        }

        private void ChkCambiarPassword_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = chkCambiarPassword.Checked;
            txtPassword.Enabled = enabled;
            txtConfirmPassword.Enabled = enabled;
            
            if (!enabled)
            {
                txtPassword.Clear();
                txtConfirmPassword.Clear();
            }
            
            MarkDirty();
        }

        private void TvFamilias_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                MarkDirty();
            }
        }

        private void BtnAsignarPatente_Click(object sender, EventArgs e)
        {
            MoveSelectedItems(lstPatentesDisponibles, lstPatentesAsignadas);
            MarkDirty();
        }

        private void BtnDesasignarPatente_Click(object sender, EventArgs e)
        {
            MoveSelectedItems(lstPatentesAsignadas, lstPatentesDisponibles);
            MarkDirty();
        }

        private void MoveSelectedItems(ListBox source, ListBox destination)
        {
            var selectedItems = source.SelectedItems.Cast<object>().ToList();
            foreach (var item in selectedItems)
            {
                source.Items.Remove(item);
                destination.Items.Add(item);
            }
        }

        private async void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                ShowLoading(true);

                var userDto = BuildUserDto();

                if (_isEditing)
                {
                    await _usuarioService.UpdateUserAsync(userDto);
                    ShowInfo("Usuario actualizado exitosamente");
                }
                else
                {
                    var newId = await _usuarioService.CreateUserAsync(userDto);
                    _currentUser = userDto;
                    _currentUser.Id = newId;
                    _isEditing = true;
                    ShowInfo("Usuario creado exitosamente");
                }

                _isDirty = false;
                UpdateUI();
            }
            catch (DomainValidationException ex)
            {
                ShowValidationError(ex);
            }
            catch (ConcurrencyException ex)
            {
                ShowError($"Conflicto de concurrencia: {ex.Message}\\n\\nEl formulario se recargará con los datos actuales.");
                if (_isEditing)
                    await LoadUser(_currentUser.Id);
            }
            catch (Exception ex)
            {
                ShowError($"Error al guardar: {ex.Message}");
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private UserEditDto BuildUserDto()
        {
            var dto = new UserEditDto
            {
                Id = _currentUser?.Id ?? Guid.Empty,
                Nombre = txtNombre.Text.Trim(),
                Usuario = txtUsuario.Text.Trim(),
                Activo = chkActivo.Checked,
                VersionConcurrencia = _currentUser?.VersionConcurrencia
            };

            // Password
            if (!_isEditing || chkCambiarPassword.Checked)
            {
                dto.Clave = txtPassword.Text;
                dto.ConfirmarClave = txtConfirmPassword.Text;
                dto.CambiarClave = true;
            }

            // Familias asignadas
            dto.FamiliasAsignadas = GetCheckedFamilias();

            // Patentes asignadas
            dto.PatentesAsignadas = GetAssignedPatentes();

            return dto;
        }

        private List<Guid> GetCheckedFamilias()
        {
            var familias = new List<Guid>();
            foreach (TreeNode node in tvFamilias.Nodes)
            {
                if (node.Checked && node.Tag is FamiliaDto familia)
                {
                    familias.Add(familia.Id);
                }
                // TODO: Procesar nodos hijos recursivamente
            }
            return familias;
        }

        private List<Guid> GetAssignedPatentes()
        {
            var patentes = new List<Guid>();
            foreach (ListItem item in lstPatentesAsignadas.Items)
            {
                if (item.Value is PatenteDto patente)
                {
                    patentes.Add(patente.Id);
                }
            }
            return patentes;
        }

        private bool ValidateForm()
        {
            errorProvider.Clear();
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                errorProvider.SetError(txtNombre, "El nombre es obligatorio");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                errorProvider.SetError(txtUsuario, "El usuario es obligatorio");
                isValid = false;
            }

            if (!_isEditing || chkCambiarPassword.Checked)
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    errorProvider.SetError(txtPassword, "La contraseña es obligatoria");
                    isValid = false;
                }
                else if (txtPassword.Text.Length < 6)
                {
                    errorProvider.SetError(txtPassword, "La contraseña debe tener al menos 6 caracteres");
                    isValid = false;
                }

                if (txtPassword.Text != txtConfirmPassword.Text)
                {
                    errorProvider.SetError(txtConfirmPassword, "Las contraseñas no coinciden");
                    isValid = false;
                }
            }

            return isValid;
        }

        private void TxtNombre_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var textBox = sender as TextBox;
            errorProvider.SetError(textBox, "");

            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                errorProvider.SetError(textBox, "El nombre es obligatorio");
            }
            else if (textBox.Text.Trim().Length < 2)
            {
                errorProvider.SetError(textBox, "El nombre debe tener al menos 2 caracteres");
            }
        }

        private void TxtUsuario_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var textBox = sender as TextBox;
            errorProvider.SetError(textBox, "");

            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                errorProvider.SetError(textBox, "El usuario es obligatorio");
            }
            else if (textBox.Text.Trim().Length < 3)
            {
                errorProvider.SetError(textBox, "El usuario debe tener al menos 3 caracteres");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, @"^[a-zA-Z0-9_]+$"))
            {
                errorProvider.SetError(textBox, "Solo se permiten letras, números y guiones bajos");
            }
        }

        private void TxtPassword_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var textBox = sender as TextBox;
            errorProvider.SetError(textBox, "");

            if (textBox.Enabled && string.IsNullOrWhiteSpace(textBox.Text))
            {
                errorProvider.SetError(textBox, "La contraseña es obligatoria");
            }
            else if (textBox.Enabled && textBox.Text.Length < 6)
            {
                errorProvider.SetError(textBox, "La contraseña debe tener al menos 6 caracteres");
            }
        }

        private void TxtConfirmPassword_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var textBox = sender as TextBox;
            errorProvider.SetError(textBox, "");

            if (textBox.Enabled && txtPassword.Text != textBox.Text)
            {
                errorProvider.SetError(textBox, "Las contraseñas no coinciden");
            }
        }

        private void MarkDirty()
        {
            _isDirty = true;
            UpdateUI();
        }

        private void UpdateUI()
        {
            btnGuardar.Enabled = _isDirty;
            this.Text = $"{(_isEditing ? "Editar" : "Nuevo")} Usuario{(_isDirty ? "*" : "")}";
        }

        private void ShowLoading(bool show)
        {
            this.Cursor = show ? Cursors.WaitCursor : Cursors.Default;
            this.Enabled = !show;
        }

        private void ShowInfo(string message)
        {
            MessageBox.Show(message, "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowValidationError(DomainValidationException ex)
        {
            var control = GetControlByFieldName(ex.Campo);
            if (control != null)
            {
                errorProvider.SetError(control, ex.Message);
                control.Focus();
            }
            else
            {
                ShowError(ex.Message);
            }
        }

        private Control GetControlByFieldName(string fieldName)
        {
            switch (fieldName?.ToLower())
            {
                case "nombre": return txtNombre;
                case "usuario": return txtUsuario;
                case "clave": 
                case "password": return txtPassword;
                case "confirmarclave":
                case "confirmarpassword": return txtConfirmPassword;
                default: return null;
            }
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            if (_isDirty)
            {
                var result = MessageBox.Show(
                    "¿Está seguro de cancelar? Se perderán los cambios no guardados.",
                    "Confirmar Cancelación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return;
            }

            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_isDirty && e.CloseReason == CloseReason.UserClosing)
            {
                var result = MessageBox.Show(
                    "¿Está seguro de cerrar? Se perderán los cambios no guardados.",
                    "Confirmar Cierre",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnFormClosing(e);
        }

        // Clase auxiliar para ListBox items
        private class ListItem
        {
            public string Text { get; set; }
            public object Value { get; set; }
            public override string ToString() => Text;
        }
    }
}