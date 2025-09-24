using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SistemaPresupuestario.BLL.DTOs;
using SistemaPresupuestario.Service.Seguridad;

namespace SistemaPresupuestario.UI.Forms
{
    /// <summary>
    /// Form for creating and editing users
    /// </summary>
    public partial class FrmUsuarioEdit : Form
    {
        private readonly IUsuarioService _usuarioService;
        private readonly Guid? _userId;
        private UsuarioDto _currentUser;
        private bool _isNewUser;

        public FrmUsuarioEdit(Guid? userId = null)
        {
            InitializeComponent();
            _usuarioService = new UsuarioService();
            _userId = userId;
            _isNewUser = !userId.HasValue;
            
            InitializeForm();
            LoadData();
        }

        private void InitializeForm()
        {
            this.Text = _isNewUser ? "Nuevo Usuario" : "Editar Usuario";
            
            // Set default values
            chkActivo.Checked = true;
            chkDebeRenovarClave.Checked = false;
            chkCambiarClave.Checked = false;
            
            // Configure password fields visibility
            if (_isNewUser)
            {
                chkCambiarClave.Visible = false;
                lblClave.Visible = true;
                txtClave.Visible = true;
                lblConfirmarClave.Visible = true;
                txtConfirmarClave.Visible = true;
            }
            else
            {
                chkCambiarClave.Visible = true;
                lblClave.Visible = false;
                txtClave.Visible = false;
                lblConfirmarClave.Visible = false;
                txtConfirmarClave.Visible = false;
            }
        }

        private void LoadData()
        {
            try
            {
                LoadFamilias();
                LoadPatentes();

                if (!_isNewUser)
                {
                    _currentUser = _usuarioService.GetUserById(_userId.Value);
                    if (_currentUser != null)
                    {
                        PopulateForm();
                    }
                    else
                    {
                        MessageBox.Show("Usuario no encontrado.", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.DialogResult = DialogResult.Cancel;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadFamilias()
        {
            var familias = _usuarioService.GetAvailableFamilies().ToList();
            
            clbFamilias.Items.Clear();
            foreach (var familia in familias)
            {
                clbFamilias.Items.Add(new FamiliaItem(familia));
            }
        }

        private void LoadPatentes()
        {
            var patentes = _usuarioService.GetAvailablePatents().ToList();
            
            clbPatentes.Items.Clear();
            foreach (var patente in patentes)
            {
                clbPatentes.Items.Add(new PatenteItem(patente));
            }
        }

        private void PopulateForm()
        {
            txtNombre.Text = _currentUser.Nombre;
            txtNombreUsuario.Text = _currentUser.NombreUsuario;
            txtEmail.Text = _currentUser.Email;
            chkActivo.Checked = _currentUser.Activo;
            chkDebeRenovarClave.Checked = _currentUser.DebeRenovarClave;

            // Select assigned families
            if (_currentUser.Familias != null)
            {
                for (int i = 0; i < clbFamilias.Items.Count; i++)
                {
                    var familiaItem = (FamiliaItem)clbFamilias.Items[i];
                    if (_currentUser.Familias.Any(f => f.Id == familiaItem.Familia.Id))
                    {
                        clbFamilias.SetItemChecked(i, true);
                    }
                }
            }

            // Select assigned patents
            if (_currentUser.Patentes != null)
            {
                for (int i = 0; i < clbPatentes.Items.Count; i++)
                {
                    var patenteItem = (PatenteItem)clbPatentes.Items[i];
                    if (_currentUser.Patentes.Any(p => p.Id == patenteItem.Patente.Id))
                    {
                        clbPatentes.SetItemChecked(i, true);
                    }
                }
            }
        }

        private void chkCambiarClave_CheckedChanged(object sender, EventArgs e)
        {
            bool showPasswordFields = chkCambiarClave.Checked;
            
            lblClave.Visible = showPasswordFields;
            txtClave.Visible = showPasswordFields;
            lblConfirmarClave.Visible = showPasswordFields;
            txtConfirmarClave.Visible = showPasswordFields;
            
            if (!showPasswordFields)
            {
                txtClave.Text = "";
                txtConfirmarClave.Text = "";
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                SaveUser();
            }
        }

        private bool ValidateForm()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
                errors.Add("El nombre es obligatorio");

            if (string.IsNullOrWhiteSpace(txtNombreUsuario.Text))
                errors.Add("El nombre de usuario es obligatorio");

            if (_isNewUser && string.IsNullOrWhiteSpace(txtClave.Text))
                errors.Add("La contraseña es obligatoria para usuarios nuevos");

            if (!_isNewUser && chkCambiarClave.Checked && string.IsNullOrWhiteSpace(txtClave.Text))
                errors.Add("Debe ingresar la nueva contraseña");

            if (!string.IsNullOrWhiteSpace(txtClave.Text) && txtClave.Text != txtConfirmarClave.Text)
                errors.Add("Las contraseñas no coinciden");

            if (!string.IsNullOrWhiteSpace(txtEmail.Text) && !IsValidEmail(txtEmail.Text))
                errors.Add("El formato del email no es válido");

            // Check if user has at least one permission
            bool hasFamilias = clbFamilias.CheckedItems.Count > 0;
            bool hasPatentes = clbPatentes.CheckedItems.Count > 0;
            
            if (!hasFamilias && !hasPatentes)
                errors.Add("El usuario debe tener al menos una familia o patente asignada");

            if (errors.Any())
            {
                MessageBox.Show(string.Join("\n", errors), "Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void SaveUser()
        {
            try
            {
                var userDto = CreateUserDto();

                if (_isNewUser)
                {
                    _usuarioService.CreateUser(userDto);
                    MessageBox.Show("Usuario creado exitosamente.", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _usuarioService.UpdateUser(userDto);
                    MessageBox.Show("Usuario actualizado exitosamente.", "Éxito", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving user: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private UsuarioEditDto CreateUserDto()
        {
            var dto = new UsuarioEditDto
            {
                Id = _isNewUser ? Guid.Empty : _userId.Value,
                Nombre = txtNombre.Text.Trim(),
                NombreUsuario = txtNombreUsuario.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Activo = chkActivo.Checked,
                DebeRenovarClave = chkDebeRenovarClave.Checked,
                CambiarClave = _isNewUser || chkCambiarClave.Checked,
                Clave = txtClave.Text,
                ConfirmarClave = txtConfirmarClave.Text
            };

            // Get selected families
            dto.FamiliasAsignadasIds = clbFamilias.CheckedItems
                .Cast<FamiliaItem>()
                .Select(item => item.Familia.Id)
                .ToList();

            // Get selected patents
            dto.PatentesAsignadasIds = clbPatentes.CheckedItems
                .Cast<PatenteItem>()
                .Select(item => item.Patente.Id)
                .ToList();

            return dto;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
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

    // Helper classes for CheckedListBox items
    public class FamiliaItem
    {
        public FamiliaDto Familia { get; }

        public FamiliaItem(FamiliaDto familia)
        {
            Familia = familia;
        }

        public override string ToString()
        {
            return Familia.NombreCompleto;
        }
    }

    public class PatenteItem
    {
        public PatenteDto Patente { get; }

        public PatenteItem(PatenteDto patente)
        {
            Patente = patente;
        }

        public override string ToString()
        {
            return Patente.NombreCompleto;
        }
    }
}