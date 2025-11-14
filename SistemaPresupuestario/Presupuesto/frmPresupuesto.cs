using BLL.Contracts;
using BLL.DTOs;
using BLL.Enums;
using Services.Services.Contracts;
using SistemaPresupuestario.Maestros.Shared;
using SistemaPresupuestario.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        private readonly IListaPrecioService _listaPrecioService;
        private readonly IPresupuestoPdfService _pdfService;

        private BindingList<PresupuestoDetalleDTO> _detalles;
        private List<PresupuestoDTO> _presupuestosCompletos;
        private int _indiceActual = -1;
        private bool _modoEdicion = false;
        private Guid? _presupuestoActualId = null;

        // Campos para almacenar IDs del cliente, vendedor y lista de precios seleccionados
        private Guid? _idClienteSeleccionado = null;
        private Guid? _idVendedorSeleccionado = null;
        private Guid? _idListaPrecioSeleccionada = null;
        
        // NUEVO: Campo para almacenar si la lista de precios incluye IVA
        private bool _listaPrecioIncluyeIva = false;
        
        // NUEVO: Campo para almacenar la alícuota ARBA del cliente
        private decimal _alicuotaArbaCliente = 0M;

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
            IProductoService productoService,
            IListaPrecioService listaPrecioService,
            IPresupuestoPdfService pdfService)
        {
            InitializeComponent();

            _presupuestoService = presupuestoService ?? throw new ArgumentNullException(nameof(presupuestoService));
            _clienteService = clienteService ?? throw new ArgumentNullException(nameof(clienteService));
            _vendedorService = vendedorService ?? throw new ArgumentNullException(nameof(vendedorService));
            _productoService = productoService ?? throw new ArgumentNullException(nameof(productoService));
            _listaPrecioService = listaPrecioService ?? throw new ArgumentNullException(nameof(listaPrecioService));

            _detalles = new BindingList<PresupuestoDetalleDTO>();
            _presupuestosCompletos = new List<PresupuestoDTO>();

            // NUEVO: Inicializar generador de PDF
            _pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
            
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

            if (dgArticulos.Columns.Count > 0)
            {
                ActualizarColumnasGrilla();
            }
        }
        
        /// <summary>
        /// Actualiza los encabezados de columnas de la grilla
        /// </summary>
        private void ActualizarColumnasGrilla()
        {
            foreach (DataGridViewColumn column in dgArticulos.Columns)
            {
                if (column.Tag == null && !string.IsNullOrWhiteSpace(column.HeaderText))
                {
                    column.Tag = column.HeaderText;
                }

                if (column.Tag is string key)
                {
                    column.HeaderText = I18n.T(key);
                }
            }
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
                ConfigurarSegunModo();
                CargarPresupuestos();
                ConfigurarEstadoInicial();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{I18n.T("Error al cargar el formulario")}: {ex.Message}", I18n.T("Error"),
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
                    this.Text = I18n.T("Gestión de Cotizaciones");

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
                    this.Text = I18n.T("Aprobar Cotizaciones");

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

            
        }

        private void ConfigurarControles()
        {
            // Configurar ComboBox de Estado
            cmbEstado.Items.Clear();

            var estadosLista = new List<dynamic>
            {
                new { Value = 1, Text = I18n.T("Borrado") },
                new { Value = 2, Text = I18n.T("Emitido") },
                new { Value = 3, Text = I18n.T("Aprobado") },
                new { Value = 4, Text = I18n.T("Rechazado") },
                new { Value = 5, Text = I18n.T("Vencido") },
                new { Value = 6, Text = I18n.T("Facturado") }
            };

            cmbEstado.DataSource = estadosLista;
            cmbEstado.DisplayMember = "Text";
            cmbEstado.ValueMember = "Value";
            cmbEstado.SelectedIndex = 1;

            // Configurar ComboBox de Condición de Pago
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
                    { "01", I18n.T("01 - Contado") },
                    { "02", I18n.T("02 - 30 días") },
                    { "03", I18n.T("03 - 60 días") },
                    { "04", I18n.T("04 - 90 días") },
                    { "05", I18n.T("05 - 120 días") }
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
                HeaderText = I18n.T("Código"),
                DataPropertyName = "Codigo",
                Width = 115,
                ValueType = typeof(string)
            };
            dgArticulos.Columns.Add(colCodigo);

            // Columna Descripción (ReadOnly)
            var colDescripcion = new DataGridViewTextBoxColumn
            {
                Name = "Descripcion",
                HeaderText = I18n.T("Descripción"),
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
                HeaderText = I18n.T("Cantidad"),
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
                HeaderText = I18n.T("Precio"),
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

            // Columna Descuento (NUEVA)
            var colDescuento = new DataGridViewTextBoxColumn
            {
                Name = "Descuento",
                HeaderText = I18n.T("Desc %"),
                DataPropertyName = "Descuento",
                Width = 70,
                ValueType = typeof(decimal),
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2",
                    NullValue = "0.00"
                }
            };
            dgArticulos.Columns.Add(colDescuento);

            // Columna Total (ReadOnly - calculado)
            var colTotal = new DataGridViewTextBoxColumn
            {
                Name = "Total",
                HeaderText = I18n.T("Total"),
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
            _idListaPrecioSeleccionada = presupuesto.IdListaPrecio; // NUEVO

            txtCotizacion.Text = presupuesto.Numero;
            txtFecha.Value = presupuesto.FechaEmision;
            cmbEstado.SelectedValue = presupuesto.Estado;

            if (presupuesto.FechaVencimiento.HasValue)
            {
                dtEntrega.Value = presupuesto.FechaVencimiento.Value;
            }

            // Cargar cliente
            txtCodigoCliente.Text = presupuesto.ClienteCodigoCliente ?? "";
            txtCliente.Text = presupuesto.ClienteRazonSocial;

            // Cargar vendedor
            txtVendedor.Text = presupuesto.VendedorNombre ?? "";

            // Cargar lista de precios (NUEVO)
            CargarListaPrecioDelPresupuesto(presupuesto.IdListaPrecio);

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

            // MODIFICADO: Cargar totales persistidos en lugar de calcularlos
            txtSUB.Text = presupuesto.Subtotal.ToString("N2");
            txtIva.Text = presupuesto.TotalIva.ToString("N2");
            txtIIBBArba.Text = presupuesto.ImporteArba.ToString("N2"); // NUEVO
            txtTotal.Text = presupuesto.Total.ToString("N2");
        }

        /// <summary>
        /// Carga la lista de precios en los controles correspondientes (NUEVO)
        /// </summary>
        private void CargarListaPrecioDelPresupuesto(Guid? idListaPrecio)
        {
            try
            {
                if (idListaPrecio.HasValue)
                {
                    var lista = _listaPrecioService.GetById(idListaPrecio.Value);
                    if (lista != null)
                    {
                        txtCodigoListaPrecio.Text = lista.Codigo;
                        txtListaPrecio.Text = lista.Nombre;
                        
                        // NUEVO: Almacenar el estado IncluyeIva
                        _listaPrecioIncluyeIva = lista.IncluyeIva;
                    }
                    else
                    {
                        txtCodigoListaPrecio.Clear();
                        txtListaPrecio.Clear();
                        _listaPrecioIncluyeIva = false;
                    }
                }
                else
                {
                    txtCodigoListaPrecio.Clear();
                    txtListaPrecio.Clear();
                    _listaPrecioIncluyeIva = false;
                }
            }
            catch
            {
                // Si hay error, limpiar los campos
                txtCodigoListaPrecio.Clear();
                txtListaPrecio.Clear();
                _listaPrecioIncluyeIva = false;
            }
        }

        /// <summary>
        /// Carga la condición de pago del cliente en el control correspondiente
        /// NUEVO: También carga la alícuota ARBA del cliente
        /// </summary>
        private void CargarCondicionPagoDelCliente(Guid idCliente)
        {
            try
            {
                var cliente = _clienteService.GetById(idCliente);
                if (cliente != null)
                {
                    // NUEVO: Cargar alícuota ARBA del cliente
                    _alicuotaArbaCliente = cliente.AlicuotaArba;
                    
                    // Cargar condición de pago si existe
                    if (!string.IsNullOrEmpty(cliente.CondicionPago))
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
                    
                    // Recalcular totales con la nueva alícuota ARBA
                    CalcularTotales();
                }
            }
            catch
            {
                // Si hay error, establecer ARBA en 0 y limpiar campos
                _alicuotaArbaCliente = 0M;
                
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
            _idListaPrecioSeleccionada = null;
            _listaPrecioIncluyeIva = false;
            _alicuotaArbaCliente = 0M; // NUEVO

            txtCotizacion.Text = I18n.T("NUEVO");
            txtFecha.Value = DateTime.Now;
            cmbEstado.SelectedIndex = 1;
            dtEntrega.Value = DateTime.Now.AddDays(7);

            txtCodigoCliente.Clear();
            txtCliente.Clear();
            txtVendedor.Clear();

            // Limpiar lista de precios (NUEVO)
            txtCodigoListaPrecio.Clear();
            txtListaPrecio.Clear();

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
            txtCodigoListaPrecio.Enabled = habilitar;
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
        /// NUEVO: Considera si la lista de precios incluye IVA o no y calcula la percepción ARBA
        /// 
        /// Fórmula de cálculo:
        /// 1. Subtotal Neto (Base Imponible) = Suma(Cantidad * Precio - Descuento) de todos los detalles
        /// 2. Percepción ARBA = Subtotal Neto * AlicuotaArba del cliente
        /// 3. IVA = Subtotal Neto * Porcentaje IVA
        /// 4. Total = Subtotal Neto + Percepción ARBA + IVA
        /// </summary>
        private void CalcularTotales()
        {
            try
            {
                decimal subtotal = 0;
                decimal iva = 0;
                decimal arba = 0;
                decimal total = 0;

                if (_listaPrecioIncluyeIva)
                {
                    // Si la lista de precios INCLUYE IVA:
                    // El "Total" de cada detalle YA incluye el IVA
                    // Necesitamos calcular el IVA implícito usando la fórmula: Total - (Total / (1 + PorcentajeIVA/100))
                    foreach (var detalle in _detalles)
                    {
                        decimal totalConIva = detalle.Total;
                        
                        // Calcular el IVA implícito usando la fórmula: Total - (Total / (1 + PorcentajeIVA/100))
                        decimal divisor = 1 + (detalle.PorcentajeIVA / 100);
                        decimal totalSinIva = totalConIva / divisor;
                        decimal ivaCalculado = totalConIva - totalSinIva;
                        
                        subtotal += totalSinIva;
                        iva += ivaCalculado;
                    }
                    
                    // Calcular percepción ARBA sobre la base imponible (subtotal sin IVA)
                    arba = subtotal * (_alicuotaArbaCliente / 100);
                    
                    // Total = Subtotal + ARBA + IVA
                    total = subtotal + arba + iva;
                }
                else
                {
                    // Si la lista de precios NO INCLUYE IVA:
                    // Cálculo tradicional
                    foreach (var detalle in _detalles)
                    {
                        // El Total del detalle es la base imponible (sin IVA)
                        subtotal += detalle.Total;
                        
                        // Calcular IVA sobre la base
                        iva += detalle.Iva;
                    }
                    
                    // Calcular percepción ARBA sobre la base imponible (subtotal sin IVA)
                    arba = subtotal * (_alicuotaArbaCliente / 100);
                    
                    // Total = Subtotal + ARBA + IVA
                    total = subtotal + arba + iva;
                }

                txtSUB.Text = subtotal.ToString("N2");
                txtIva.Text = iva.ToString("N2");
                txtIIBBArba.Text = arba.ToString("N2"); // NUEVO: Mostrar percepción ARBA
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

        private void txtCodigoCliente_Leave(object sender, EventArgs e)
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
                var clientes = _clienteService.GetAll();
                var cliente = clientes.FirstOrDefault(c => c.CodigoCliente != null &&
                    c.CodigoCliente.Trim().Equals(txtCodigoCliente.Text.Trim(), StringComparison.OrdinalIgnoreCase));

                if (cliente != null)
                {
                    // Cliente encontrado - guardar IDs y mostrar datos
                    _idClienteSeleccionado = cliente.Id;
                    _idVendedorSeleccionado = cliente.IdVendedor;
                    _alicuotaArbaCliente = cliente.AlicuotaArba; // NUEVO

                    txtCodigoCliente.Text = cliente.CodigoCliente;
                    txtCliente.Text = cliente.RazonSocial;

                    // Cargar nombre del vendedor si tiene
                    if (cliente.IdVendedor.HasValue)
                    {
                        var vendedor = _vendedorService.GetById(cliente.IdVendedor.Value);
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

                    // Cargar condición de pago del cliente (también cargará AlicuotaArba y recalculará)
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
        private void MostrarSelectorCliente(List<ClienteDTO> clientes)
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
                        // Guardar IDs y alícuota ARBA
                        _idClienteSeleccionado = clienteSeleccionado.Id;
                        _idVendedorSeleccionado = clienteSeleccionado.IdVendedor;
                        _alicuotaArbaCliente = clienteSeleccionado.AlicuotaArba; // NUEVO

                        // Aplicar el cliente seleccionado
                        txtCodigoCliente.Text = clienteSeleccionado.CodigoCliente;
                        txtCliente.Text = clienteSeleccionado.RazonSocial;

                        // Cargar nombre del vendedor si tiene
                        if (clienteSeleccionado.IdVendedor.HasValue)
                        {
                            var vendedor = _vendedorService.GetById(clienteSeleccionado.IdVendedor.Value);
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

                        // Cargar condición de pago del cliente (también recalculará totales)
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
                    _alicuotaArbaCliente = 0M; // NUEVO
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
            if (e.KeyCode == Keys.F1) // Usar F1 como atajo alternativo para selector
            {
                e.SuppressKeyPress = true;
                MostrarSelectorClienteDirecto();
            }
        }

        /// <summary>
        /// Muestra el selector de clientes directamente (cuando se presiona Enter en campo vacío)
        /// </summary>
        private void MostrarSelectorClienteDirecto()
        {
            try
            {
                // Obtener todos los clientes
                var clientes = _clienteService.GetAll();
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
        /// Vista previa del presupuesto currente
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
        /// Genera e imprime el presupuesto actual en formato PDF
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
                this.Cursor = Cursors.WaitCursor;

                // Obtener el presupuesto con todos sus datos
                var presupuesto = _presupuestosCompletos.FirstOrDefault(p => p.Id == _presupuestoActualId.Value);
                
                if (presupuesto == null)
                {
                    MessageBox.Show("No se pudo cargar el presupuesto", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Generar y abrir el PDF
                _pdfService.GenerarYAbrirPdf(presupuesto.Id);

                MessageBox.Show("PDF generado exitosamente", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar el PDF: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
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

                // Actualizar renglon
                var detalle = _detalles[e.RowIndex];
                detalle.Renglon = e.RowIndex + 1;

                // Si es la columna de código (primera columna), buscar el producto
                if (e.ColumnIndex == 0 && dgArticulos.Rows[e.RowIndex].Cells[0].Value != null)
                {
                    var codigo = dgArticulos.Rows[e.RowIndex].Cells[0].Value.ToString();
                    if (!string.IsNullOrWhiteSpace(codigo))
                    {
                        BuscarYAplicarProductoPorCodigoEnter(codigo);
                    }
                }

                // ✅ NUEVO: Si el usuario modificó Cantidad, Precio o Descuento, recalcular el Total
                string columnName = dgArticulos.Columns[e.ColumnIndex].Name;
                if (columnName == "Cantidad" || columnName == "Precio" || columnName == "Descuento")
                {
                    // Llamar explícitamente a RecalcularTotal()
                    detalle.RecalcularTotal();
                    
                    // Refrescar la fila para mostrar el Total actualizado en la grilla
                    dgArticulos.InvalidateRow(e.RowIndex);
                }

                // Recalcular totales del presupuesto
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
                MessageBox.Show($"{I18n.T("Error al crear nuevo presupuesto")}: {ex.Message}", I18n.T("Error"),
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

        private void btnAceptar_Click(object sender, EventArgs e)
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

                // NUEVO: Parsear los totales desde los TextBox
                decimal subtotalCalculado = 0;
                decimal ivaCalculado = 0;
                decimal totalCalculado = 0;

                if (!string.IsNullOrWhiteSpace(txtSUB.Text))
                {
                    decimal.TryParse(txtSUB.Text, out subtotalCalculado);
                }

                if (!string.IsNullOrWhiteSpace(txtIva.Text))
                {
                    decimal.TryParse(txtIva.Text, out ivaCalculado);
                }

                if (!string.IsNullOrWhiteSpace(txtTotal.Text))
                {
                    decimal.TryParse(txtTotal.Text, out totalCalculado);
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
                        IdListaPrecio = _idListaPrecioSeleccionada, // NUEVO: Puede ser null
                        Detalles = detallesValidos,
                        // NUEVO: Asignar totales calculados
                        Subtotal = subtotalCalculado,
                        TotalIva = ivaCalculado,
                        Total = totalCalculado
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
                    var presupuestoNuevo = new PresupuestoDTO
                    {
                        Numero = txtCotizacion.Text.Trim(),
                        FechaEmision = txtFecha.Value,
                        Estado = estadoPresupuesto,
                        FechaVencimiento = dtEntrega.Value,
                        IdCliente = _idClienteSeleccionado.Value,
                        IdVendedor = _idVendedorSeleccionado,
                        IdListaPrecio = _idListaPrecioSeleccionada, // NUEVO: Puede ser null
                        Detalles = detallesValidos,
                        // NUEVO: Asignar totales calculados
                        Subtotal = subtotalCalculado,
                        TotalIva = ivaCalculado,
                        Total = totalCalculado
                    };

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

                        detalle.IdProducto = producto.Id;
                        detalle.Codigo = producto.Codigo;
                        detalle.Descripcion = producto.Descripcion;
                        detalle.PorcentajeIVA = producto.PorcentajeIVA;

                        // NUEVO: Buscar el precio en la lista de precios seleccionada
                        decimal precioDefault = 0M;
                        if (_idListaPrecioSeleccionada.HasValue)
                        {
                            try
                            {
                                var precio = _listaPrecioService.ObtenerPrecioProducto(_idListaPrecioSeleccionada.Value, producto.Id);
                                
                                if (precio.HasValue)
                                {
                                    precioDefault = precio.Value;
                                }
                            }
                            catch
                            {
                                precioDefault = 0M;
                            }
                        }

                        detalle.Precio = precioDefault;
                        dgArticulos.Refresh();
                    }

                    return true;
                }

                return false;
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

                        decimal precioDefault = 0M;
                        if (_idListaPrecioSeleccionada.HasValue)
                        {
                            try
                            {
                                var precio = _listaPrecioService.ObtenerPrecioProducto(
                                    _idListaPrecioSeleccionada.Value, 
                                    productoSeleccionado.Id);
                                
                                if (precio.HasValue)
                                {
                                    precioDefault = precio.Value;
                                }
                            }
                            catch
                            {
                                // Si hay error al buscar el precio, mantener en 0
                                precioDefault = 0M;
                            }
                        }

                        if (rowIndex < _detalles.Count)
                        {
                            // Fila existente en _detalles
                            var detalle = _detalles[rowIndex];

                            detalle.IdProducto = productoSeleccionado.Id;
                            detalle.Codigo = productoSeleccionado.Codigo;
                            detalle.Descripcion = productoSeleccionado.Descripcion;
                            detalle.PorcentajeIVA = productoSeleccionado.PorcentajeIVA;
                            detalle.Precio = precioDefault;
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
                                Precio = precioDefault,
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
            try
            {
                // Si estábamos en modo nuevo sin presupuesto previo
                if (_modoEdicion && !_presupuestoActualId.HasValue)
                {
                    // Recargar todos los presupuestos
                    CargarPresupuestos();
                }
                else if (_modoEdicion && _presupuestoActualId.HasValue)
                {
                    // Si estábamos editando, recargar el presupuesto actual desde la base de datos
                    // Esto asegura que se descarten todos los cambios no guardados
                    var presupuestoOriginal = _presupuestoService.GetById(_presupuestoActualId.Value);

                    if (presupuestoOriginal != null)
                    {
                        // Actualizar en la lista local
                        var indice = _presupuestosCompletos.FindIndex(p => p.Id == _presupuestoActualId.Value);
                        if (indice >= 0)
                        {
                            _presupuestosCompletos[indice] = presupuestoOriginal;
                        }

                        // Mostrar el presupuesto original (esto recarga la grilla completa)
                        MostrarPresupuesto(presupuestoOriginal);
                    }
                    else
                    {
                        // Si no se encuentra, recargar todo
                        CargarPresupuestos();
                    }
                }

                // Restaurar estado inicial de controles
                ConfigurarEstadoInicial();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cancelar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            // Validar valores numéricos en columnas de cantidad, precio y descuento
            if (e.ColumnIndex == dgArticulos.Columns["Cantidad"].Index ||
                e.ColumnIndex == dgArticulos.Columns["Precio"].Index ||
                e.ColumnIndex == dgArticulos.Columns["Descuento"].Index)
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
            // Manejar Enter en DataGridView
            if (e.KeyCode == Keys.Enter && dgArticulos.CurrentCell != null)
            {
                dgArticulos.EndEdit();

                int col = dgArticulos.CurrentCell.ColumnIndex;
                int row = dgArticulos.CurrentCell.RowIndex;

                // Si estamos en la última columna
                if (col >= dgArticulos.ColumnCount - 1)
                {
                    // Si es la última fila (nueva fila), agregar el detalle y crear una nueva fila
                    if (dgArticulos.Rows[row].IsNewRow)
                    {
                        // La fila nueva ya se agregó automáticamente al BindingList
                        // Solo necesitamos movernos a la nueva fila
                        if (row + 1 < dgArticulos.RowCount)
                        {
                            dgArticulos.CurrentCell = dgArticulos[0, row + 1];
                            dgArticulos.BeginEdit(true);
                        }
                    }
                    else
                    {
                        // Ir a la siguiente fila o crear una nueva
                        if (row + 1 < dgArticulos.RowCount)
                        {
                            dgArticulos.CurrentCell = dgArticulos[0, row + 1];
                            dgArticulos.BeginEdit(true);
                        }
                    }
                }
                else
                {
                    // Moverse a la siguiente columna
                    dgArticulos.CurrentCell = dgArticulos[col + 1, row];
                    dgArticulos.BeginEdit(true);
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

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
            e.Row.Cells["Descuento"].Value = 0M;
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
        private void ValidarYMoverFocoCliente()
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
                var clientes = _clienteService.GetAll();
                var cliente = clientes.FirstOrDefault(c => c.CodigoCliente != null &&
                    c.CodigoCliente.Trim().Equals(codigo, StringComparison.OrdinalIgnoreCase));

                if (cliente != null)
                {
                    // Cliente encontrado - guardar IDs y aplicar datos
                    _idClienteSeleccionado = cliente.Id;
                    _idVendedorSeleccionado = cliente.IdVendedor;
                    _alicuotaArbaCliente = cliente.AlicuotaArba; // NUEVO

                    txtCodigoCliente.Text = cliente.CodigoCliente;
                    txtCliente.Text = cliente.RazonSocial;

                    // Cargar nombre del vendedor si tiene
                    if (cliente.IdVendedor.HasValue)
                    {
                        var vendedor = _vendedorService.GetById(cliente.IdVendedor.Value);
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

                    // Cargar condición de pago del cliente (también cargará AlicuotaArba y recalculará)
                    CargarCondicionPagoDelCliente(cliente.Id);

                    // Mover al siguiente control
                    this.SelectNextControl(txtCodigoCliente, true, true, true, true);
                }
                else
                {
                    // Cliente no encontrado - Mostrar selector
                    MostrarSelectorClienteDirecto();
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
            // Manejar Enter en DataGridView
            if ((dgArticulos.Focused || dgArticulos.EditingControl != null) && keyData == Keys.Enter)
            {
                if (dgArticulos.CurrentCell == null)
                    return base.ProcessCmdKey(ref msg, keyData);

                // 1. Capturamos el estado actual ANTES de confirmar
                int col = dgArticulos.CurrentCell.ColumnIndex;
                int row = dgArticulos.CurrentCell.RowIndex;
                bool esFilaNueva = dgArticulos.Rows[row].IsNewRow;

                // Si estamos en la columna de código
                if (col == 0)
                {
                    // Obtener el valor
                    var codigo = dgArticulos[col, row].Value?.ToString()?.Trim();

                    dgArticulos.EndEdit();

                    // *** SOLO agregar fila si REALMENTE es la fila nueva plantilla ***
                    if (esFilaNueva && dgArticulos.DataSource is BindingList<PresupuestoDetalleDTO> bindingList)
                    {
                        // Guardar referencia al DataSource
                        var dataSource = dgArticulos.DataSource;

                        // Desvincular temporalmente
                        dgArticulos.DataSource = null;

                        try
                        {
                            // Crear un nuevo DTO
                            var nuevoDetalle = new PresupuestoDetalleDTO();

                            // Asignar el código si existe
                            if (!string.IsNullOrWhiteSpace(codigo))
                            {
                                nuevoDetalle.Codigo = codigo;
                            }

                            // Agregar al BindingList
                            bindingList.Add(nuevoDetalle);
                        }
                        finally
                        {
                            // Revincular el DataSource
                            dgArticulos.DataSource = dataSource;
                        }

                        // *** IMPORTANTE: La fila que acabamos de agregar tiene el mismo índice 'row' ***
                        // NO necesitamos recalcular porque el DataGridView mantiene el índice
                        // La nueva fila plantilla será row + 1

                        // Posicionar en la fila recién creada (mantener el mismo índice)
                        dgArticulos.CurrentCell = dgArticulos[col, row];
                    }

                    // --- LÓGICA DE VALIDACIÓN ---
                    // En este punto, 'row' sigue apuntando a la fila correcta
                    if (!string.IsNullOrWhiteSpace(codigo))
                    {
                        bool productoEncontrado = BuscarYAplicarProductoPorCodigoEnter(codigo);
                        if (!productoEncontrado)
                        {
                            MostrarSelectorProducto();
                        }
                    }
                    else
                    {
                        MostrarSelectorProducto();
                    }

                    // Moverse a la siguiente columna de la MISMA fila
                    if (col < dgArticulos.ColumnCount - 1)
                    {
                        dgArticulos.CurrentCell = dgArticulos[col + 1, row];
                        dgArticulos.BeginEdit(true);
                    }
                }
                else if (col >= dgArticulos.ColumnCount - 1)
                {
                    // Última columna
                    dgArticulos.EndEdit();

                    // Obtener la fila actual después del EndEdit
                    int filaActual = dgArticulos.CurrentCell?.RowIndex ?? row;
                    bool eraFilaNueva = dgArticulos.Rows[filaActual].IsNewRow;

                    if (eraFilaNueva)
                    {
                        // Si estamos en la fila nueva plantilla, ir a ella
                        int nuevaPlantillaIndex = dgArticulos.Rows.Count - 1;

                        if (nuevaPlantillaIndex >= 0 && nuevaPlantillaIndex < dgArticulos.Rows.Count)
                        {
                            dgArticulos.CurrentCell = dgArticulos[0, nuevaPlantillaIndex];
                            dgArticulos.BeginEdit(true);
                        }
                    }
                    else if (filaActual + 1 < dgArticulos.RowCount)
                    {
                        // Si estamos en una fila normal, ir a la SIGUIENTE fila
                        dgArticulos.CurrentCell = dgArticulos[0, filaActual + 1];
                        dgArticulos.BeginEdit(true);
                    }
                }
                else
                {
                    // Columnas intermedias: siguiente columna en la MISMA fila
                    dgArticulos.EndEdit();
                    dgArticulos.CurrentCell = dgArticulos[col + 1, row];
                    dgArticulos.BeginEdit(true);
                }

                return true;
            }

            // Manejar Enter en TextBox de código de cliente
            if (txtCodigoCliente.Focused && keyData == Keys.Enter)
            {
                var codigo = txtCodigoCliente.Text.Trim();

                if (string.IsNullOrWhiteSpace(codigo))
                {
                    // Campo vacío - mostrar selector directamente
                    MostrarSelectorClienteDirecto();
                }
                else
                {
                    // Validar lista de precios
                    ValidarYMoverFocoCliente();
                }

                return true;
            }

            // Manejar Enter en TextBox de código de lista de precios
            if (txtCodigoListaPrecio.Focused && keyData == Keys.Enter)
            {
                var codigo = txtCodigoListaPrecio.Text.Trim();

                if (string.IsNullOrWhiteSpace(codigo))
                {
                    // Campo vacío - mostrar selector directamente
                    MostrarSelectorListaPrecio();
                }
                else
                {
                    // Validar lista de precios
                    ValidarYMoverFocoListaPrecio();
                }

                return true;
            }

            // Manejar Enter en otros controles editables (simular Tab)
            if (keyData == Keys.Enter)
            {
                Control controlActual = this.ActiveControl;

                if (controlActual is TextBox ||
                    controlActual is ComboBox ||
                    controlActual is DateTimePicker)
                {
                    this.SelectNextControl(controlActual, true, true, true, true);
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ============= EVENTOS DE LISTA DE PRECIOS =============

        /// <summary>
        /// Evento al presionar Enter en el campo de código de lista de precios
        /// </summary>
        private void txtCodigoListaPrecio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                var codigo = txtCodigoListaPrecio.Text.Trim();

                if (string.IsNullOrWhiteSpace(codigo))
                {
                    // Campo vacío - mostrar selector directamente
                    MostrarSelectorListaPrecio();
                }
                else
                {
                    // Validar lista de precios
                    ValidarYMoverFocoListaPrecio();
                }
            }
            else if (e.KeyCode == Keys.F1) // Usar F1 como atajo alternativopara selector
            {
                e.SuppressKeyPress = true;
                MostrarSelectorListaPrecio();
            }
        }

        /// <summary>
        /// Evento al salir del campo de código de lista de precios
        /// </summary>
        private void txtCodigoListaPrecio_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigoListaPrecio.Text))
            {
                txtListaPrecio.Clear();
                _idListaPrecioSeleccionada = null;
                return;
            }

            try
            {
                // Intentar buscar lista de precios por código
                var listaPrecios = _listaPrecioService.GetActivas();
                var lista = listaPrecios.FirstOrDefault(lp => lp.Codigo != null &&
                    lp.Codigo.Trim().Equals(txtCodigoListaPrecio.Text.Trim(), StringComparison.OrdinalIgnoreCase));

                if (lista != null)
                {
                    // Lista de precios encontrada - guardar ID y mostrar datos
                    _idListaPrecioSeleccionada = lista.Id;
                    txtCodigoListaPrecio.Text = lista.Codigo;
                    txtListaPrecio.Text = lista.Nombre;

                    // NUEVO: Almacenar el estado IncluyeIva
                    _listaPrecioIncluyeIva = lista.IncluyeIva;
                }
                else
                {
                    // Lista de precios no encontrada - Mostrar selector
                    MostrarSelectorListaPrecio();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar lista de precios: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Muestra el selector de listas de precios para que el usuario elija una
        /// </summary>
        private void MostrarSelectorListaPrecio()
        {
            try
            {
                // Obtener todas las listas de precios activas
                var listasPrecios = _listaPrecioService.GetActivas().ToList();

                if (!listasPrecios.Any())
                {
                    MessageBox.Show("No hay listas de precios disponibles", "Advertencia",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Configurar el selector de listas de precios
                var config = new SelectorConfig<ListaPrecioDTO>
                {
                    Titulo = "Seleccionar Lista de Precios",
                    Datos = listasPrecios,
                    PlaceholderBusqueda = "Buscar por código o nombre...",
                    PermitirSeleccionMultiple = false,
                    Columnas = new List<ColumnaConfig>
                    {
                        new ColumnaConfig
                        {
                            NombrePropiedad = "Codigo",
                            TituloColumna = "Código",
                            Ancho = 100,
                            Visible = true
                        },
                        new ColumnaConfig
                        {
                            NombrePropiedad = "Nombre",
                            TituloColumna = "Nombre",
                            Ancho = 300,
                            Visible = true
                        }
                    },
                    FuncionFiltro = (busqueda, lista) =>
                    {
                        var textoBusqueda = busqueda.ToUpper();
                        return (lista.Codigo != null && lista.Codigo.ToUpper().Contains(textoBusqueda)) ||
                               (lista.Nombre != null && lista.Nombre.ToUpper().Contains(textoBusqueda));
                    }
                };

                // Mostrar el selector
                var selector = frmSelector.Mostrar(config);

                if (selector.ShowDialog() == DialogResult.OK)
                {
                    var listaSeleccionada = selector.ElementoSeleccionado as ListaPrecioDTO;

                    if (listaSeleccionada != null)
                    {
                        // Guardar ID
                        _idListaPrecioSeleccionada = listaSeleccionada.Id;

                        // Aplicar la lista de precios seleccionada
                        txtCodigoListaPrecio.Text = listaSeleccionada.Codigo;
                        txtListaPrecio.Text = listaSeleccionada.Nombre;
                        
                        // NUEVO: Almacenar el estado IncluyeIva
                        _listaPrecioIncluyeIva = listaSeleccionada.IncluyeIva;
                    }
                }
                else
                {
                    // Usuario canceló - limpiar campos
                    txtCodigoListaPrecio.Clear();
                    txtListaPrecio.Clear();
                    _idListaPrecioSeleccionada = null;
                    _listaPrecioIncluyeIva = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al mostrar selector de listas de precios: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Valida la lista de precios ingresada y mueve el foco al siguiente control
        /// Si no es válida, muestra el selector
        /// </summary>
        private void ValidarYMoverFocoListaPrecio()
        {
            try
            {
                var codigo = txtCodigoListaPrecio.Text.Trim();

                if (string.IsNullOrWhiteSpace(codigo))
                {
                    txtListaPrecio.Clear();
                    _idListaPrecioSeleccionada = null;

                    // Mover al siguiente control
                    this.SelectNextControl(txtCodigoListaPrecio, true, true, true, true);
                    return;
                }

                // Intentar buscar lista de precios por código
                var listasPrecios = _listaPrecioService.GetActivas();
                var lista = listasPrecios.FirstOrDefault(lp => lp.Codigo != null &&
                    lp.Codigo.Trim().Equals(codigo, StringComparison.OrdinalIgnoreCase));

                if (lista != null)
                {
                    // Lista de precios encontrada - guardar ID y aplicar datos
                    _idListaPrecioSeleccionada = lista.Id;
                    txtCodigoListaPrecio.Text = lista.Codigo;
                    txtListaPrecio.Text = lista.Nombre;

                    // Mover al siguiente control
                    this.SelectNextControl(txtCodigoListaPrecio, true, true, true, true);
                }
                else
                {
                    // Lista de precios no encontrada - Mostrar selector
                    MostrarSelectorListaPrecio();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar lista de precios: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnImprimir_Click_1(object sender, EventArgs e)
        {
            ImprimirPresupuesto();
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