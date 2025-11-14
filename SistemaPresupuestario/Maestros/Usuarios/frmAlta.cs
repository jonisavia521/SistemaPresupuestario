using Services.DomainModel.Security.Composite;
using Services.Services.Contracts;
using SistemaPresupuestario.Helpers; // NUEVO
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
        
        // ✅ TRADUCCIÓN AUTOMÁTICA: Aplicar traducciones a TODOS los controles
        FormTranslator.Translate(this);
        
        // ✅ TRADUCCIÓN DINÁMICA: Suscribirse al evento de cambio de idioma
        I18n.LanguageChanged += OnLanguageChanged;
        this.FormClosed += (s, e) => I18n.LanguageChanged -= OnLanguageChanged;
    }
    
    /// <summary>
    /// Manejador del evento de cambio de idioma
    /// </summary>
    private void OnLanguageChanged(object sender, EventArgs e)
    {
        FormTranslator.Translate(this);
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
                this.Text = $"{I18n.T("Editar Usuario")} - {_usuarioActual.Nombre}";
            }
            else
            {
                this.Text = I18n.T("Nuevo Usuario");
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
        
        // ✅ CORRECCIÓN DE SEGURIDAD: En modo edición, ocultar el campo de contraseña
        // La contraseña NO se puede cambiar desde este formulario
        txtClave.Text = "";
        txtClave.Visible = false;
        
        // Si tienes un label para la contraseña, ocultarlo también
        // lblClave.Visible = false; // Descomenta si existe

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

        // ✅ CORRECCIÓN: Recorrer TODOS los hijos (familias Y patentes)
        // El patrón Composite dice que GetChildrens() retorna TODOS los componentes hijos
        foreach (var hijo in familia.GetChildrens())
        {
            // Verificar si es una Familia (tiene hijos) o una Patente (no tiene hijos)
            if (hijo.ChildrenCount() > 0)
            {
                // Es una Familia - agregar recursivamente
                var familiaHija = hijo as Familia;
                nodo.Nodes.Add(CrearNodoFamilia(familiaHija));
            }
            else
            {
                // Es una Patente - agregar como nodo hoja
                var patente = hijo as Patente;
                if (patente != null)
                {
                    var nodoPatente = new TreeNode($"[P] {patente.MenuItemName}")
                    {
                        Tag = patente, // Guardar referencia a la patente
                        Name = patente.IdComponent.ToString(),
                        // Opcional: cambiar el ícono o color para distinguir patentes de familias
                        ForeColor = System.Drawing.Color.Blue
                    };
                    nodo.Nodes.Add(nodoPatente);
                }
            }
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

        // ✅ CORRECCIÓN: Obtener familias Y patentes asignadas al usuario
        var familiasAsignadas = _usuarioActual.Permisos.OfType<Familia>().ToList();
        var patentesAsignadas = _usuarioActual.Permisos.OfType<Patente>().ToList();

        // Recorrer el árbol y marcar según corresponda
        foreach (TreeNode nodo in treeViewFamilias.Nodes)
        {
            MarcarNodoRecursivo(nodo, familiasAsignadas, patentesAsignadas);
        }
    }

    /// <summary>
    /// ✅ NUEVA LÓGICA: Marca los nodos basándose en familias Y patentes asignadas
    /// </summary>
    private void MarcarNodoRecursivo(TreeNode nodo, List<Familia> familiasAsignadas, List<Patente> patentesAsignadas)
    {
        var familia = nodo.Tag as Familia;
        var patente = nodo.Tag as Patente;

        if (familia != null)
        {
            // Es una Familia
            if (familiasAsignadas.Any(f => f.IdComponent == familia.IdComponent))
            {
                // Si la familia está asignada → marcar TODO el sub-árbol
                nodo.Checked = true;
                MarcarTodosLosHijosRecursivo(nodo);
            }
            else
            {
                // Si la familia NO está asignada → revisar sus hijos individualmente
                foreach (TreeNode hijo in nodo.Nodes)
                {
                    MarcarNodoRecursivo(hijo, familiasAsignadas, patentesAsignadas);
                }
            }
        }
        else if (patente != null)
        {
            // Es una Patente individual
            if (patentesAsignadas.Any(p => p.IdComponent == patente.IdComponent))
            {
                nodo.Checked = true;
            }
        }
    }

    /// <summary>
    /// Marca recursivamente todos los nodos hijos (familias y patentes)
    /// Se usa cuando una familia está asignada al usuario, lo que significa
    /// que el usuario tiene acceso a TODO lo que contiene esa familia
    /// </summary>
    private void MarcarTodosLosHijosRecursivo(TreeNode nodo)
    {
        foreach (TreeNode hijo in nodo.Nodes)
        {
            hijo.Checked = true;
            
            // Marcar recursivamente los hijos de este hijo
            MarcarTodosLosHijosRecursivo(hijo);
        }
    }

    private void MarcarPatentesSeleccionadas()
    {
        if (_usuarioActual == null) return;

        // ✅ CORRECCIÓN: Solo marcar patentes que están DIRECTAMENTE asignadas al usuario
        // NO las que vienen de familias (esas ya se marcan en el TreeView)
        var patentesDirectas = _usuarioActual.Permisos.OfType<Patente>().ToList();

        for (int i = 0; i < checkedListBoxPatentes.Items.Count; i++)
        {
            var item = (PatenteItem)checkedListBoxPatentes.Items[i];
            if (patentesDirectas.Any(p => p.IdComponent == item.Patente.IdComponent))
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
                MessageBox.Show(I18n.T("El nombre es requerido"), I18n.T("Validación"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                MessageBox.Show(I18n.T("El usuario es requerido"), I18n.T("Validación"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsuario.Focus();
                return;
            }

            // ✅ CORRECCIÓN: Solo validar contraseña en modo AGREGAR
            if (_usuarioActual == null && string.IsNullOrWhiteSpace(txtClave.Text))
            {
                MessageBox.Show(I18n.T("La contraseña es requerida"), I18n.T("Validación"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtClave.Focus();
                return;
            }

            this.Cursor = Cursors.WaitCursor;

            if (_usuarioActual == null)
            {
                // ============================================================
                // MODO AGREGAR - Crear nuevo usuario CON contraseña
                // ============================================================
                var nuevoUsuario = new Usuario
                {
                    Id = Guid.NewGuid(),
                    Nombre = txtNombre.Text.Trim(),
                    User = txtUsuario.Text.Trim(),
                    Password = txtClave.Text // La contraseña será hasheada en el constructor
                };

                // Agregar permisos seleccionados
                AgregarPermisosSeleccionados(nuevoUsuario);

                _usuarioService.Add(nuevoUsuario);
                _usuarioService.SavePermisos(nuevoUsuario);

                MessageBox.Show(I18n.T("Usuario creado exitosamente"), I18n.T("Éxito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // ============================================================
                // MODO EDITAR - Actualizar SOLO nombre, usuario y permisos
                // ⚠️ NUNCA actualizar la contraseña desde este formulario
                // ============================================================
                _usuarioActual.Nombre = txtNombre.Text.Trim();
                _usuarioActual.User = txtUsuario.Text.Trim();

                // ✅ NO tocar _usuarioActual.Password
                // La contraseña NO se modifica desde este formulario

                // Limpiar y agregar nuevos permisos
                _usuarioActual.Permisos.Clear();
                AgregarPermisosSeleccionados(_usuarioActual);

                // ✅ El método Update ahora solo actualiza Nombre y Usuario (no la Clave)
                _usuarioService.Update(_usuarioActual);
                _usuarioService.SavePermisos(_usuarioActual);

                MessageBox.Show(I18n.T("Usuario actualizado exitosamente"), I18n.T("Éxito"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{I18n.T("Error al guardar usuario")}: {ex.Message}", I18n.T("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            this.Cursor = Cursors.Default;
        }
    }

    /// <summary>
    /// ✅ NUEVA LÓGICA CORRECTA: 
    /// Recorre TODO el árbol y agrega:
    /// 1. Familias marcadas (sin recorrer sus hijos porque la familia ya los incluye)
    /// 2. Patentes individuales marcadas (solo si su familia padre NO está marcada)
    /// </summary>
    private void AgregarPermisosSeleccionados(Usuario usuario)
    {
        // Primero: Identificar qué familias están marcadas
        var familiasSeleccionadas = new List<Guid>();
        IdentificarFamiliasSeleccionadas(treeViewFamilias.Nodes, familiasSeleccionadas);

        // Segundo: Agregar familias marcadas
        foreach (TreeNode nodo in treeViewFamilias.Nodes)
        {
            AgregarFamiliasRecursivo(nodo, usuario, familiasSeleccionadas);
        }

        // Tercero: Agregar patentes del TreeView que NO estén dentro de familias marcadas
        foreach (TreeNode nodo in treeViewFamilias.Nodes)
        {
            AgregarPatentesIndividualesRecursivo(nodo, usuario, familiasSeleccionadas);
        }

        // Cuarto: Agregar patentes DIRECTAS del CheckedListBox
        foreach (var item in checkedListBoxPatentes.CheckedItems.Cast<PatenteItem>())
        {
            // Evitar duplicados
            if (!usuario.Permisos.OfType<Patente>().Any(p => p.IdComponent == item.Patente.IdComponent))
            {
                usuario.Permisos.Add(item.Patente);
            }
        }
    }

    /// <summary>
    /// Identifica todas las familias que están marcadas para después filtrar patentes individuales
    /// </summary>
    private void IdentificarFamiliasSeleccionadas(TreeNodeCollection nodos, List<Guid> familiasSeleccionadas)
    {
        foreach (TreeNode nodo in nodos)
        {
            var familia = nodo.Tag as Familia;
            
            if (familia != null && nodo.Checked)
            {
                familiasSeleccionadas.Add(familia.IdComponent);
                // No seguir recorriendo hijos porque si la familia está marcada,
                // todas sus sub-familias también cuentan como "dentro de familia marcada"
            }
            else
            {
                // Solo seguir recorriendo si esta familia NO está marcada
                IdentificarFamiliasSeleccionadas(nodo.Nodes, familiasSeleccionadas);
            }
        }
    }

    /// <summary>
    /// Agrega solo las familias marcadas (sin sus hijos porque el patrón Composite los incluye automáticamente)
    /// </summary>
    private void AgregarFamiliasRecursivo(TreeNode nodo, Usuario usuario, List<Guid> familiasSeleccionadas)
    {
        var familia = nodo.Tag as Familia;
        
        if (familia != null && nodo.Checked)
        {
            // Agregar la familia completa
            usuario.Permisos.Add(familia);
            // ⚠️ NO recorrer hijos porque la familia ya los contiene (Composite pattern)
            return;
        }

        // Si NO está marcada, seguir buscando en sus hijos
        foreach (TreeNode hijo in nodo.Nodes)
        {
            AgregarFamiliasRecursivo(hijo, usuario, familiasSeleccionadas);
        }
    }

    /// <summary>
    /// ✅ NUEVA LÓGICA: Agrega solo las PATENTES individuales marcadas
    /// que NO están dentro de una familia marcada
    /// </summary>
    private void AgregarPatentesIndividualesRecursivo(TreeNode nodo, Usuario usuario, List<Guid> familiasSeleccionadas)
    {
        var familia = nodo.Tag as Familia;
        var patente = nodo.Tag as Patente;

        if (familia != null)
        {
            // Si es una familia marcada, NO procesar sus hijos (ya se agregaron con la familia)
            if (familiasSeleccionadas.Contains(familia.IdComponent))
            {
                return;
            }

            // Si la familia NO está marcada, revisar sus hijos
            foreach (TreeNode hijo in nodo.Nodes)
            {
                AgregarPatentesIndividualesRecursivo(hijo, usuario, familiasSeleccionadas);
            }
        }
        else if (patente != null && nodo.Checked)
        {
            // Es una patente marcada → verificar que NO esté en familia marcada
            if (!EstaEnFamiliaMarcada(nodo, familiasSeleccionadas))
            {
                // Evitar duplicados
                if (!usuario.Permisos.OfType<Patente>().Any(p => p.IdComponent == patente.IdComponent))
                {
                    usuario.Permisos.Add(patente);
                }
            }
        }
    }

    /// <summary>
    /// Verifica si un nodo está dentro de una familia marcada (recorriendo hacia arriba)
    /// </summary>
    private bool EstaEnFamiliaMarcada(TreeNode nodo, List<Guid> familiasSeleccionadas)
    {
        TreeNode padre = nodo.Parent;
        while (padre != null)
        {
            var familiaPadre = padre.Tag as Familia;
            if (familiaPadre != null && familiasSeleccionadas.Contains(familiaPadre.IdComponent))
            {
                return true;
            }
            padre = padre.Parent;
        }
        return false;
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