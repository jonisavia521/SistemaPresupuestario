using BLL.Contracts;
using BLL.DTOs;
using SistemaPresupuestario.Maestros.Shared;
using SistemaPresupuestario.Helpers; // NUEVO
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros.ListaPrecio
{
    public partial class frmListaPrecioAlta : Form
    {
        private readonly IListaPrecioService _listaPrecioService;
        private readonly IProductoService _productoService;
        private ListaPrecioDTO _listaPrecioActual;
        private BindingList<ListaPrecioDetalleDTO> _detalles;

        // Propiedad pública para verificar si el formulario está editando una lista específica
        public Guid? ListaPrecioId => _listaPrecioActual?.Id;

        public frmListaPrecioAlta(
            IListaPrecioService listaPrecioService,
            IProductoService productoService)
        {
            InitializeComponent();
            _listaPrecioService = listaPrecioService ?? throw new ArgumentNullException(nameof(listaPrecioService));
            _productoService = productoService ?? throw new ArgumentNullException(nameof(productoService));
            _detalles = new BindingList<ListaPrecioDetalleDTO>();
            
            // ? TRADUCCIÓN AUTOMÁTICA
            FormTranslator.Translate(this);
            
            // ? TRADUCCIÓN DINÁMICA
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

        private void frmListaPrecioAlta_Load(object sender, EventArgs e)
        {
            ConfigurarGrilla();

            if (_listaPrecioActual != null)
            {
                // Modo edición
                CargarDatos();
                this.Text = $"Editar Lista de Precios - {_listaPrecioActual.Nombre}";
            }
            else
            {
                // Modo nuevo
                this.Text = "Nueva Lista de Precios";
            }
        }

        public void CargarListaPrecio(Guid id)
        {
            _listaPrecioActual = _listaPrecioService.GetById(id);
        }

        private void ConfigurarGrilla()
        {
            dgvDetalles.AutoGenerateColumns = false;
            dgvDetalles.AllowUserToAddRows = true;

            // IMPORTANTE: Limpiar el DataSource antes de configurar
            dgvDetalles.DataSource = null;

            // Configurar las columnas manualmente
            dgvDetalles.Columns.Clear();

            // Columna IdProducto (oculta)
            var colIdProducto = new DataGridViewTextBoxColumn
            {
                Name = "IdProducto",
                HeaderText = "ID Producto",
                DataPropertyName = "IdProducto",
                Visible = false
            };
            dgvDetalles.Columns.Add(colIdProducto);

            // Columna Código
            var colCodigo = new DataGridViewTextBoxColumn
            {
                Name = "Codigo",
                HeaderText = "Código",
                DataPropertyName = "Codigo",
                Width = 120,
                ValueType = typeof(string)
            };
            dgvDetalles.Columns.Add(colCodigo);

            // Columna Descripción (ReadOnly)
            var colDescripcion = new DataGridViewTextBoxColumn
            {
                Name = "Descripcion",
                HeaderText = "Descripción",
                DataPropertyName = "Descripcion",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                ReadOnly = true,
                ValueType = typeof(string)
            };
            dgvDetalles.Columns.Add(colDescripcion);

            // Columna Precio
            var colPrecio = new DataGridViewTextBoxColumn
            {
                Name = "Precio",
                HeaderText = "Precio",
                DataPropertyName = "Precio",
                Width = 120,
                ValueType = typeof(decimal),
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2",
                    NullValue = "0.00"
                }
            };
            dgvDetalles.Columns.Add(colPrecio);

            // Ahora asignar el DataSource
            dgvDetalles.DataSource = _detalles;

            // Configurar eventos de la grilla
            dgvDetalles.CellEndEdit += DgvDetalles_CellEndEdit;
            dgvDetalles.CellValidating += DgvDetalles_CellValidating;
            dgvDetalles.UserDeletingRow += DgvDetalles_UserDeletingRow;
            dgvDetalles.KeyDown += DgvDetalles_KeyDown;
            dgvDetalles.EditingControlShowing += DgvDetalles_EditingControlShowing;
            dgvDetalles.DefaultValuesNeeded += DgvDetalles_DefaultValuesNeeded;
            dgvDetalles.DataError += DgvDetalles_DataError;
            dgvDetalles.CellBeginEdit += DgvDetalles_CellBeginEdit;
        }

        private void CargarDatos()
        {
            txtCodigo.Text = _listaPrecioActual.Codigo;
            txtNombre.Text = _listaPrecioActual.Nombre;
            chkActivo.Checked = _listaPrecioActual.Activo;
            chkIncluyeIva.Checked = _listaPrecioActual.IncluyeIva;

            // Cargar detalles en el BindingList
            _detalles.Clear();
            if (_listaPrecioActual.Detalles != null)
            {
                foreach (var detalle in _listaPrecioActual.Detalles)
                {
                    _detalles.Add(detalle);
                }
            }
        }

        private void DgvDetalles_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Al comenzar a editar, seleccionar todo el texto
            if (e.ColumnIndex == 1 && e.RowIndex >= 0) // Columna de código (segunda columna visible)
            {
                dgvDetalles.BeginInvoke((Action)(() =>
                {
                    if (dgvDetalles.EditingControl is TextBox textBox)
                    {
                        textBox.SelectAll();
                    }
                }));
            }
        }

        private void DgvDetalles_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvDetalles.CurrentCell.ColumnIndex == 2) // Columna de descripción
            {
                // Solo lectura - deshabilitar el control de edición
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.ReadOnly = true;
                }
            }
            else
            {
                // Permitir edición
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.ReadOnly = false;
                }
            }
        }

        private void DgvDetalles_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Validar que el índice de la fila sea válido
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                    return;

                // Verificar que no sea la fila de nuevos elementos
                if (dgvDetalles.Rows[e.RowIndex].IsNewRow)
                    return;

                // Verificar que el índice esté dentro del rango de la lista _detalles
                if (e.RowIndex >= _detalles.Count)
                    return;

                var detalle = _detalles[e.RowIndex];

                // Si es la columna de código, buscar el producto
                if (e.ColumnIndex == 1 && dgvDetalles.Rows[e.RowIndex].Cells[1].Value != null) // Columna Codigo
                {
                    var codigo = dgvDetalles.Rows[e.RowIndex].Cells[1].Value.ToString();
                    if (!string.IsNullOrWhiteSpace(codigo))
                    {
                        BuscarYAplicarProductoPorCodigoSync(codigo);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar detalle: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvDetalles_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Validar valores numéricos en columna de precio
            if (e.ColumnIndex == dgvDetalles.Columns["Precio"].Index)
            {
                if (!string.IsNullOrWhiteSpace(e.FormattedValue?.ToString()))
                {
                    decimal valor;
                    if (!decimal.TryParse(e.FormattedValue.ToString(), out valor) || valor < 0)
                    {
                        MessageBox.Show("Debe ingresar un valor numérico válido mayor o igual a cero.",
                            "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Cancel = true;
                    }
                }
            }
        }

        private void DgvDetalles_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            var result = MessageBox.Show("¿Está seguro de eliminar este producto?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void DgvDetalles_KeyDown(object sender, KeyEventArgs e)
        {
            // Permitir eliminar fila con tecla Delete
            if (e.KeyCode == Keys.Delete && !dgvDetalles.CurrentRow.IsNewRow)
            {
                var result = MessageBox.Show("¿Está seguro de eliminar este producto?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    dgvDetalles.Rows.Remove(dgvDetalles.CurrentRow);
                }

                e.Handled = true;
            }
        }

        private void DgvDetalles_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            // Establecer valores por defecto
            e.Row.Cells["Codigo"].Value = "";
            e.Row.Cells["Descripcion"].Value = "";
            e.Row.Cells["Precio"].Value = 0M;
        }

        private void DgvDetalles_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Prevenir que se muestre el error por defecto
            e.ThrowException = false;
            e.Cancel = true;

            // Mostrar mensaje al usuario solo si no es un error de conversión temporal
            if (e.Context != DataGridViewDataErrorContexts.Parsing)
            {
                var columnName = dgvDetalles.Columns[e.ColumnIndex]?.Name ?? "Desconocida";
                MessageBox.Show($"Error en la columna '{columnName}' (fila {e.RowIndex + 1}):\n{e.Exception.Message}",
                    "Error de datos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Busca un producto por código y lo aplica a la fila actual (versión síncrona)
        /// </summary>
        private void BuscarYAplicarProductoPorCodigoSync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return;

            try
            {
                // Validar formato de código
                if (codigo.Length < 3)
                {
                    MessageBox.Show("El código del producto debe tener al menos 3 caracteres", "Advertencia",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Buscar producto por código
                var producto = _productoService.GetByCodigo(codigo.Trim());
                if (producto != null)
                {
                    // Aplicar datos del producto a la fila actual
                    if (dgvDetalles.CurrentRow != null && dgvDetalles.CurrentRow.Index < _detalles.Count)
                    {
                        var detalle = _detalles[dgvDetalles.CurrentRow.Index];

                        // Establecer el IdProducto (campo requerido para guardar)
                        detalle.IdProducto = producto.Id;
                        detalle.Codigo = producto.Codigo;
                        detalle.Descripcion = producto.Descripcion;

                        // Mantener el precio si ya tiene, o dejar en 0
                        if (detalle.Precio == 0M)
                        {
                            detalle.Precio = 0M;
                        }

                        // Refrescar la fila para mostrar los cambios
                        dgvDetalles.Refresh();

                        // Seleccionar siguiente celda (Precio)
                        if (dgvDetalles.Columns.Contains("Precio"))
                        {
                            dgvDetalles.CurrentCell = dgvDetalles.CurrentRow.Cells["Precio"];
                            dgvDetalles.BeginEdit(true);
                        }
                    }
                }
                else
                {
                    // Si no se encuentra, limpiar campos relevantes
                    if (dgvDetalles.CurrentRow != null && dgvDetalles.CurrentRow.Index < _detalles.Count)
                    {
                        var detalle = _detalles[dgvDetalles.CurrentRow.Index];
                        detalle.IdProducto = Guid.Empty;
                        detalle.Codigo = null;
                        detalle.Descripcion = null;

                        dgvDetalles.Refresh();
                    }

                    MessageBox.Show("Producto no encontrado", "Advertencia",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar producto: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Busca un producto por código y retorna true si lo encuentra
        /// Usado específicamente para el manejo de Enter en la grilla
        /// </summary>
        private bool BuscarYAplicarProductoPorCodigoEnter(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;

            try
            {
                // Validar formato de código
                if (codigo.Length < 3)
                {
                    return false; // No es válido, se mostrará selector
                }

                // Buscar producto por código (método síncrono)
                var producto = _productoService.GetByCodigo(codigo.Trim());

                if (producto != null)
                {
                    // Aplicar datos del producto a la fila actual
                    if (dgvDetalles.CurrentRow != null && dgvDetalles.CurrentRow.Index < _detalles.Count)
                    {
                        var detalle = _detalles[dgvDetalles.CurrentRow.Index];

                        // Establecer el IdProducto (campo requerido para guardar)
                        detalle.IdProducto = producto.Id;
                        detalle.Codigo = producto.Codigo;
                        detalle.Descripcion = producto.Descripcion;

                        // Refrescar la fila
                        dgvDetalles.Refresh();
                    }

                    return true; // Producto encontrado
                }

                return false; // Producto no encontrado
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar producto: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Muestra el selector de productos y aplica el seleccionado a la fila actual
        /// </summary>
        private void MostrarSelectorProducto()
        {
            try
            {
                // Obtener todos los productos activos
                var productos = _productoService.GetActivos();
                var listaProductos = productos.ToList();

                if (!listaProductos.Any())
                {
                    MessageBox.Show("No hay productos disponibles", "Advertencia",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Configurar el selector de productos
                var config = new SelectorConfig<ProductoDTO>
                {
                    Titulo = "Seleccionar Producto",
                    Datos = listaProductos,
                    PlaceholderBusqueda = "Buscar por código o descripción...",
                    PermitirSeleccionMultiple = false,
                    Columnas = new List<ColumnaConfig>
                    {
                        new ColumnaConfig
                        {
                            NombrePropiedad = "Codigo",
                            TituloColumna = "Código",
                            Ancho = 120,
                            Visible = true
                        },
                        new ColumnaConfig
                        {
                            NombrePropiedad = "Descripcion",
                            TituloColumna = "Descripción",
                            Ancho = 400,
                            Visible = true
                        },
                        new ColumnaConfig
                        {
                            NombrePropiedad = "IVATexto",
                            TituloColumna = "IVA",
                            Ancho = 100,
                            Visible = true
                        }
                    },
                    FuncionFiltro = (busqueda, producto) =>
                    {
                        var textoBusqueda = busqueda.ToUpper();
                        return (producto.Codigo != null && producto.Codigo.ToUpper().Contains(textoBusqueda)) ||
                               (producto.Descripcion != null && producto.Descripcion.ToUpper().Contains(textoBusqueda));
                    }
                };

                // Mostrar el selector
                var selector = frmSelector.Mostrar(config);

                if (selector.ShowDialog() == DialogResult.OK)
                {
                    var productoSeleccionado = selector.ElementoSeleccionado as ProductoDTO;

                    if (productoSeleccionado != null && dgvDetalles.CurrentRow != null)
                    {
                        // Verificar si la fila actual pertenece a _detalles
                        int rowIndex = dgvDetalles.CurrentRow.Index;

                        if (rowIndex < _detalles.Count)
                        {
                            // Fila existente en _detalles
                            var detalle = _detalles[rowIndex];

                            // Establecer el IdProducto (campo requerido para guardar)
                            detalle.IdProducto = productoSeleccionado.Id;
                            detalle.Codigo = productoSeleccionado.Codigo;
                            detalle.Descripcion = productoSeleccionado.Descripcion;

                            // Mantener el precio si ya tiene
                            if (detalle.Precio == 0M)
                            {
                                detalle.Precio = 0M;
                            }
                        }
                        else
                        {
                            // Fila nueva (la fila de agregar del DataGridView)
                            var nuevoDetalle = new ListaPrecioDetalleDTO
                            {
                                IdProducto = productoSeleccionado.Id,
                                Codigo = productoSeleccionado.Codigo,
                                Descripcion = productoSeleccionado.Descripcion,
                                Precio = 0M
                            };

                            _detalles.Add(nuevoDetalle);
                        }

                        // Refrescar la grilla
                        dgvDetalles.Refresh();

                        // Mover a la columna de precio
                        int row = dgvDetalles.CurrentRow.Index;
                        dgvDetalles.CurrentCell = dgvDetalles["Precio", row];
                        dgvDetalles.BeginEdit(true);
                    }
                }
                else
                {
                    // Usuario canceló - limpiar la fila si está vacía
                    if (dgvDetalles.CurrentRow != null && dgvDetalles.CurrentRow.Index < _detalles.Count)
                    {
                        var detalle = _detalles[dgvDetalles.CurrentRow.Index];
                        var codigo = detalle.Codigo;

                        if (string.IsNullOrWhiteSpace(codigo))
                        {
                            detalle.IdProducto = Guid.Empty;
                            detalle.Descripcion = null;
                            dgvDetalles.Refresh();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al mostrar selector de productos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Obtiene solo los detalles válidos de la grilla, filtrando filas vacías
        /// </summary>
        private List<ListaPrecioDetalleDTO> ObtenerDetallesValidos()
        {
            var detallesValidos = new List<ListaPrecioDetalleDTO>();

            foreach (var detalle in _detalles)
            {
                // Validar que el detalle tenga un código de producto válido
                if (!string.IsNullOrWhiteSpace(detalle.Codigo) &&
                    detalle.IdProducto != Guid.Empty)
                {
                    // Validar que tenga precio válido
                    if (detalle.Precio >= 0)
                    {
                        detallesValidos.Add(detalle);
                    }
                }
            }

            return detallesValidos;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                // IMPORTANTE: Finalizar edición de la grilla para confirmar cambios pendientes
                dgvDetalles.EndEdit();

                // Validaciones
                if (string.IsNullOrWhiteSpace(txtCodigo.Text))
                {
                    MessageBox.Show("El código es requerido", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCodigo.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    MessageBox.Show("El nombre es requerido", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNombre.Focus();
                    return;
                }

                // Obtener solo los detalles válidos
                var detallesValidos = ObtenerDetallesValidos();

                if (detallesValidos.Count == 0)
                {
                    MessageBox.Show("Debe agregar al menos un producto a la lista de precios", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgvDetalles.Focus();
                    return;
                }

                this.Cursor = Cursors.WaitCursor;

                var dto = new ListaPrecioDTO
                {
                    Codigo = txtCodigo.Text.Trim(),
                    Nombre = txtNombre.Text.Trim(),
                    Activo = chkActivo.Checked,
                    IncluyeIva = chkIncluyeIva.Checked,
                    Detalles = detallesValidos
                };

                if (_listaPrecioActual == null)
                {
                    // Modo nuevo
                    _listaPrecioService.Add(dto);
                    MessageBox.Show("Lista de precios creada exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Modo edición
                    dto.Id = _listaPrecioActual.Id;
                    _listaPrecioService.Update(dto);
                    MessageBox.Show("Lista de precios actualizada exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar lista de precios: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        /// <summary>
        /// Sobreescribe ProcessCmdKey para manejar Enter como Tab en la grilla
        /// Incluye validación de campos y apertura de selector cuando sea necesario
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Manejar Enter en DataGridView (solo columna Codigo)
            if ((dgvDetalles.Focused || dgvDetalles.EditingControl != null) && keyData == Keys.Enter)
            {
                // Solo procesar si estamos en la columna de código
                if (dgvDetalles.CurrentCell != null && dgvDetalles.CurrentCell.ColumnIndex == 1) // Columna Codigo
                {
                    // Finalizar edición actual
                    dgvDetalles.EndEdit();

                    // Obtener el código ingresado
                    var codigo = dgvDetalles.CurrentCell.Value?.ToString()?.Trim();

                    if (!string.IsNullOrWhiteSpace(codigo))
                    {
                        // Validar y buscar producto
                        bool productoEncontrado = BuscarYAplicarProductoPorCodigoEnter(codigo);

                        if (productoEncontrado)
                        {
                            // Mover a la siguiente celda (Precio)
                            int col = dgvDetalles.CurrentCell.ColumnIndex;
                            int row = dgvDetalles.CurrentCell.RowIndex;

                            if (col < dgvDetalles.ColumnCount - 1)
                            {
                                dgvDetalles.CurrentCell = dgvDetalles[col + 1, row];
                            }
                        }
                        else
                        {
                            // Mostrar selector de productos
                            MostrarSelectorProducto();
                        }
                    }
                    else
                    {
                        // Campo vacío - mostrar selector directamente
                        MostrarSelectorProducto();
                    }

                    return true; // Indicamos que manejamos la tecla
                }
                else
                {
                    // Para otras columnas, comportamiento normal de navegación
                    dgvDetalles.EndEdit();

                    int col = dgvDetalles.CurrentCell.ColumnIndex;
                    int row = dgvDetalles.CurrentCell.RowIndex;

                    // Si no es la última columna
                    if (col < dgvDetalles.ColumnCount - 1)
                    {
                        dgvDetalles.CurrentCell = dgvDetalles[col + 1, row];
                    }
                    // Si es la última columna y no es la última fila
                    else if (row < dgvDetalles.RowCount - 1)
                    {
                        dgvDetalles.CurrentCell = dgvDetalles[1, row + 1]; // Columna Codigo de la siguiente fila
                    }

                    return true;
                }
            }

            // Manejar Enter en otros controles editables (simular Tab)
            if (keyData == Keys.Enter)
            {
                Control controlActual = this.ActiveControl;

                // Lista de controles donde Enter debe funcionar como Tab
                if (controlActual is TextBox || controlActual is CheckBox)
                {
                    // Simular Tab
                    this.SelectNextControl(controlActual, true, true, true, true);
                    return true;
                }
            }

            // Para cualquier otra tecla, procesamiento normal
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
