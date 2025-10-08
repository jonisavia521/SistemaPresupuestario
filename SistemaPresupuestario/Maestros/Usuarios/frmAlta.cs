using Services.DomainModel.Security.Composite;
using Services.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

public partial class frmAlta : Form
{
    private readonly IUsuarioService _usuarioService;
    private Usuario _usuarioActual; // null = modo agregar, con valor = modo editar
    private List<Familia> _familiasDisponibles;
    private List<Patente> _patentesDisponibles;

    public frmAlta(IUsuarioService usuarioService)
    {
        InitializeComponent();
        _usuarioService = usuarioService;
    }

    private void frmAlta_Load(object sender, EventArgs e)
    {
        try
        {
            this.Cursor = Cursors.WaitCursor;

            // Cargar familias y patentes disponibles
            _familiasDisponibles = _usuarioService.GetAllFamilias().ToList();
            _patentesDisponibles = _usuarioService.GetAllPatentes().ToList();

            // Cargar TreeView de familias (pestaña Familias)
            CargarArbolFamilias();

            // Cargar CheckedListBox de patentes (pestaña Patentes)
            CargarListaPatentes();

            // Si es edición, cargar datos
            if (_usuarioActual != null)
            {
                CargarDatosUsuario();
                this.Text = $"Editar Usuario - {_usuarioActual.Nombre}";
            }
            else
            {
                this.Text = "Nuevo Usuario";
            }
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }

    /// <summary>
    /// Método público para cargar usuario en modo edición
    /// </summary>
    public void CargarUsuario(Guid usuarioId)
    {
        _usuarioActual = _usuarioService.GetById(usuarioId);
    }

    private void CargarDatosUsuario()
    {
        txtNombre.Text = _usuarioActual.Nombre;
        txtUsuario.Text = _usuarioActual.User;
        txtClave.Text = ""; // No mostrar contraseña por seguridad
        txtClave.Enabled = false; // Deshabilitar edición de clave (o crear botón "Cambiar contraseña")

        // Marcar familias asignadas en TreeView
        MarcarFamiliasSeleccionadas();

        // Marcar patentes asignadas en CheckedListBox
        MarcarPatentesSeleccionadas();
    }

    private void CargarArbolFamilias()
    {
        treeViewFamilias.Nodes.Clear();
        treeViewFamilias.CheckBoxes = true;

        // Obtener familias raíz (puedes necesitar filtrar según jerarquía)
        var familiasRaiz = _familiasDisponibles; // Ajustar según tu lógica

        foreach (var familia in familiasRaiz)
        {
            var nodo = CrearNodoFamilia(familia);
            treeViewFamilias.Nodes.Add(nodo);
        }

        treeViewFamilias.ExpandAll();
    }

    private TreeNode CrearNodoFamilia(Familia familia)
    {
        var nodo = new TreeNode(familia.Nombre)
        {
            Tag = familia, // Guardar referencia a la familia
            Name = familia.IdComponent.ToString()
        };

        // Agregar hijos recursivamente
        foreach (var hijo in familia.GetChildrens().OfType<Familia>())
        {
            nodo.Nodes.Add(CrearNodoFamilia(hijo));
        }

        return nodo;
    }

    private void CargarListaPatentes()
    {
        checkedListBoxPatentes.Items.Clear();

        foreach (var patente in _patentesDisponibles)
        {
            checkedListBoxPatentes.Items.Add(
                new PatenteItem { Patente = patente },
                false
            );
        }
    }

    private void MarcarFamiliasSeleccionadas()
    {
        if (_usuarioActual == null) return;

        var familiasAsignadas = _usuarioActual.Permisos.OfType<Familia>().ToList();

        foreach (TreeNode nodo in treeViewFamilias.Nodes)
        {
            MarcarNodoRecursivo(nodo, familiasAsignadas);
        }
    }

    private void MarcarNodoRecursivo(TreeNode nodo, List<Familia> familiasAsignadas)
    {
        var familia = nodo.Tag as Familia;
        if (familia != null && familiasAsignadas.Any(f => f.IdComponent == familia.IdComponent))
        {
            nodo.Checked = true;
        }

        foreach (TreeNode hijo in nodo.Nodes)
        {
            MarcarNodoRecursivo(hijo, familiasAsignadas);
        }
    }

    private void MarcarPatentesSeleccionadas()
    {
        if (_usuarioActual == null) return;

        var patentesAsignadas = _usuarioActual.Permisos.OfType<Patente>().ToList();

        for (int i = 0; i < checkedListBoxPatentes.Items.Count; i++)
        {
            var item = (PatenteItem)checkedListBoxPatentes.Items[i];
            if (patentesAsignadas.Any(p => p.IdComponent == item.Patente.IdComponent))
            {
                checkedListBoxPatentes.SetItemChecked(i, true);
            }
        }
    }

    private void btnAceptar_Click(object sender, EventArgs e)
    {
        try
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es requerido", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                MessageBox.Show("El usuario es requerido", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsuario.Focus();
                return;
            }

            if (_usuarioActual == null && string.IsNullOrWhiteSpace(txtClave.Text))
            {
                MessageBox.Show("La contraseña es requerida", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtClave.Focus();
                return;
            }

            this.Cursor = Cursors.WaitCursor;

            if (_usuarioActual == null)
            {
                // MODO AGREGAR
                var nuevoUsuario = new Usuario
                {
                    Id = Guid.NewGuid(),
                    Nombre = txtNombre.Text.Trim(),
                    User = txtUsuario.Text.Trim(),
                    Password = txtClave.Text
                };

                // Agregar permisos seleccionados
                AgregarPermisosSeleccionados(nuevoUsuario);

                _usuarioService.Add(nuevoUsuario);
                _usuarioService.SavePermisos(nuevoUsuario);

                MessageBox.Show("Usuario creado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // MODO EDITAR
                _usuarioActual.Nombre = txtNombre.Text.Trim();
                _usuarioActual.User = txtUsuario.Text.Trim();

                // Solo actualizar contraseña si se ingresó una nueva
                if (!string.IsNullOrWhiteSpace(txtClave.Text))
                {
                    _usuarioActual.Password = txtClave.Text;
                }

                // Limpiar y agregar nuevos permisos
                _usuarioActual.Permisos.Clear();
                AgregarPermisosSeleccionados(_usuarioActual);

                _usuarioService.Update(_usuarioActual);
                _usuarioService.SavePermisos(_usuarioActual);

                MessageBox.Show("Usuario actualizado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al guardar usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }

    private void AgregarPermisosSeleccionados(Usuario usuario)
    {
        // Agregar familias seleccionadas
        foreach (TreeNode nodo in treeViewFamilias.Nodes)
        {
            AgregarFamiliasRecursivo(nodo, usuario);
        }

        // Agregar patentes seleccionadas
        foreach (var item in checkedListBoxPatentes.CheckedItems.Cast<PatenteItem>())
        {
            usuario.Permisos.Add(item.Patente);
        }
    }

    private void AgregarFamiliasRecursivo(TreeNode nodo, Usuario usuario)
    {
        if (nodo.Checked)
        {
            var familia = nodo.Tag as Familia;
            if (familia != null)
            {
                usuario.Permisos.Add(familia);
            }
        }

        foreach (TreeNode hijo in nodo.Nodes)
        {
            AgregarFamiliasRecursivo(hijo, usuario);
        }
    }

    private void btnCancelar_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    // Clase auxiliar para mostrar patentes en CheckedListBox
    private class PatenteItem
    {
        public Patente Patente { get; set; }

        public override string ToString()
        {
            return Patente.MenuItemName;
        }
    }
}