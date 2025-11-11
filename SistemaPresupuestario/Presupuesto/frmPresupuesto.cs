using BLL.Contracts;
using BLL.DTOs;
using BLL.Enums;
using SistemaPresupuestario.Maestros.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace SistemaPresupuestario.Presupuesto
{
    public partial class frmPresupuesto : Form
    {
        private readonly IPresupuestoService _presupuestoService;
        private readonly IClienteService _clienteService;
        private readonly IVendedorService _vendedorService;
        private readonly IProductoService _productoService;

        private BindingList<PresupuestoDetalleDTO> _detalles;
        private List<PresupuestoDTO> _presupuestosCompletos;
        private int _indiceActual = -1;
        private bool _modoEdicion = false;
        private Guid? _presupuestoActualId = null;

        // Campos para almacenar IDs del cliente y vendedor seleccionados
        private Guid? _idClienteSeleccionado = null;
        private Guid? _idVendedorSeleccionado = null;

        // Caché de clientes para evitar múltiples llamadas asíncronas concurrentes
        private List<ClienteDTO> _clientesCache = null;
        private DateTime? _clientesCacheTimestamp = null;
        private readonly TimeSpan _clientesCacheDuration = TimeSpan.FromMinutes(5);

        // Nuevo: Modo de operación del formulario
        private ModoPresupuesto _modoOperacion = ModoPresupuesto.Gestionar;

        public frmPresupuesto(
            IPresupuestoService presupuestoService,
            IClienteService clienteService,
            IVendedorService vendedorService,
            IProductoService productoService)
        {
            InitializeComponent();

            _presupuestoService = presupuestoService ?? throw new ArgumentNullException(nameof(presupuestoService));
            _clienteService = clienteService ?? throw new ArgumentNullException(nameof(clienteService));
            _vendedorService = vendedorService ?? throw new ArgumentNullException(nameof(vendedorService));
            _productoService = productoService ?? throw new ArgumentNullException(nameof(productoService));

            _detalles = new BindingList<PresupuestoDetalleDTO>();
            _presupuestosCompletos = new List<PresupuestoDTO>();
        }

        /// <summary>
        /// Establece el modo de operación del formulario
        /// Debe llamarse ANTES de mostrar el formulario
        /// </summary>
        public void EstablecerModo(ModoPresupuesto modo)
        {
            _modoOperacion = modo;
        }

        private void frmPresupuesto_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                ConfigurarControles();
                ConfigurarGrilla();
                ConfigurarSegunModo(); // Nueva lógica según modo
                CargarPresupuestos();
                ConfigurarEstadoInicial();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el formulario: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Configura el formulario según el modo de operación
        /// </summary>
        private void ConfigurarSegunModo()
        {
            // ComboBox de estado siempre solo lectura
            cmbEstado.Enabled = false;

            switch (_modoOperacion)
            {
                case ModoPresupuesto.Gestionar:
                    this.Text = "Gestión de Cotizaciones";

                    // Mostrar botones de CRUD
                    btnNuevo.Visible = true;
                    btnModificar.Visible = true;
                    btnEliminar.Visible = true;
                    btnCopiar.Visible = true;

                    // Ocultar botones de aprobación
                    btnAprobar.Visible = false;
                    btnRechazar.Visible = false;
                    break;

                case ModoPresupuesto.Aprobar:
                    this.Text = "Aprobar Cotizaciones";

                    // Ocultar botones de CRUD
                    btnNuevo.Visible = false;
                    btnModificar.Visible = false;
                    btnEliminar.Visible = false;
                    btnCopiar.Visible = false;

                    // Mostrar botones de aprobación
                    btnAprobar.Visible = true;
                    btnRechazar.Visible = true;
                    break;
            }

            // Ocultar texto de TODOS los botones, solo mostrar iconos
            foreach (ToolStripItem item in tsMenu.Items)
            {
                if (item is ToolStripButton button)
                {
                    button.DisplayStyle = ToolStripItemDisplayStyle.Image;
                }
            }
        }

        private void ConfigurarControles()
        {
            // Configurar ComboBox de Estado
            cmbEstado.Items.Clear();
            cmbEstado.Items.Add(new { Value = 1, Text = "Borrado" });
            cmbEstado.Items.Add(new { Value = 2, Text = "Emitido" });
            cmbEstado.Items.Add(new { Value = 3, Text = "Aprobado" });
            cmbEstado.Items.Add(new { Value = 4, Text = "Rechazado" });
            cmbEstado.Items.Add(new { Value = 5, Text = "Vencido" });
            cmbEstado.Items.Add(new { Value = 6, Text = "Facturado" });
            cmbEstado.DisplayMember = "Text";
            cmbEstado.ValueMember = "Value";
            cmbEstado.SelectedIndex = 1; // Emitido por defecto

            // Configurar ComboBox de Condición de Pago (igual que en frmClienteAlta)
            CargarCondicionesPago();

            // Configurar ComboBox de plazo de entrega
            comboBox1.SelectedIndex = 0; // Cantidad: 1
            comboBox2.SelectedIndex = 0; // Unidad: Día/s
            comboBox3.SelectedIndex = 0; // A partir de: Firma de contrato

            // Configurar fechas
            txtFecha.Value = DateTime.Now;
            dtEntrega.Value = DateTime.Now.AddDays(7); // 7 días por defecto
        }

        /// <summary>
        /// Carga las condiciones de pago en el ComboBox (mismo formato que frmClienteAlta)
        /// </summary>
        private void CargarCondicionesPago()
        {
            // Verificar si txtCodigoFormaPago es un ComboBox o TextBox
            Control control = this.Controls.Find("txtCodigoFormaPago", true).FirstOrDefault();

            if (control != null && control is ComboBox)
            {
                ComboBox cboCondicionPago = (ComboBox)control;
                cboCondicionPago.Items.Clear();

                // Diccionario con códigos y descripciones (mismo que frmClienteAlta)
                var condiciones = new Dictionary<string, string>
                {
                    { "01", "01 - Contado" },
                    { "02", "02 - 30 días" },
                    { "03", "03 - 60 días" },
                    { "04", "04 - 90 días" },
                    { "05", "05 - 120 días" }
                };

                foreach (var condicion in condiciones)
                {
                    cboCondicionPago.Items.Add(new { Value = condicion.Key, Text = condicion.Value });
                }

                cboCondicionPago.DisplayMember = "Text";
                cboCondicionPago.ValueMember = "Value";
                cboCondicionPago.SelectedIndex = 0;
            }
        }

        private void ConfigurarGrilla()
        {
            dgArticulos.AutoGenerateColumns = false;
            dgArticulos.AllowUserToAddRows = true;

            // IMPORTANTE: Limpiar el DataSource antes de configurar
            dgArticulos.DataSource = null;

            // Configurar las columnas manualmente
            dgArticulos.Columns.Clear();

            // Columna Código
            var colCodigo = new DataGridViewTextBoxColumn
            {
                Name = "Codigo",
                HeaderText = "Código",
                DataPropertyName = "Codigo",
                Width = 115,
                ValueType = typeof(string)
            };
            dgArticulos.Columns.Add(colCodigo);

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
            dgArticulos.Columns.Add(colDescripcion);

            // Columna Cantidad
            var colCantidad = new DataGridViewTextBoxColumn
            {
                Name = "Cantidad",
                HeaderText = "Cantidad",
                DataPropertyName = "Cantidad",
                Width = 70,
                ValueType = typeof(decimal),
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2"
                }
            };
            dgArticulos.Columns.Add(colCantidad);

            // Columna Precio
            var colPrecio = new DataGridViewTextBoxColumn
            {
                Name = "Precio",
                HeaderText = "Precio",
                DataPropertyName = "Precio",
                Width = 70,
                ValueType = typeof(decimal),
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2",
                    NullValue = "0.00"
                }
            };
            dgArticulos.Columns.Add(colPrecio);

            // Columna Total (ReadOnly - calculado)
            var colTotal = new DataGridViewTextBoxColumn
            {
                Name = "Total",
                HeaderText = "Total",
                DataPropertyName = "Total",
                Width = 70,
                ReadOnly = true,
                ValueType = typeof(decimal),
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2",
                    NullValue = "0.00"
                }
            };
            dgArticulos.Columns.Add(colTotal);

            // Ahora asignar el DataSource
            dgArticulos.DataSource = _detalles;

            // Configurar eventos de la grilla
            dgArticulos.CellEndEdit += DgArticulos_CellEndEdit;
            dgArticulos.CellValidating += DgArticulos_CellValidating;
            dgArticulos.UserDeletingRow += DgArticulos_UserDeletingRow;
            dgArticulos.KeyDown += DgArticulos_KeyDown;
            dgArticulos.EditingControlShowing += DgArticulos_EditingControlShowing;
            dgArticulos.DefaultValuesNeeded += DgArticulos_DefaultValuesNeeded;
            dgArticulos.DataError += DgArticulos_DataError;
            dgArticulos.CellBeginEdit += DgArticulos_CellBeginEdit;
        }

        private void ConfigurarEstadoInicial()
        {
            _modoEdicion = false;
            HabilitarControles(false);
            HabilitarBotones(navegacion: true, edicion: true, guardar: false);
        }

        private void CargarPresupuestos()
        {
            try
            {
                // Cargar presupuestos según el modo de operación
                switch (_modoOperacion)
                {
                    case ModoPresupuesto.Gestionar:
                        // Mostrar TODOS los estados: Borrado (1), Emitido (2), Aprobado (3), Rechazado (4), Vencido (5), Facturado (6)
                        _presupuestosCompletos = _presupuestoService.GetAll().ToList();

                        if (_presupuestosCompletos.Any())
                        {
                            _indiceActual = 0;
                            MostrarPresupuesto(_presupuestosCompletos[_indiceActual]);
                        }
                        else
                        {
                            LimpiarFormulario();
                        }
                        break;

                    case ModoPresupuesto.Aprobar:
                        // Solo mostrar presupuestos Emitidos (2)
                        _presupuestosCompletos = _presupuestoService
                            .GetByEstado(2)
                            .ToList();

                        if (_presupuestosCompletos.Any())
                        {
                            _indiceActual = 0;
                            MostrarPresupuesto(_presupuestosCompletos[_indiceActual]);
                        }
                        else
                        {
                            LimpiarFormulario();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar presupuestos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarPresupuesto(PresupuestoDTO presupuesto)
        {
            _presupuestoActualId = presupuesto.Id;
            _idClienteSeleccionado = presupuesto.IdCliente;
            _idVendedorSeleccionado = presupuesto.IdVendedor;

            txtCotizacion.Text = presupuesto.Numero;
            txtFecha.Value = presupuesto.FechaEmision;
            cmbEstado.SelectedValue = presupuesto.Estado;

            if (presupuesto.FechaVencimiento.HasValue)
            {
                dtEntrega.Value = presupuesto.FechaVencimiento.Value;
            }

            // Cargar cliente
            txtCodigoCliente.Text = presupuesto.IdCliente.ToString().Substring(0, 8);
            txtCliente.Text = presupuesto.ClienteRazonSocial;

            // Cargar vendedor
            txtVendedor.Text = presupuesto.VendedorNombre ?? "";

            // Cargar condición de pago del cliente
            CargarCondicionPagoDelCliente(presupuesto.IdCliente);

            // Cargar detalles
            _detalles.Clear();
            if (presupuesto.Detalles != null)
            {
                foreach (var detalle in presupuesto.Detalles.OrderBy(d => d.Renglon))
                {
                    _detalles.Add(detalle);
                }
            }

            CalcularTotales();
        }

        /// <summary>
        /// Carga la condición de pago del cliente en el control correspondiente
        /// </summary>
        private async void CargarCondicionPagoDelCliente(Guid idCliente)
        {
            try
            {
                var cliente = await _clienteService.GetByIdAsync(idCliente);
                if (cliente != null && !string.IsNullOrEmpty(cliente.CondicionPago))
                {
                    // Si txtCodigoFormaPago es un ComboBox, seleccionar el valor
                    Control control = this.Controls.Find("txtCodigoFormaPago", true).FirstOrDefault();
                    if (control != null && control is ComboBox)
                    {
                        ComboBox cboCondicionPago = (ComboBox)control;
                        // Buscar el item con el Value correspondiente
                        for (int i = 0; i < cboCondicionPago.Items.Count; i++)
                        {
                            dynamic item = cboCondicionPago.Items[i];
                            if (item.Value == cliente.CondicionPago)
                            {
                                cboCondicionPago.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    else if (txtCodigoFormaPago != null)
                    {
                        // Si es TextBox, establecer el código
                        txtCodigoFormaPago.Text = cliente.CondicionPago;

                        // Establecer la descripción en txtFormaPago si existe
                        if (txtFormaPago != null)
                        {
                            var descripciones = new Dictionary<string, string>
                            {
                                { "01", "01 - Contado" },
                                { "02", "02 - 30 días" },
                                { "03", "03 - 60 días" },
                                { "04", "04 - 90 días" },
                                { "05", "05 - 120 días" }
                            };

                            if (descripciones.ContainsKey(cliente.CondicionPago))
                            {
                                txtFormaPago.Text = descripciones[cliente.CondicionPago];
                            }
                        }
                    }
                }
            }
            catch
            {
                // Si hayerror, dejar en blanco
                Control control = this.Controls.Find("txtCodigoFormaPago", true).FirstOrDefault();
                if (control != null && control is ComboBox)
                {
                    ((ComboBox)control).SelectedIndex = 0; // Contado por defecto
                }
                else if (txtCodigoFormaPago != null)
                {
                    txtCodigoFormaPago.Clear();
                    if (txtFormaPago != null)
                    {
                        txtFormaPago.Clear();
                    }
                }
            }
        }

        private void LimpiarFormulario()
        {
            _presupuestoActualId = null;
            _idClienteSeleccionado = null;
            _idVendedorSeleccionado = null;

            txtCotizacion.Text = "NUEVO";
            txtFecha.Value = DateTime.Now;
            cmbEstado.SelectedIndex = 1; // Emitido por defecto (estado 2)
            dtEntrega.Value = DateTime.Now.AddDays(7);

            txtCodigoCliente.Clear();
            txtCliente.Clear();
            txtVendedor.Clear();

            // Limpiar condición de pago
            Control control = this.Controls.Find("txtCodigoFormaPago", true).FirstOrDefault();
            if (control != null && control is ComboBox)
            {
                ((ComboBox)control).SelectedIndex = 0; // Contado por defecto
            }
            else if (txtCodigoFormaPago != null)
            {
                txtCodigoFormaPago.Clear();
                if (txtFormaPago != null)
                {
                    txtFormaPago.Clear();
                }
            }

            _detalles.Clear();
            CalcularTotales();
        }

        private void HabilitarControles(bool habilitar)
        {
            txtCodigoCliente.Enabled = habilitar;

            // Habilitar/deshabilitar condición de pago
            Control control = this.Controls.Find("txtCodigoFormaPago", true).FirstOrDefault();
            if (control != null && control is ComboBox)
            {
                control.Enabled = habilitar;
            }
            else if (txtCodigoFormaPago != null)
            {
                txtCodigoFormaPago.Enabled = habilitar;
            }

            comboBox1.Enabled = habilitar;
            comboBox2.Enabled = habilitar;
            comboBox3.Enabled = habilitar;
            txtFecha.Enabled = habilitar;
            dtEntrega.Enabled = habilitar;
            dgArticulos.Enabled = habilitar;

            // Cambiar el modo de solo lectura del DataGridView
            dgArticulos.ReadOnly = !habilitar;

            // También deshabilitar la edición de todas las columnas excepto las que se pueden editar
            foreach (DataGridViewColumn column in dgArticulos.Columns)
            {
                if (column.Name == "Descripcion" || column.Name == "Total")
                {
                    column.ReadOnly = true;
                }
                else
                {
                    column.ReadOnly = !habilitar;
                }
            }
        }

        private void HabilitarBotones(bool navegacion, bool edicion, bool guardar)
        {
            btnPrimero.Enabled = navegacion;
            btnAnterior.Enabled = navegacion;
            btnSiguiente.Enabled = navegacion;
            btnUltimo.Enabled = navegacion;
            btnBuscar.Enabled = navegacion;

            // Lógica específica según modo y estado del presupuesto
            if (_modoOperacion == ModoPresupuesto.Gestionar)
            {
                // Modo Gestionar: Solo botones de CRUD
                if (_presupuestoActualId.HasValue && _presupuestosCompletos.Any())
                {
                    var presupuesto = _presupuestosCompletos.FirstOrDefault(p => p.Id == _presupuestoActualId.Value);
                    if (presupuesto != null)
                    {
                        // Solo permitir editar/eliminar Emitidos (2)
                        bool esEmitido = presupuesto.Estado == 2;

                        btnNuevo.Enabled = edicion;
                        btnModificar.Enabled = edicion && esEmitido;
                        btnEliminar.Enabled = edicion && esEmitido;
                        btnCopiar.Enabled = edicion; // Copiar siempre habilitado si hay presupuesto
                    }
                    else
                    {
                        btnNuevo.Enabled = edicion;
                        btnModificar.Enabled = false;
                        btnEliminar.Enabled = false;
                        btnCopiar.Enabled = false;
                    }
                }
                else
                {
                    // Sin presupuesto seleccionado
                    btnNuevo.Enabled = edicion;
                    btnModificar.Enabled = false;
                    btnEliminar.Enabled = false;
                    btnCopiar.Enabled = false;
                }
            }
            else if (_modoOperacion == ModoPresupuesto.Aprobar)
            {
                // Modo Aprobar: Solo botones de aprobación/rechazo
                bool tienePresupuesto = _presupuestoActualId.HasValue && _presupuestosCompletos.Any();

                btnAprobar.Enabled = tienePresupuesto;
                btnRechazar.Enabled = tienePresupuesto;
            }

            btnAceptar.Enabled = guardar;
            btnCancelar.Enabled = guardar;
        }

        /// <summary>
        /// Calcula y actualiza los totales del presupuesto en los controles
        /// </summary>
        private void CalcularTotales()
        {
            try
            {
                decimal subtotal = 0;
                decimal iva = 0;
                decimal total = 0;

                foreach (var detalle in _detalles)
                {
                    subtotal += detalle.Total;
                    iva += detalle.Iva;
                    total += detalle.TotalConIva;
                }

                txtSUB.Text = subtotal.ToString("N2");
                txtIva.Text = iva.ToString("N2");
                txtTotal.Text = total.ToString("N2");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al calcular totales: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============= EVENTOS DE NAVEGACIÓN =============

        private void btnPrimero_Click(object sender, EventArgs e)
        {
            if (_presupuestosCompletos.Count == 0)
                return;

            _indiceActual = 0;
            MostrarPresupuesto(_presupuestosCompletos[_indiceActual]);
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (_presupuestosCompletos.Count == 0 || _indiceActual <= 0)
                return;

            _indiceActual--;
            MostrarPresupuesto(_presupuestosCompletos[_indiceActual]);
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (_presupuestosCompletos.Count == 0 || _indiceActual >= _presupuestosCompletos.Count - 1)
                return;

            _indiceActual++;
            MostrarPresupuesto(_presupuestosCompletos[_indiceActual]);
        }

        private void btnUltimo_Click(object sender, EventArgs e)
        {
            if (_presupuestosCompletos.Count == 0)
                return;

            _indiceActual = _presupuestosCompletos.Count - 1;
            MostrarPresupuesto(_presupuestosCompletos[_indiceActual]);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            // TODO: Implementar búsqueda de presupuestos
            MessageBox.Show("Funcionalidad de búsqueda no implementada aún", "Información",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                CargarPresupuestos();
                MessageBox.Show("Presupuestos actualizados correctamente", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar presupuestos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        // ============= EVENTOS DE CLIENTE =============

        private async void txtCodigoCliente_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigoCliente.Text))
            {
                txtCliente.Clear();
                txtVendedor.Clear();
                _idClienteSeleccionado = null;
                _idVendedorSeleccionado = null;
                return;
            }

            try
            {
                // Intentar buscar cliente por código
                var clientes = await _clienteService.GetAllAsync();
                var cliente = clientes.FirstOrDefault(c => c.CodigoCliente != null &&
                    c.CodigoCliente.Trim().Equals(txtCodigoCliente.Text.Trim(), StringComparison.OrdinalIgnoreCase));

                if (cliente != null)
                {
                    // Cliente encontrado - guardar IDs y mostrar datos
                    _idClienteSeleccionado = cliente.Id;
                    _idVendedorSeleccionado = cliente.IdVendedor;

                    txtCodigoCliente.Text = cliente.CodigoCliente;
                    txtCliente.Text = cliente.RazonSocial;

                    // Cargar nombre del vendedor si tiene
                    if (cliente.IdVendedor.HasValue)
                    {
                        var vendedor = await _vendedorService.GetByIdAsync(cliente.IdVendedor.Value);
                        if (vendedor != null)
                        {
                            txtVendedor.Text = vendedor.Nombre;
                        }
                        else
                        {
                            txtVendedor.Clear();
                        }
                    }
                    else
                    {
                        txtVendedor.Clear();
                    }

                    // Cargar condición de pago del cliente
                    CargarCondicionPagoDelCliente(cliente.Id);
                }
                else
                {
                    // Cliente no encontrado - Mostrar selector
                    MostrarSelectorCliente(clientes.ToList());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar cliente: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Muestra el selector de clientes para que el usuario elija uno
        /// </summary>
        private async void MostrarSelectorCliente(List<ClienteDTO> clientes)
        {
            try
            {
                // Configurar el selector de clientes
                var config = new SelectorConfig<ClienteDTO>
                {
                    Titulo = "Seleccionar Cliente",
                    Datos = clientes.Where(c => c.Activo), // Solo clientes activos
                    PlaceholderBusqueda = "Buscar por código, razón social o CUIT...",
                    PermitirSeleccionMultiple = false,
                    Columnas = new List<ColumnaConfig>
                    {
                        new ColumnaConfig
                        {
                            NombrePropiedad = "CodigoCliente",
                            TituloColumna = "Código",
                            Ancho = 100,
                            Visible = true
                        },
                        new ColumnaConfig
                        {
                            NombrePropiedad = "RazonSocial",
                            TituloColumna = "Razón Social",
                            Ancho = 300,
                            Visible = true
                        },
                        new ColumnaConfig
                        {
                            NombrePropiedad = "NumeroDocumento",
                            TituloColumna = "CUIT/DNI",
                            Ancho = 120,
                            Visible = true
                        },
                        new ColumnaConfig
                        {
                            NombrePropiedad = "Localidad",
                            TituloColumna = "Localidad",
                            Ancho = 150,
                            Visible = true
                        }
                    },
                    FuncionFiltro = (busqueda, cliente) =>
                    {
                        var textoBusqueda = busqueda.ToUpper();
                        return (cliente.CodigoCliente != null && cliente.CodigoCliente.ToUpper().Contains(textoBusqueda)) ||
                               (cliente.RazonSocial != null && cliente.RazonSocial.ToUpper().Contains(textoBusqueda)) ||
                               (cliente.NumeroDocumento != null && cliente.NumeroDocumento.ToUpper().Contains(textoBusqueda));
                    }
                };

                // Mostrar el selector
                var selector = frmSelector.Mostrar(config);

                if (selector.ShowDialog() == DialogResult.OK)
                {
                    var clienteSeleccionado = selector.ElementoSeleccionado as ClienteDTO;

                    if (clienteSeleccionado != null)
                    {
                        // Guardar IDs
                        _idClienteSeleccionado = clienteSeleccionado.Id;
                        _idVendedorSeleccionado = clienteSeleccionado.IdVendedor;

                        // Aplicar el cliente seleccionado
                        txtCodigoCliente.Text = clienteSeleccionado.CodigoCliente;
                        txtCliente.Text = clienteSeleccionado.RazonSocial;

                        // Cargar nombre del vendedor si tiene
                        if (clienteSeleccionado.IdVendedor.HasValue)
                        {
                            var vendedor = await _vendedorService.GetByIdAsync(clienteSeleccionado.IdVendedor.Value);
                            if (vendedor != null)
                            {
                                txtVendedor.Text = vendedor.Nombre;
                            }
                            else
                            {
                                txtVendedor.Clear();
                            }
                        }
                        else
                        {
                            txtVendedor.Clear();
                        }

                        // Cargar condición de pago del cliente
                        CargarCondicionPagoDelCliente(clienteSeleccionado.Id);
                    }
                }
                else
                {
                    // Usuario canceló - limpiar campos
                    txtCodigoCliente.Clear();
                    txtCliente.Clear();
                    txtVendedor.Clear();
                    _idClienteSeleccionado = null;
                    _idVendedorSeleccionado = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al mostrar selector de clientes: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtCodigoCliente_KeyDown(object sender, KeyEventArgs e)
        {
            // NOTA: Este evento ya no es necesario porque ProcessCmdKey maneja el Enter
            // Lo dejamos por compatibilidad pero el Enter será manejado por ProcessCmdKey
            if (e.KeyCode == Keys.F1) // Podemos usar F1 como atajo alternativo para selector
            {
                e.SuppressKeyPress = true;
                MostrarSelectorClienteDirecto();
            }
        }

        /// <summary>
        /// Muestra el selector de clientes directamente (cuando se presiona Enter en campo vacío)
        /// </summary>
        private async void MostrarSelectorClienteDirecto()
        {
            try
            {
                // Obtener todos los clientes
                var clientes = await _clienteService.GetAllAsync();
                MostrarSelectorCliente(clientes.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar clientes: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============= REPORTES =============

        /// <summary>
        /// Vista previa del presupuesto actuales
        /// </summary>
        private void VerReporte()
        {
            if (!_presupuestoActualId.HasValue)
            {
                MessageBox.Show("No hay un presupuesto seleccionado para ver el reporte", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;

                var presupuesto = _presupuestosCompletos.FirstOrDefault(p => p.Id == _presupuestoActualId.Value);
                if (presupuesto != null)
                {
                    // TODO: Implementar generación de reporte
                    MessageBox.Show("Funcionalidad de reporte no implementada aún", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el reporte: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnReporte_Click(object sender, EventArgs e)
        {
            VerReporte();
        }

        /// <summary>
        /// Método para búsqueda y aplicación de productos por código (versión asíncrona)
        /// </summary>
        private async void BuscarYAplicarProductoPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return;

            try
            {
                // Validar formato de código (optimizar según necesidad)
                if (codigo.Length < 3)
                {
                    MessageBox.Show("El código del producto debe tener al menos 3 caracteres", "Advertencia",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Buscar producto por código
                var producto = await _productoService.GetByCodigoAsync(codigo.Trim());
                if (producto != null)
                {
                    // Aplicar datos del producto a la fila actual
                    if (dgArticulos.CurrentRow != null)
                    {
                        dgArticulos.CurrentRow.Cells["Codigo"].Value = producto.Codigo;
                        dgArticulos.CurrentRow.Cells["Descripcion"].Value = producto.Descripcion;
                        // NOTA: El precio no está en la tabla Producto, dejar en 0 para que el usuario lo ingrese
                        dgArticulos.CurrentRow.Cells["Precio"].Value = 0M;

                        // Seleccionar siguiente celda
                        if (dgArticulos.Columns.Contains("Cantidad"))
                        {
                            dgArticulos.CurrentCell = dgArticulos.CurrentRow.Cells["Cantidad"];
                            dgArticulos.BeginEdit(true);
                        }
                    }
                }
                else
                {
                    // Si no se encuentra, limpiar campos relevantes
                    if (dgArticulos.CurrentRow != null)
                    {
                        dgArticulos.CurrentRow.Cells["Codigo"].Value = null;
                        dgArticulos.CurrentRow.Cells["Descripcion"].Value = null;
                        dgArticulos.CurrentRow.Cells["Precio"].Value = null;
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
        /// Busca un producto por código y lo aplica a la fila actual o abre el selector si no existe (versión síncrona)
        /// </summary>
        private void BuscarYAplicarProductoPorCodigoSync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return;

            try
            {
                // Validar formato de código (optimizar según necesidad)
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
                    if (dgArticulos.CurrentRow != null && dgArticulos.CurrentRow.Index < _detalles.Count)
                    {
                        var detalle = _detalles[dgArticulos.CurrentRow.Index];

                        // ✅ IMPORTANTE: Establecer el IdProducto (campo requerido para guardar)
                        detalle.IdProducto = producto.Id;
                        detalle.Codigo = producto.Codigo;
                        detalle.Descripcion = producto.Descripcion;
                        detalle.PorcentajeIVA = producto.PorcentajeIVA;

                        // NOTA: El precio no está en la tabla Producto, dejar en 0 para que el usuario lo ingrese
                        detalle.Precio = 0M;

                        // Refrescar la fila para mostrar los cambios
                        dgArticulos.Refresh();

                        // Seleccionar siguiente celda (Cantidad)
                        if (dgArticulos.Columns.Contains("Cantidad"))
                        {
                            dgArticulos.CurrentCell = dgArticulos.CurrentRow.Cells["Cantidad"];
                            dgArticulos.BeginEdit(true);
                        }
                    }
                }
                else
                {
                    // Si no se encuentra, limpiar campos relevantes
                    if (dgArticulos.CurrentRow != null && dgArticulos.CurrentRow.Index < _detalles.Count)
                    {
                        var detalle = _detalles[dgArticulos.CurrentRow.Index];
                        detalle.IdProducto = null;
                        detalle.Codigo = null;
                        detalle.Descripcion = null;
                        detalle.Precio = 0M;

                        dgArticulos.Refresh();
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

        // ============= IMPRESIÓN =============

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            ImprimirPresupuesto();
        }

        /// <summary>
        /// Imprime el presupuesto actual utilizando el diseño de PrintDocument
        /// </summary>
        private void ImprimirPresupuesto()
        {
            if (!_presupuestoActualId.HasValue)
            {
                MessageBox.Show("No hay un presupuesto seleccionado para imprimir", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Obtener el presupuesto
                var presupuesto = _presupuestosCompletos.FirstOrDefault(p => p.Id == _presupuestoActualId.Value);
                if (presupuesto == null)
                    throw new Exception("Presupuesto no encontrado");

                // Configurar y mostrar el diálogo de impresión
                PrintDialog printDialog = new PrintDialog();
                PrintDocument printDocument = new PrintDocument();

                printDialog.Document = printDocument;
                printDocument.PrintPage += (sender, e) => PrintDocument_PrintPage(sender, e, presupuesto);

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al imprimir presupuesto: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Evento para personalizar la impresión del presupuesto
        /// </summary>
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e, PresupuestoDTO presupuesto)
        {
            try
            {
                // Configurar fuente
                Font fontTitle = new Font("Arial", 12, FontStyle.Bold);
                Font fontNormal = new Font("Arial", 10);
                Font fontSubtitle = new Font("Arial", 11, FontStyle.Bold);

                // Información del presupuesto
                if (presupuesto != null)
                {
                    // Imprimir encabezado
                    e.Graphics.DrawString("Presupuesto:", fontTitle, Brushes.Black, 50, 50);
                    e.Graphics.DrawString($"Número: {presupuesto.Numero}", fontNormal, Brushes.Black, 50, 80);
                    e.Graphics.DrawString($"Fecha: {presupuesto.FechaEmision.ToShortDateString()}", fontNormal, Brushes.Black, 50, 110);
                    e.Graphics.DrawString($"Estado: {presupuesto.EstadoTexto}", fontNormal, Brushes.Black, 50, 140);

                    // Información del cliente
                    e.Graphics.DrawString("Cliente:", fontSubtitle, Brushes.Black, 50, 180);
                    e.Graphics.DrawString($"Código: {presupuesto.IdCliente}", fontNormal, Brushes.Black, 50, 210);
                    e.Graphics.DrawString($"Nombre: {presupuesto.ClienteRazonSocial}", fontNormal, Brushes.Black, 50, 240);

                    // Información del vendedor
                    e.Graphics.DrawString("Vendedor:", fontSubtitle, Brushes.Black, 350, 180);
                    e.Graphics.DrawString($"Nombre: {presupuesto.VendedorNombre}", fontNormal, Brushes.Black, 350, 210);

                    // Detalle de artículos
                    e.Graphics.DrawString("Detalles:", fontSubtitle, Brushes.Black, 50, 280);

                    float yPos = 310;
                    foreach (var detalle in presupuesto.Detalles)
                    {
                        e.Graphics.DrawString($"Renglon {detalle.Renglon}: {detalle.Descripcion} - Cantidad: {detalle.Cantidad} - Precio: {detalle.PrecioUnitario}",
                            fontNormal, Brushes.Black, 50, yPos);
                        yPos += 20;
                    }

                    // Totales
                    e.Graphics.DrawString($"Total Bruto: {presupuesto.TotalBruto:C}", fontNormal, Brushes.Black, 400, yPos);
                    e.Graphics.DrawString($"Total Descuentos: {presupuesto.TotalDescuentos:C}", fontNormal, Brushes.Black, 400, yPos + 30);
                    e.Graphics.DrawString($"Total Neto: {presupuesto.TotalNeto:C}", fontNormal, Brushes.Black, 400, yPos + 60);
                    e.Graphics.DrawString($"Total IVA: {presupuesto.TotalIva:C}", fontNormal, Brushes.Black, 400, yPos + 90);
                    e.Graphics.DrawString($"Total Final: {presupuesto.TotalFinal:C}", fontTitle, Brushes.Black, 400, yPos + 120);
                }
            }
            catch (Exception ex)
            {
                e.Graphics.DrawString($"Error al imprimir: {ex.Message}", new Font("Arial", 10), Brushes.Red, 50, 50);
            }
        }

        private void DgArticulos_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgArticulos.CurrentCell.ColumnIndex == 1) // Columna de descripción (asumiendo que es la segunda columna)
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

        private void DgArticulos_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Al comenzar a editar, seleccionar todo el texto
            if (e.ColumnIndex == 0 && e.RowIndex >= 0) // Columna de código (primera columna)
            {
                dgArticulos.BeginInvoke((Action)(() =>
                {
                    if (dgArticulos.EditingControl is TextBox textBox)
                    {
                        textBox.SelectAll();
                    }
                }));
            }
        }

        /// <summary>
        /// Al modificar una celda, se debe actualizar el total correspondiente
        /// </summary>
        private void DgArticulos_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Validar que el índice de la fila sea válido y esté dentro del rango de _detalles
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                    return;

                // Verificar que no sea la fila de nuevos elementos (AllowUserToAddRows = true)
                if (dgArticulos.Rows[e.RowIndex].IsNewRow)
                    return;

                // Verificar que el índice esté dentro del rango de la lista _detalles
                if (e.RowIndex >= _detalles.Count)
                    return;

                // Actualizar renglon y calcular totales
                var detalle = _detalles[e.RowIndex];
                detalle.Renglon = e.RowIndex + 1;

                // Si es la columna de código (primera columna), buscar el producto
                if (e.ColumnIndex == 0 && dgArticulos.Rows[e.RowIndex].Cells[0].Value != null)
                {
                    var codigo = dgArticulos.Rows[e.RowIndex].Cells[0].Value.ToString();
                    if (!string.IsNullOrWhiteSpace(codigo))
                    {
                        BuscarYAplicarProductoPorCodigoSync(codigo);
                    }
                }

                CalcularTotales();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar detalle: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============= EVENTOS DE BOTONES =============

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            try
            {
                _modoEdicion = true;
                LimpiarFormulario();

                // Obtener siguiente número
                var nuevoNumero = _presupuestoService.GetNextNumero();
                txtCotizacion.Text = nuevoNumero;

                HabilitarControles(true);
                HabilitarBotones(navegacion: false, edicion: false, guardar: true);

                txtCodigoCliente.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al crear nuevo presupuesto: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (!_presupuestoActualId.HasValue)
            {
                MessageBox.Show("No hay un presupuesto seleccionado para modificar", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var presupuesto = _presupuestosCompletos.FirstOrDefault(p => p.Id == _presupuestoActualId.Value);
            if (presupuesto != null)
            {
                // Solo se pueden modificar presupuestos Emitidos (estado 2)
                if (presupuesto.Estado != 2)
                {
                    MessageBox.Show("Solo se pueden modificar presupuestos en estado Emitido", "Advertencia",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            _modoEdicion = true;
            HabilitarControles(true);
            HabilitarBotones(navegacion: false, edicion: false, guardar: true);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (!_presupuestoActualId.HasValue)
            {
                MessageBox.Show("No hay un presupuesto seleccionado para eliminar", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var presupuesto = _presupuestosCompletos.FirstOrDefault(p => p.Id == _presupuestoActualId.Value);
            if (presupuesto != null && presupuesto.Estado != 2) // Solo Emitidos
            {
                MessageBox.Show("Solo se pueden eliminar presupuestos en estado Emitido", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                "¿Está seguro de eliminar este presupuesto?\n\n" +
                "El presupuesto cambiará a estado Borrado.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    _presupuestoService.Borrar(_presupuestoActualId.Value);

                    MessageBox.Show("Presupuesto marcado como Borrado exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CargarPresupuestos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar presupuesto: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void btnCopiar_Click(object sender, EventArgs e)
        {
            if (!_presupuestoActualId.HasValue)
            {
                MessageBox.Show("No hay un presupuesto seleccionado para copiar", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;

                var nuevoCopia = _presupuestoService.Copiar(_presupuestoActualId.Value);

                MessageBox.Show("Presupuesto copiado exitosamente", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarPresupuestos();

                // Buscar y mostrar la copia
                _indiceActual = _presupuestosCompletos.FindIndex(p => p.Id == nuevoCopia.Id);
                if (_indiceActual >= 0)
                {
                    MostrarPresupuesto(_presupuestosCompletos[_indiceActual]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al copiar presupuesto: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // IMPORTANTE: Finalizar edición de la grilla para confirmar cambios pendientes
                dgArticulos.EndEdit();

                // Validar que se haya seleccionado un cliente
                if (!_idClienteSeleccionado.HasValue)
                {
                    MessageBox.Show("Debe seleccionar un cliente válido", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCodigoCliente.Focus();
                    return;
                }

                // Obtener solo los detalles válidos (filtrar fila vacía del DataGridView)
                var detallesValidos = ObtenerDetallesValidos();

                if (detallesValidos.Count == 0)
                {
                    MessageBox.Show("Debe agregar al menos un artículo al presupuesto", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dgArticulos.Focus();
                    return;
                }

                // Obtener el estado, con valor por defecto si es null
                int estadoPresupuesto = 2; // Emitido por defecto
                if (cmbEstado.SelectedValue != null)
                {
                    estadoPresupuesto = (int)cmbEstado.SelectedValue;
                }

                if (_modoEdicion && _presupuestoActualId.HasValue)
                {
                    // Modo edición: actualizar presupuesto existente
                    var presupuestoActualizar = new PresupuestoDTO
                    {
                        Id = _presupuestoActualId.Value,
                        Numero = txtCotizacion.Text.Trim(),
                        FechaEmision = txtFecha.Value,
                        Estado = estadoPresupuesto,
                        FechaVencimiento = dtEntrega.Value,
                        IdCliente = _idClienteSeleccionado.Value,
                        IdVendedor = _idVendedorSeleccionado, // Puede ser null
                        Detalles = detallesValidos
                    };

                    // Validar datos requeridos
                    var resultadoValidacion = ValidarDatosPresupuesto(presupuestoActualizar);
                    if (!resultadoValidacion.IsValid)
                    {
                        MessageBox.Show(resultadoValidacion.Mensaje, "Advertencia",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Actualizar presupuesto
                    _presupuestoService.Update(presupuestoActualizar);

                    MessageBox.Show("Presupuesto actualizado correctamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Modo nuevo: crear presupuesto
                    var presupuestoNuevo = new PresupuestoDTO();

                    presupuestoNuevo.Numero = txtCotizacion.Text.Trim();
                    presupuestoNuevo.FechaEmision = txtFecha.Value;
                    presupuestoNuevo.Estado = estadoPresupuesto;
                    presupuestoNuevo.FechaVencimiento = dtEntrega.Value;
                    presupuestoNuevo.IdCliente = _idClienteSeleccionado.Value;
                    presupuestoNuevo.IdVendedor = _idVendedorSeleccionado;
                    presupuestoNuevo.Detalles = detallesValidos;


                    // Validar datos requeridos
                    var resultadoValidacion = ValidarDatosPresupuesto(presupuestoNuevo);
                    if (!resultadoValidacion.IsValid)
                    {
                        MessageBox.Show(resultadoValidacion.Mensaje, "Advertencia",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Crear nuevo presupuesto usando Add en lugar de Create
                    _presupuestoService.Add(presupuestoNuevo);
                    _presupuestoActualId = presupuestoNuevo.Id;

                    MessageBox.Show("Presupuesto creado correctamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Recargar presupuestos
                CargarPresupuestos();
                ConfigurarEstadoInicial();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar presupuesto: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Obtiene solo los detalles válidos de la grilla, filtrando filas vacías
        /// </summary>
        private List<PresupuestoDetalleDTO> ObtenerDetallesValidos()
        {
            var detallesValidos = new List<PresupuestoDetalleDTO>();

            foreach (var detalle in _detalles)
            {
                // Validar que el detalle tenga un código de producto válido
                // (esto indica que no es la fila vacía del DataGridView)
                if (!string.IsNullOrWhiteSpace(detalle.Codigo) &&
                    detalle.IdProducto.HasValue &&
                    detalle.IdProducto.Value != Guid.Empty)
                {
                    // Validar que tenga cantidad y precio válidos
                    if (detalle.Cantidad > 0 && detalle.Precio >= 0)
                    {
                        detallesValidos.Add(detalle);
                    }
                }
            }

            return detallesValidos;
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
                    if (dgArticulos.CurrentRow != null && dgArticulos.CurrentRow.Index < _detalles.Count)
                    {
                        var detalle = _detalles[dgArticulos.CurrentRow.Index];

                        // ✅ IMPORTANTE: Establecer el IdProducto (campo requerido para guardar)
                        detalle.IdProducto = producto.Id;
                        detalle.Codigo = producto.Codigo;
                        detalle.Descripcion = producto.Descripcion;
                        detalle.PorcentajeIVA = producto.PorcentajeIVA;

                        // NOTA: El precio no está en la tabla Producto, dejar en 0 para que el usuario lo ingrese
                        detalle.Precio = 0M;

                        // Refrescar la fila
                        dgArticulos.Refresh();
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
        private async void MostrarSelectorProducto()
        {
            try
            {
                // Obtener todos los productos activos
                var productos = await _productoService.GetActivosAsync();
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
                            Ancho = 300,
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

                    if (productoSeleccionado != null && dgArticulos.CurrentRow != null)
                    {
                        // Verificar si la fila actual pertenece a _detalles
                        int rowIndex = dgArticulos.CurrentRow.Index;

                        if (rowIndex < _detalles.Count)
                        {
                            // Fila existente en _detalles
                            var detalle = _detalles[rowIndex];

                            // ✅ IMPORTANTE: Establecer el IdProducto (campo requerido para guardar)
                            detalle.IdProducto = productoSeleccionado.Id;
                            detalle.Codigo = productoSeleccionado.Codigo;
                            detalle.Descripcion = productoSeleccionado.Descripcion;
                            detalle.PorcentajeIVA = productoSeleccionado.PorcentajeIVA;

                            // NOTA: El precio no está en la tabla Producto, dejar en 0
                            detalle.Precio = 0M;
                        }
                        else
                        {
                            // Fila nueva (la fila de agregar del DataGridView)
                            var nuevoDetalle = new PresupuestoDetalleDTO
                            {
                                IdProducto = productoSeleccionado.Id,
                                Codigo = productoSeleccionado.Codigo,
                                Descripcion = productoSeleccionado.Descripcion,
                                PorcentajeIVA = productoSeleccionado.PorcentajeIVA,
                                Precio = 0M,
                                Cantidad = 1M
                            };

                            _detalles.Add(nuevoDetalle);
                        }

                        // Refrescar la grilla
                        dgArticulos.Refresh();

                        // Mover a la columna de cantidad
                        int row = dgArticulos.CurrentRow.Index;
                        dgArticulos.CurrentCell = dgArticulos["Cantidad", row];
                        dgArticulos.BeginEdit(true);
                    }
                }
                else
                {
                    // Usuario canceló - limpiar la fila si está vacía
                    if (dgArticulos.CurrentRow != null && dgArticulos.CurrentRow.Index < _detalles.Count)
                    {
                        var detalle = _detalles[dgArticulos.CurrentRow.Index];
                        var codigo = detalle.Codigo;

                        if (string.IsNullOrWhiteSpace(codigo))
                        {
                            detalle.IdProducto = null;
                            detalle.Descripcion = null;
                            detalle.Precio = 0M;
                            dgArticulos.Refresh();
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

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            ConfigurarEstadoInicial();
        }

        /// <summary>
        /// Aprueba el presupuesto actual
        /// </summary>
        private void btnAprobar_Click(object sender, EventArgs e)
        {
            if (!_presupuestoActualId.HasValue)
            {
                MessageBox.Show("No hay un presupuesto seleccionado para aprobar", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var presupuesto = _presupuestosCompletos.FirstOrDefault(p => p.Id == _presupuestoActualId.Value);
            if (presupuesto != null && presupuesto.Estado != 2) // Solo Emitidos
            {
                MessageBox.Show("Solo se pueden aprobar presupuestos en estado Emitido", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                "¿Está seguro de aprobar este presupuesto?",
                "Confirmar aprobación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    _presupuestoService.Aprobar(_presupuestoActualId.Value);

                    MessageBox.Show("Presupuesto aprobado exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CargarPresupuestos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al aprobar presupuesto: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        /// <summary>
        /// Rechaza el presupuesto actualmente seleccionado
        /// </summary>
        private void btnRechazar_Click(object sender, EventArgs e)
        {
            if (!_presupuestoActualId.HasValue)
            {
                MessageBox.Show("No hay un presupuesto seleccionado para rechazar", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var presupuesto = _presupuestosCompletos.FirstOrDefault(p => p.Id == _presupuestoActualId.Value);
            if (presupuesto != null && presupuesto.Estado != 2) // Solo Emitidos
            {
                MessageBox.Show("Solo se pueden rechazar presupuestos en estado Emitido", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                "¿Está seguro de rechazar este presupuesto?",
                "Confirmar rechazo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    _presupuestoService.Rechazar(_presupuestoActualId.Value);

                    MessageBox.Show("Presupuesto rechazado exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CargarPresupuestos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al rechazar presupuesto: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        /// <summary>
        /// Valida los datos de un presupuesto antes de ser guardado
        /// </summary>
        private ValidacionResultado ValidarDatosPresupuesto(PresupuestoDTO presupuesto)
        {
            var resultado = new ValidacionResultado { IsValid = true };

            // Validar número
            if (string.IsNullOrWhiteSpace(presupuesto.Numero))
            {
                resultado.IsValid = false;
                resultado.Mensaje = "El número de presupuesto es obligatorio.";
                return resultado;
            }

            // Validar cliente
            if (presupuesto.IdCliente == Guid.Empty)
            {
                resultado.IsValid = false;
                resultado.Mensaje = "Debe seleccionar un cliente válido.";
                return resultado;
            }

            // Validar detalles
            if (presupuesto.Detalles == null || !presupuesto.Detalles.Any())
            {
                resultado.IsValid = false;
                resultado.Mensaje = "El presupuesto debe tener al menos un detalle.";
                return resultado;
            }

            // Validar que todos los detalles tengan código de producto
            foreach (var detalle in presupuesto.Detalles)
            {
                if (string.IsNullOrWhiteSpace(detalle.Codigo))
                {
                    resultado.IsValid = false;
                    resultado.Mensaje = "Todos los detalles deben tener un código de producto válido.";
                    return resultado;
                }
            }

            return resultado;
        }

        /// <summary>
        /// Evento cuando se valida una celda antes de cambiar de foco
        /// </summary>
        private void DgArticulos_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Validar valores numéricos en columnas de cantidad y precio
            if (e.ColumnIndex == dgArticulos.Columns["Cantidad"].Index ||
                e.ColumnIndex == dgArticulos.Columns["Precio"].Index)
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

        /// <summary>
        /// Evento antes de eliminar una fila
        /// </summary>
        private void DgArticulos_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            var result = MessageBox.Show("¿Está seguro de eliminar este detalle?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Evento para manejar teclas en el DataGridView
        /// </summary>
        private void DgArticulos_KeyDown(object sender, KeyEventArgs e)
        {
            // Permitir eliminar fila con tecla Delete
            if (e.KeyCode == Keys.Delete && !dgArticulos.CurrentRow.IsNewRow)
            {
                var result = MessageBox.Show("¿Está seguro de eliminar este detalle?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    dgArticulos.Rows.Remove(dgArticulos.CurrentRow);
                    CalcularTotales();
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// Evento cuando se necesitan valores por defecto para una nueva fila
        /// </summary>
        private void DgArticulos_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            // Establecer valores por defecto
            e.Row.Cells["Codigo"].Value = "";
            e.Row.Cells["Descripcion"].Value = "";
            e.Row.Cells["Cantidad"].Value = 1M;
            e.Row.Cells["Precio"].Value = 0M;
            e.Row.Cells["Total"].Value = 0M;
        }

        /// <summary>
        /// Evento para manejar errores de datos en el DataGridView
        /// </summary>
        private void DgArticulos_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Prevenir que se muestre el error por defecto
            e.ThrowException = false;
            e.Cancel = true;

            // Registrar información del error para debugging
            var columnName = dgArticulos.Columns[e.ColumnIndex]?.Name ?? "Desconocida";
            var rowIndex = e.RowIndex;

            // Mostrar mensaje al usuario solo si no es un error de conversión temporal
            if (e.Context != DataGridViewDataErrorContexts.Parsing)
            {
                MessageBox.Show($"Error en la columna '{columnName}' (fila {rowIndex + 1}):\n{e.Exception.Message}",
                    "Error de datos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Valida el cliente ingresado y mueve el foco al siguiente control
        /// Si no es válido, muestra el selector
        /// </summary>
        private async void ValidarYMoverFocoCliente()
        {
            try
            {
                var codigo = txtCodigoCliente.Text.Trim();

                if (string.IsNullOrWhiteSpace(codigo))
                {
                    txtCliente.Clear();
                    txtVendedor.Clear();
                    _idClienteSeleccionado = null;
                    _idVendedorSeleccionado = null;

                    // Mover al siguiente control
                    this.SelectNextControl(txtCodigoCliente, true, true, true, true);
                    return;
                }

                // Intentar buscar cliente por código
                var clientes = await _clienteService.GetAllAsync();
                var cliente = clientes.FirstOrDefault(c => c.CodigoCliente != null &&
                    c.CodigoCliente.Trim().Equals(codigo, StringComparison.OrdinalIgnoreCase));

                if (cliente != null)
                {
                    // Cliente encontrado - guardar IDs y aplicar datos
                    _idClienteSeleccionado = cliente.Id;
                    _idVendedorSeleccionado = cliente.IdVendedor;

                    txtCodigoCliente.Text = cliente.CodigoCliente;
                    txtCliente.Text = cliente.RazonSocial;

                    // Cargar nombre del vendedor si tiene
                    if (cliente.IdVendedor.HasValue)
                    {
                        var vendedor = await _vendedorService.GetByIdAsync(cliente.IdVendedor.Value);
                        if (vendedor != null)
                        {
                            txtVendedor.Text = vendedor.Nombre;
                        }
                        else
                        {
                            txtVendedor.Clear();
                        }
                    }
                    else
                    {
                        txtVendedor.Clear();
                    }

                    // Cargar condición de pago del cliente
                    CargarCondicionPagoDelCliente(cliente.Id);

                    // Mover al siguiente control
                    this.SelectNextControl(txtCodigoCliente, true, true, true, true);
                }
                else
                {
                    // Cliente no encontrado - Mostrar selector
                    MostrarSelectorCliente(clientes.ToList());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar cliente: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sobreescribe ProcessCmdKey para manejar Enter como Tab en todo el formulario
        /// Incluye validación de campos y apertura de selector cuando sea necesario
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Manejar Enter en DataGridView (solo columna Codigo)
            if ((dgArticulos.Focused || dgArticulos.EditingControl != null) && keyData == Keys.Enter)
            {
                // Solo procesar si estamos en la columna de código
                if (dgArticulos.CurrentCell != null && dgArticulos.CurrentCell.ColumnIndex == 0) // Columna Codigo
                {
                    // Finalizar edición actual
                    dgArticulos.EndEdit();

                    // Obtener el código ingresado
                    var codigo = dgArticulos.CurrentCell.Value?.ToString()?.Trim();

                    if (!string.IsNullOrWhiteSpace(codigo))
                    {
                        // Validar y buscar producto
                        bool productoEncontrado = BuscarYAplicarProductoPorCodigoEnter(codigo);

                        if (productoEncontrado)
                        {
                            // Mover a la siguiente celda (Cantidad)
                            int col = dgArticulos.CurrentCell.ColumnIndex;
                            int row = dgArticulos.CurrentCell.RowIndex;

                            if (col < dgArticulos.ColumnCount - 1)
                            {
                                dgArticulos.CurrentCell = dgArticulos[col + 1, row];
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
                    dgArticulos.EndEdit();

                    int col = dgArticulos.CurrentCell.ColumnIndex;
                    int row = dgArticulos.CurrentCell.RowIndex;

                    // Si no es la última columna
                    if (col < dgArticulos.ColumnCount - 1)
                    {
                        dgArticulos.CurrentCell = dgArticulos[col + 1, row];
                    }
                    // Si es la última columna y no es la última fila
                    else if (row < dgArticulos.RowCount - 1)
                    {
                        dgArticulos.CurrentCell = dgArticulos[0, row + 1];
                    }

                    return true;
                }
            }

            // Manejar Enter en TextBox de código de cliente
            if (txtCodigoCliente.Focused && keyData == Keys.Enter)
            {
                // Suprimir el beep
                var codigo = txtCodigoCliente.Text.Trim();

                if (string.IsNullOrWhiteSpace(codigo))
                {
                    // Campo vacío - mostrar selector directamente
                    MostrarSelectorClienteDirecto();
                }
                else
                {
                    // Validar cliente
                    ValidarYMoverFocoCliente();
                }

                return true; // Indicamos que manejamos la tecla
            }

            // Manejar Enter en otros controles editables (simular Tab)
            if (keyData == Keys.Enter)
            {
                Control controlActual = this.ActiveControl;

                // Lista de controles donde Enter debe funcionar como Tab
                if (controlActual is TextBox ||
                    controlActual is ComboBox ||
                    controlActual is DateTimePicker)
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

    /// <summary>
    /// Resultado de la validación de datos de un presupuesto
    /// </summary>
    public class ValidacionResultado
    {
        public bool IsValid { get; set; }
        public string Mensaje { get; set; }
    }
}
