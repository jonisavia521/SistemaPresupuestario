using BLL.Contracts;
using BLL.DTOs;
using BLL.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros.Usuarios
{
    public partial class frmUsuarioEdit : Form
    {
        private readonly IUsuarioService _usuarioService;
        private UserEditDto _usuario;
        private List<FamiliaDto> _familias;
        private List<PatenteDto> _patentes;
        private PermisoEfectivoDto _permisosEfectivos;
        private bool _isLoading = false;
        private bool _modoEdicion = false;

        /// <summary>
        /// Constructor para nuevo usuario
        /// </summary>
        public frmUsuarioEdit(IUsuarioService usuarioService)
        {
            InitializeComponent();
            _usuarioService = usuarioService;
            _usuario = new UserEditDto { Id = Guid.Empty }; // Nuevo usuario
            _modoEdicion = false;
            this.Text = "Nuevo Usuario";
        }

        /// <summary>
        /// Constructor para editar usuario existente
        /// </summary>
        public frmUsuarioEdit(IUsuarioService usuarioService, Guid usuarioId)
        {
            InitializeComponent();
            _usuarioService = usuarioService;
            _usuario = new UserEditDto { Id = usuarioId };
            _modoEdicion = true;
            this.Text = "Editar Usuario";
        }

        private async void frmUsuarioEdit_Load(object sender, EventArgs e)
        {
            try
            {
                _isLoading = true;
                lblStatus.Text = "Cargando datos...";

                // Configurar UI inicial
                ConfigureInitialUI();

                // Cargar datos de referencia
                await LoadReferenceData();

                // Cargar datos del usuario si está en modo edición
                if (_modoEdicion && _usuario.Id != Guid.Empty)
                {
                    await LoadUserData();
                }

                // Configurar controles según el modo
                ConfigureControlsForMode();

                lblStatus.Text = "Listo";
                _isLoading = false;
            }
            catch (Exception ex)
            {
                _isLoading = false;
                lblStatus.Text = "Error al cargar datos";
                MessageBox.Show($"Error al cargar los datos: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void ConfigureInitialUI()
        {
            // Configurar campos de clave para modo nuevo/edición
            chkCambiarClave.Visible = _modoEdicion;
            
            if (!_modoEdicion)
            {
                // Nuevo usuario - clave obligatoria
                lblClave.Text = "Clave:*";
                lblConfirmarClave.Text = "Confirmar Clave:*";
                txtClave.Enabled = true;
                txtConfirmarClave.Enabled = true;
            }
            else
            {
                // Edición - clave opcional
                txtClave.Enabled = false;
                txtConfirmarClave.Enabled = false;
            }
        }

        private async Task LoadReferenceData()
        {
            // Cargar familias
            var familiasCollection = await _usuarioService.GetFamiliasJerarquicasAsync();
            _familias = familiasCollection.ToList();

            // Cargar patentes
            var patentesCollection = await _usuarioService.GetAllPatentesAsync();
            _patentes = patentesCollection.ToList();

            // Llenar TreeView de familias
            LoadFamiliasTreeView();

            // Llenar CheckedListBox de patentes
            LoadPatentesCheckBox();
        }

        private void LoadFamiliasTreeView()
        {
            tvFamilias.Nodes.Clear();
            
            // Crear estructura jerárquica
            var familiaNodes = new Dictionary<Guid, TreeNode>();
            
            // Crear todos los nodos
            foreach (var familia in _familias)
            {
                var node = new TreeNode(familia.Nombre)
                {
                    Tag = familia,
                    Name = familia.IdFamilia.ToString()
                };
                familiaNodes[familia.IdFamilia] = node;
            }

            // Construir jerarquía
            foreach (var familia in _familias)
            {
                var node = familiaNodes[familia.IdFamilia];
                
                // Si no tiene padres, es nodo raíz
                bool esRaiz = true;
                foreach (var otraFamilia in _familias)
                {
                    if (otraFamilia.FamiliasHijas.Contains(familia.IdFamilia))
                    {
                        esRaiz = false;
                        familiaNodes[otraFamilia.IdFamilia].Nodes.Add(node);
                        break;
                    }
                }
                
                if (esRaiz)
                {
                    tvFamilias.Nodes.Add(node);
                }
            }

            tvFamilias.ExpandAll();
        }

        private void LoadPatentesCheckBox()
        {
            clbPatentes.Items.Clear();
            
            foreach (var patente in _patentes.OrderBy(p => p.Nombre))
            {
                clbPatentes.Items.Add(new PatenteCheckListItem 
                { 
                    Patente = patente, 
                    DisplayText = $"{patente.Nombre} ({patente.Vista})" 
                });
            }
        }

        private async Task LoadUserData()
        {
            // Cargar datos del usuario
            _usuario = await _usuarioService.GetUserForEditAsync(_usuario.Id);

            // Llenar campos básicos
            txtNombre.Text = _usuario.Nombre;
            txtUsuario.Text = _usuario.Usuario;

            // Marcar familias asignadas
            MarkAssignedFamilias();
            
            // Marcar patentes asignadas
            MarkAssignedPatentes();

            // Cargar permisos efectivos
            await LoadEffectivePermissions();
        }

        private void MarkAssignedFamilias()
        {
            foreach (TreeNode node in GetAllTreeNodes(tvFamilias.Nodes))
            {
                var familia = (FamiliaDto)node.Tag;
                node.Checked = _usuario.FamiliasAsignadas.Contains(familia.IdFamilia);
            }
        }

        private void MarkAssignedPatentes()
        {
            for (int i = 0; i < clbPatentes.Items.Count; i++)
            {
                var item = (PatenteCheckListItem)clbPatentes.Items[i];
                clbPatentes.SetItemChecked(i, _usuario.PatentesAsignadas.Contains(item.Patente.IdPatente));
            }
        }

        private async Task LoadEffectivePermissions()
        {
            if (_usuario.Id == Guid.Empty) return; // No calcular para usuario nuevo

            try
            {
                _permisosEfectivos = await _usuarioService.GetEffectivePermissionsAsync(_usuario.Id);
                DisplayEffectivePermissions();
            }
            catch
            {
                // Si falla, limpiar la vista
                lvPermisosEfectivos.Items.Clear();
            }
        }

        private void DisplayEffectivePermissions()
        {
            lvPermisosEfectivos.Items.Clear();

            // Agregar patentes directas
            foreach (var patente in _permisosEfectivos.PatentesDirectas)
            {
                var item = new ListViewItem(new[] { "Patente", patente.Nombre, "Asignación Directa" })
                {
                    Tag = patente,
                    ForeColor = Color.DarkBlue
                };
                lvPermisosEfectivos.Items.Add(item);
            }

            // Agregar patentes heredadas
            foreach (var patenteHeredada in _permisosEfectivos.PatentesHeredadas)
            {
                var item = new ListViewItem(new[] { 
                    "Patente", 
                    patenteHeredada.Patente.Nombre, 
                    $"Heredada de: {patenteHeredada.OrigenFamilia}" 
                })
                {
                    Tag = patenteHeredada,
                    ForeColor = Color.DarkGreen
                };
                lvPermisosEfectivos.Items.Add(item);
            }

            // Agregar familias
            foreach (var familia in _permisosEfectivos.FamiliasAsignadas)
            {
                var item = new ListViewItem(new[] { "Familia", familia.Nombre, "Asignación Directa" })
                {
                    Tag = familia,
                    ForeColor = Color.DarkRed
                };
                lvPermisosEfectivos.Items.Add(item);
            }
        }

        private void ConfigureControlsForMode()
        {
            if (!_modoEdicion)
            {
                // Nuevo usuario - deshabilitar tab de permisos efectivos
                tabPermisosEfectivos.Enabled = false;
                btnActualizarPermisos.Enabled = false;
            }
        }

        private IEnumerable<TreeNode> GetAllTreeNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                yield return node;
                foreach (var childNode in GetAllTreeNodes(node.Nodes))
                {
                    yield return childNode;
                }
            }
        }

        #region Event Handlers

        private void chkCambiarClave_CheckedChanged(object sender, EventArgs e)
        {
            bool habilitarClave = chkCambiarClave.Checked;
            txtClave.Enabled = habilitarClave;
            txtConfirmarClave.Enabled = habilitarClave;
            
            if (!habilitarClave)
            {
                txtClave.Clear();
                txtConfirmarClave.Clear();
                errorProvider.SetError(txtClave, "");
                errorProvider.SetError(txtConfirmarClave, "");
            }
        }

        private void tvFamilias_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (_isLoading) return;

            // Evitar recursión
            _isLoading = true;
            
            // Si se selecciona un nodo padre, seleccionar todos los hijos
            if (e.Node.Checked)
            {
                CheckAllChildNodes(e.Node, true);
            }
            else
            {
                CheckAllChildNodes(e.Node, false);
                // Si se deselecciona, también deseleccionar padres
                CheckParentNode(e.Node);
            }
            
            _isLoading = false;
        }

        private void CheckAllChildNodes(TreeNode node, bool check)
        {
            foreach (TreeNode childNode in node.Nodes)
            {
                childNode.Checked = check;
                CheckAllChildNodes(childNode, check);
            }
        }

        private void CheckParentNode(TreeNode node)
        {
            if (node.Parent != null)
            {
                bool anyChildChecked = node.Parent.Nodes.Cast<TreeNode>().Any(n => n.Checked);
                if (!anyChildChecked)
                {
                    node.Parent.Checked = false;
                    CheckParentNode(node.Parent);
                }
            }
        }

        private async void btnActualizarPermisos_Click(object sender, EventArgs e)
        {
            if (_usuario.Id == Guid.Empty) return;

            try
            {
                lblStatus.Text = "Actualizando permisos efectivos...";
                await LoadEffectivePermissions();
                lblStatus.Text = "Permisos actualizados";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error al actualizar permisos";
                MessageBox.Show($"Error al actualizar permisos: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                lblStatus.Text = "Guardando usuario...";
                
                // Preparar datos para guardar
                PrepareUserData();

                if (_modoEdicion)
                {
                    await _usuarioService.UpdateUserWithRelationsAsync(_usuario);
                    MessageBox.Show("Usuario actualizado exitosamente.", "Información", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    await _usuarioService.CreateUserWithRelationsAsync(_usuario);
                    MessageBox.Show("Usuario creado exitosamente.", "Información", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                lblStatus.Text = "Usuario guardado exitosamente";
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (DomainValidationException ex)
            {
                lblStatus.Text = "Error de validación";
                MessageBox.Show(ex.Message, "Error de Validación", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (ConcurrencyException ex)
            {
                lblStatus.Text = "Error de concurrencia";
                MessageBox.Show(ex.Message, "Error de Concurrencia", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.Retry;
                this.Close();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error al guardar usuario";
                MessageBox.Show($"Error al guardar el usuario: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Validation

        private bool ValidateForm()
        {
            bool isValid = true;
            errorProvider.Clear();

            // Validar nombre
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                errorProvider.SetError(txtNombre, "El nombre es obligatorio");
                isValid = false;
            }

            // Validar usuario
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                errorProvider.SetError(txtUsuario, "El usuario es obligatorio");
                isValid = false;
            }

            // Validar clave
            if (!ValidatePassword())
            {
                isValid = false;
            }

            // Validar permisos
            if (!ValidatePermissions())
            {
                isValid = false;
            }

            return isValid;
        }

        private bool ValidatePassword()
        {
            // Para usuario nuevo, la clave es obligatoria
            if (!_modoEdicion)
            {
                if (string.IsNullOrWhiteSpace(txtClave.Text))
                {
                    errorProvider.SetError(txtClave, "La clave es obligatoria");
                    return false;
                }
                
                if (txtClave.Text.Length < 6)
                {
                    errorProvider.SetError(txtClave, "La clave debe tener al menos 6 caracteres");
                    return false;
                }
            }
            else if (chkCambiarClave.Checked)
            {
                // En edición, solo validar si se quiere cambiar la clave
                if (string.IsNullOrWhiteSpace(txtClave.Text))
                {
                    errorProvider.SetError(txtClave, "La clave es obligatoria si desea cambiarla");
                    return false;
                }
                
                if (txtClave.Text.Length < 6)
                {
                    errorProvider.SetError(txtClave, "La clave debe tener al menos 6 caracteres");
                    return false;
                }
            }

            // Validar confirmación de clave
            if (!string.IsNullOrEmpty(txtClave.Text) && txtClave.Text != txtConfirmarClave.Text)
            {
                errorProvider.SetError(txtConfirmarClave, "Las claves no coinciden");
                return false;
            }

            return true;
        }

        private bool ValidatePermissions()
        {
            // Verificar que tenga al menos una familia o patente asignada
            bool tieneFamilias = GetSelectedFamilias().Any();
            bool tienePatentes = GetSelectedPatentes().Any();

            if (!tieneFamilias && !tienePatentes)
            {
                MessageBox.Show("El usuario debe tener al menos una familia o patente asignada.", 
                    "Validación de Permisos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tabPermisos.SelectedIndex = 0; // Mostrar tab de familias
                return false;
            }

            return true;
        }

        private void PrepareUserData()
        {
            _usuario.Nombre = txtNombre.Text.Trim();
            _usuario.Usuario = txtUsuario.Text.Trim();
            
            // Clave
            if (!_modoEdicion || chkCambiarClave.Checked)
            {
                _usuario.Clave = txtClave.Text;
                _usuario.ConfirmarClave = txtConfirmarClave.Text;
                _usuario.CambiarClave = _modoEdicion && chkCambiarClave.Checked;
            }

            // Asignaciones
            _usuario.FamiliasAsignadas = GetSelectedFamilias();
            _usuario.PatentesAsignadas = GetSelectedPatentes();
        }

        private List<Guid> GetSelectedFamilias()
        {
            var selectedFamilias = new List<Guid>();
            
            foreach (TreeNode node in GetAllTreeNodes(tvFamilias.Nodes))
            {
                if (node.Checked)
                {
                    var familia = (FamiliaDto)node.Tag;
                    selectedFamilias.Add(familia.IdFamilia);
                }
            }
            
            return selectedFamilias;
        }

        private List<Guid> GetSelectedPatentes()
        {
            var selectedPatentes = new List<Guid>();
            
            for (int i = 0; i < clbPatentes.Items.Count; i++)
            {
                if (clbPatentes.GetItemChecked(i))
                {
                    var item = (PatenteCheckListItem)clbPatentes.Items[i];
                    selectedPatentes.Add(item.Patente.IdPatente);
                }
            }
            
            return selectedPatentes;
        }

        #endregion

        #region Field Validation Events

        private void txtNombre_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                errorProvider.SetError(txtNombre, "El nombre es obligatorio");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(txtNombre, "");
            }
        }

        private void txtUsuario_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                errorProvider.SetError(txtUsuario, "El usuario es obligatorio");
                e.Cancel = true;
            }
            else if (txtUsuario.Text.Length < 3)
            {
                errorProvider.SetError(txtUsuario, "El usuario debe tener al menos 3 caracteres");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(txtUsuario, "");
            }
        }

        private void txtClave_Validating(object sender, CancelEventArgs e)
        {
            if (txtClave.Enabled && !string.IsNullOrEmpty(txtClave.Text) && txtClave.Text.Length < 6)
            {
                errorProvider.SetError(txtClave, "La clave debe tener al menos 6 caracteres");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(txtClave, "");
            }
        }

        private void txtConfirmarClave_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtClave.Text) && txtClave.Text != txtConfirmarClave.Text)
            {
                errorProvider.SetError(txtConfirmarClave, "Las claves no coinciden");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(txtConfirmarClave, "");
            }
        }

        #endregion
    }

    /// <summary>
    /// Clase auxiliar para mostrar patentes en CheckedListBox
    /// </summary>
    public class PatenteCheckListItem
    {
        public PatenteDto Patente { get; set; }
        public string DisplayText { get; set; }

        public override string ToString()
        {
            return DisplayText;
        }
    }
}