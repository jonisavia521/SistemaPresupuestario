using BLL.Contracts;
using BLL.DTOs;
using BLL.Enums;
using SistemaPresupuestario.Maestros.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        
        // Nuevo: Modo de operación del formulario
        private ModoPresupuesto _modoOperacion = ModoPresupuesto.Generar;

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
                case ModoPresupuesto.Generar:
                    this.Text = "Generar Cotización";
                    // Ocultar navegación, solo crear nuevos
                    btnPrimero.Visible = false;
                    btnAnterior.Visible = false;
                    btnSiguiente.Visible = false;
                    btnUltimo.Visible = false;
                    btnBuscar.Visible = false;
                    btnActualizar.Visible = false;
                    
                    // Solo botones de creación
                    btnModificar.Visible = false;
                    btnEliminar.Visible = false;
                    btnCopiar.Visible = false;
                    break;

                case ModoPresupuesto.Gestionar:
                    this.Text = "Gestión de Cotizaciones";
                    // Navegación completa habilitada
                    // Todos los botones visibles por defecto
                    
                    // Agregar botón "Emitir" si no existe
                    if (!tsMenu.Items.ContainsKey("btnEmitir"))
                    {
                        var btnEmitir = new ToolStripButton
                        {
                            Name = "btnEmitir",
                            Text = "Emitir",
                            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                            ToolTipText = "Emitir presupuesto para aprobación",
                            Image = btnNuevo.Image // Reutilizar icono
                        };
                        btnEmitir.Click += BtnEmitir_Click;
                        
                        // Insertar después de btnCopiar
                        int indexCopiar = tsMenu.Items.IndexOf(btnCopiar);
                        tsMenu.Items.Insert(indexCopiar + 1, btnEmitir);
                    }
                    break;

                case ModoPresupuesto.Aprobar:
                    this.Text = "Aprobar Cotizaciones";
                    // Navegación habilitada
                    
                    // Ocultar botones de edición
                    btnNuevo.Visible = false;
                    btnModificar.Visible = false;
                    btnEliminar.Visible = false;
                    btnCopiar.Visible = false;
                    
                    // Agregar botones específicos de aprobación
                    if (!tsMenu.Items.ContainsKey("btnAprobarPresupuesto"))
                    {
                        var separador = new ToolStripSeparator { Name = "separadorAprobacion" };
                        tsMenu.Items.Insert(tsMenu.Items.IndexOf(btnCancelar) + 1, separador);
                        
                        var btnAprobarPresupuesto = new ToolStripButton
                        {
                            Name = "btnAprobarPresupuesto",
                            Text = "Aprobar",
                            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                            ToolTipText = "Aprobar presupuesto seleccionado",
                            Image = btnAceptar.Image,
                            BackColor = System.Drawing.Color.LightGreen
                        };
                        btnAprobarPresupuesto.Click += BtnAprobarPresupuesto_Click;
                        
                        var btnRechazarPresupuesto = new ToolStripButton
                        {
                            Name = "btnRechazarPresupuesto",
                            Text = "Rechazar",
                            DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                            ToolTipText = "Rechazar presupuesto seleccionado",
                            Image = btnCancelar.Image,
                            BackColor = System.Drawing.Color.LightCoral
                        };
                        btnRechazarPresupuesto.Click += BtnRechazarPresupuesto_Click;
                        
                        tsMenu.Items.Insert(tsMenu.Items.IndexOf(separador) + 1, btnAprobarPresupuesto);
                        tsMenu.Items.Insert(tsMenu.Items.IndexOf(btnAprobarPresupuesto) + 1, btnRechazarPresupuesto);
                    }
                    break;
            }
        }

        private void ConfigurarControles()
        {
            // Configurar ComboBox de Estado
            cmbEstado.Items.Clear();
            cmbEstado.Items.Add(new { Value = 0, Text = "Borrador" });
            cmbEstado.Items.Add(new { Value = 1, Text = "Emitido" });
            cmbEstado.Items.Add(new { Value = 2, Text = "Aprobado" });
            cmbEstado.Items.Add(new { Value = 3, Text = "Rechazado" });
            cmbEstado.Items.Add(new { Value = 4, Text = "Vencido" });
            cmbEstado.DisplayMember = "Text";
            cmbEstado.ValueMember = "Value";
            cmbEstado.SelectedIndex = 0;

            // Configurar ComboBox de plazo de entrega
            comboBox1.SelectedIndex = 0; // Cantidad: 1
            comboBox2.SelectedIndex = 0; // Unidad: Día/s
            comboBox3.SelectedIndex = 0; // A partir de: Firma de contrato

            // Configurar fechas
            txtFecha.Value = DateTime.Now;
            dtEntrega.Value = DateTime.Now.AddDays(7); // 7 días por defecto
        }

        private void ConfigurarGrilla()
        {
            dgArticulos.AutoGenerateColumns = false;
            dgArticulos.DataSource = _detalles;

            // Configurar eventos de la grilla
            dgArticulos.CellEndEdit += DgArticulos_CellEndEdit;
            dgArticulos.UserDeletingRow += DgArticulos_UserDeletingRow;
            dgArticulos.KeyDown += DgArticulos_KeyDown;
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
                    case ModoPresupuesto.Generar:
                        // En modo Generar, no cargamos presupuestos existentes
                        // El usuario solo puede crear nuevos
                        _presupuestosCompletos = new List<PresupuestoDTO>();
                        LimpiarFormulario();
                        
                        // Iniciar directamente en modo nuevo
                        btnNuevo_Click(null, EventArgs.Empty);
                        break;

                    case ModoPresupuesto.Gestionar:
                        // Mostrar: Borrador (0), Aprobado (2), Rechazado (3), Vencido (4)
                        // NO mostrar Emitido (1) - esos están para aprobación
                        _presupuestosCompletos = _presupuestoService
                            .GetByEstados(0, 2, 3, 4)
                            .ToList();
                        
                        if (_presupuestosCompletos.Any())
                        {
                            _indiceActual = 0;
                            MostrarPresupuesto(_presupuestosCompletos[_indiceActual]);
                        }
                        else
                        {
                            LimpiarFormulario();
                            MessageBox.Show("No hay cotizaciones para gestionar", "Información",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        break;

                    case ModoPresupuesto.Aprobar:
                        // Solo mostrar presupuestos Emitidos (1)
                        _presupuestosCompletos = _presupuestoService
                            .GetByEstado(1)
                            .ToList();
                        
                        if (_presupuestosCompletos.Any())
                        {
                            _indiceActual = 0;
                            MostrarPresupuesto(_presupuestosCompletos[_indiceActual]);
                        }
                        else
                        {
                            LimpiarFormulario();
                            MessageBox.Show("No hay cotizaciones pendientes de aprobación", "Información",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            // Cargar form de pago (vacío por ahora, se puede extender)
            txtCodigoFormaPago.Text = "";
            txtFormaPago.Text = "";

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

        private void LimpiarFormulario()
        {
            _presupuestoActualId = null;
            txtCotizacion.Text = "NUEVO";
            txtFecha.Value = DateTime.Now;
            cmbEstado.SelectedIndex = 0;
            dtEntrega.Value = DateTime.Now.AddDays(7);

            txtCodigoCliente.Clear();
            txtCliente.Clear();
            txtVendedor.Clear();
            txtCodigoFormaPago.Clear();
            txtFormaPago.Clear();

            _detalles.Clear();
            CalcularTotales();
        }

        private void HabilitarControles(bool habilitar)
        {
            txtCodigoCliente.Enabled = habilitar;
            txtCodigoFormaPago.Enabled = habilitar;
            comboBox1.Enabled = habilitar;
            comboBox2.Enabled = habilitar;
            comboBox3.Enabled = habilitar;
            txtFecha.Enabled = habilitar;
            dtEntrega.Enabled = habilitar;
            dgArticulos.Enabled = habilitar;
            dgArticulos.ReadOnly = !habilitar;
        }

        private void HabilitarBotones(bool navegacion, bool edicion, bool guardar)
        {
            btnPrimero.Enabled = navegacion;
            btnAnterior.Enabled = navegacion;
            btnSiguiente.Enabled = navegacion;
            btnUltimo.Enabled = navegacion;
            btnBuscar.Enabled = navegacion;

            // Lógica específica según modo y estado del presupuesto
            if (_modoOperacion == ModoPresupuesto.Gestionar && _presupuestoActualId.HasValue)
            {
                var presupuesto = _presupuestosCompletos.FirstOrDefault(p => p.Id == _presupuestoActualId.Value);
                if (presupuesto != null)
                {
                    // Solo permitir editar/eliminar Borradores (0)
                    bool esBorrador = presupuesto.Estado == 0;
                    btnModificar.Enabled = edicion && esBorrador;
                    btnEliminar.Enabled = edicion && esBorrador;
                    
                    // Copiar siempre habilitado si hay presupuesto
                    btnCopiar.Enabled = edicion;
                    
                    // Emitir solo habilitado para Borradores
                    var btnEmitir = tsMenu.Items.Find("btnEmitir", false).FirstOrDefault() as ToolStripButton;
                    if (btnEmitir != null)
                    {
                        btnEmitir.Enabled = edicion && esBorrador && presupuesto.Detalles != null && presupuesto.Detalles.Any();
                    }
                }
                else
                {
                    btnModificar.Enabled = false;
                    btnEliminar.Enabled = false;
                    btnCopiar.Enabled = false;
                }
            }
            else if (_modoOperacion == ModoPresupuesto.Aprobar && _presupuestoActualId.HasValue)
            {
                // En modo aprobar, habilitar botones de aprobación/rechazo
                var btnAprobarPresupuesto = tsMenu.Items.Find("btnAprobarPresupuesto", false).FirstOrDefault() as ToolStripButton;
                var btnRechazarPresupuesto = tsMenu.Items.Find("btnRechazarPresupuesto", false).FirstOrDefault() as ToolStripButton;
                
                if (btnAprobarPresupuesto != null)
                    btnAprobarPresupuesto.Enabled = true;
                    
                if (btnRechazarPresupuesto != null)
                    btnRechazarPresupuesto.Enabled = true;
            }
            else
            {
                // Modo generar o sin presupuesto seleccionado
                btnNuevo.Enabled = edicion && _modoOperacion == ModoPresupuesto.Generar;
                btnModificar.Enabled = false;
                btnEliminar.Enabled = false;
                btnCopiar.Enabled = false;
            }

            btnAceptar.Enabled = guardar;
            btnCancelar.Enabled = guardar;
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
                // Validar estado según el modo
                if (presupuesto.Estado == 2) // Aprobado
                {
                    MessageBox.Show("No se puede modificar un presupuesto aprobado", "Advertencia",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (presupuesto.Estado == 1 && _modoOperacion == ModoPresupuesto.Gestionar) // Emitido
                {
                    MessageBox.Show("No se puede modificar un presupuesto emitido.\n" +
                        "Debe rechazarlo primero para editarlo nuevamente.", "Advertencia",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (presupuesto.Estado != 0 && _modoOperacion == ModoPresupuesto.Gestionar) // No es borrador
                {
                    MessageBox.Show("Solo se pueden modificar presupuestos en estado Borrador", "Advertencia",
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

            var result = MessageBox.Show("¿Está seguro de eliminar este presupuesto?", "Confirmar eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    
                    _presupuestoService.Delete(_presupuestoActualId.Value);
                    
                    MessageBox.Show("Presupuesto eliminado exitosamente", "Éxito",
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
                if (!ValidarFormulario())
                    return;

                this.Cursor = Cursors.WaitCursor;

                var presupuestoDto = ConstruirPresupuestoDTO();

                bool resultado;
                if (_presupuestoActualId.HasValue)
                {
                    presupuestoDto.Id = _presupuestoActualId.Value;
                    resultado = _presupuestoService.Update(presupuestoDto);
                }
                else
                {
                    resultado = _presupuestoService.Add(presupuestoDto);
                }

                if (resultado)
                {
                    MessageBox.Show("Presupuesto guardado exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    _modoEdicion = false;
                    CargarPresupuestos();
                    
                    // Buscar el presupuesto guardado
                    if (!_presupuestoActualId.HasValue && presupuestoDto.Id != Guid.Empty)
                    {
                        _presupuestoActualId = presupuestoDto.Id;
                    }
                    
                    _indiceActual = _presupuestosCompletos.FindIndex(p => p.Id == _presupuestoActualId);
                    if (_indiceActual >= 0)
                    {
                        MostrarPresupuesto(_presupuestosCompletos[_indiceActual]);
                    }

                    HabilitarControles(false);
                    HabilitarBotones(navegacion: true, edicion: true, guardar: false);
                }
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

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            _modoEdicion = false;
            
            if (_presupuestoActualId.HasValue && _indiceActual >= 0 && _indiceActual < _presupuestosCompletos.Count)
            {
                MostrarPresupuesto(_presupuestosCompletos[_indiceActual]);
            }
            else
            {
                LimpiarFormulario();
            }

            HabilitarControles(false);
            HabilitarBotones(navegacion: true, edicion: true, guardar: false);
        }

        // ============= NAVEGACIÓN =============

        private void btnPrimero_Click(object sender, EventArgs e)
        {
            if (_presupuestosCompletos.Any())
            {
                _indiceActual = 0;
                MostrarPresupuesto(_presupuestosCompletos[_indiceActual]);
            }
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (_indiceActual > 0)
            {
                _indiceActual--;
                MostrarPresupuesto(_presupuestosCompletos[_indiceActual]);
            }
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (_indiceActual < _presupuestosCompletos.Count - 1)
            {
                _indiceActual++;
                MostrarPresupuesto(_presupuestosCompletos[_indiceActual]);
            }
        }

        private void btnUltimo_Click(object sender, EventArgs e)
        {
            if (_presupuestosCompletos.Any())
            {
                _indiceActual = _presupuestosCompletos.Count - 1;
                MostrarPresupuesto(_presupuestosCompletos[_indiceActual]);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            // Implementar búsqueda personalizada si se requiere
            MessageBox.Show("Función de búsqueda pendiente de implementación", "Información",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarPresupuestos();
        }

        // ============= BOTONES ESPECÍFICOS SEGÚN MODO =============

        /// <summary>
        /// Emite un presupuesto (cambia estado de Borrador a Emitido)
        /// Solo disponible en modo Gestionar para presupuestos en estado Borrador
        /// </summary>
        private void BtnEmitir_Click(object sender, EventArgs e)
        {
            if (!_presupuestoActualId.HasValue)
            {
                MessageBox.Show("No hay un presupuesto seleccionado", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var presupuesto = _presupuestosCompletos.FirstOrDefault(p => p.Id == _presupuestoActualId.Value);
            if (presupuesto == null)
                return;

            if (presupuesto.Estado != 0)
            {
                MessageBox.Show("Solo se pueden emitir presupuestos en estado Borrador", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (presupuesto.Detalles == null || !presupuesto.Detalles.Any())
            {
                MessageBox.Show("No se puede emitir un presupuesto sin artículos", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                "¿Está seguro de emitir este presupuesto?\n\n" +
                "Una vez emitido, quedará pendiente de aprobación y no podrá ser editado.",
                "Confirmar emisión",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    _presupuestoService.Emitir(_presupuestoActualId.Value);

                    MessageBox.Show("Presupuesto emitido exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Recargar - el presupuesto emitido ya no aparecerá en Gestionar
                    CargarPresupuestos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al emitir presupuesto: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        /// <summary>
        /// Aprueba un presupuesto emitido
        /// Solo disponible en modo Aprobar para presupuestos en estado Emitido
        /// </summary>
        private void BtnAprobarPresupuesto_Click(object sender, EventArgs e)
        {
            if (!_presupuestoActualId.HasValue)
            {
                MessageBox.Show("No hay un presupuesto seleccionado", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var presupuesto = _presupuestosCompletos.FirstOrDefault(p => p.Id == _presupuestoActualId.Value);
            if (presupuesto == null)
                return;

            if (presupuesto.Estado != 1)
            {
                MessageBox.Show("Solo se pueden aprobar presupuestos en estado Emitido", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"¿Está seguro de APROBAR este presupuesto?\n\n" +
                $"Número: {presupuesto.Numero}\n" +
                $"Cliente: {presupuesto.ClienteRazonSocial}\n" +
                $"Total: ${presupuesto.Total:N2}\n\n" +
                "Una vez aprobado, no podrá ser modificado.",
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

                    // Recargar - el presupuesto aprobado ya no aparecerá en Aprobar
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
        /// Rechaza un presupuesto emitido
        /// Solo disponible en modo Aprobar para presupuestos en estado Emitido
        /// </summary>
        private void BtnRechazarPresupuesto_Click(object sender, EventArgs e)
        {
            if (!_presupuestoActualId.HasValue)
            {
                MessageBox.Show("No hay un presupuesto seleccionado", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var presupuesto = _presupuestosCompletos.FirstOrDefault(p => p.Id == _presupuestoActualId.Value);
            if (presupuesto == null)
                return;

            if (presupuesto.Estado != 1)
            {
                MessageBox.Show("Solo se pueden rechazar presupuestos en estado Emitido", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"¿Está seguro de RECHAZAR este presupuesto?\n\n" +
                $"Número: {presupuesto.Numero}\n" +
                $"Cliente: {presupuesto.ClienteRazonSocial}\n" +
                $"Total: ${presupuesto.Total:N2}\n\n" +
                "El presupuesto quedará marcado como rechazado.",
                "Confirmar rechazo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    _presupuestoService.Rechazar(_presupuestoActualId.Value);

                    MessageBox.Show("Presupuesto rechazado", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Recargar - el presupuesto rechazado ya no aparecerá en Aprobar
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

        // ============= SELECTORES =============

        private void txtCodigoCliente_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCodigoCliente.Text))
                return;

            AbrirSelectorCliente();
        }

        private void txtCodigoCliente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3)
            {
                e.Handled = true;
                AbrirSelectorCliente();
            }
        }

        private async void AbrirSelectorCliente()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                var clientes = await _clienteService.GetActivosAsync();
                var clientesList = clientes.ToList();

                if (!clientesList.Any())
                {
                    MessageBox.Show("No hay clientes disponibles", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var config = new SelectorConfig<ClienteDTO>
                {
                    Titulo = "Seleccionar Cliente",
                    Datos = clientesList,
                    PlaceholderBusqueda = "Buscar por código, razón social o CUIT...",
                    Columnas = new List<ColumnaConfig>
                    {
                        new ColumnaConfig { NombrePropiedad = "CodigoCliente", TituloColumna = "Código", Ancho = 100 },
                        new ColumnaConfig { NombrePropiedad = "RazonSocial", TituloColumna = "Razón Social", Ancho = 300 },
                        new ColumnaConfig { NombrePropiedad = "NumeroDocumento", TituloColumna = "CUIT", Ancho = 120 },
                        new ColumnaConfig { NombrePropiedad = "TipoIva", TituloColumna = "Tipo IVA", Ancho = 150 }
                    },
                    FuncionFiltro = (busqueda, cliente) =>
                    {
                        var b = busqueda.ToUpper();
                        return cliente.CodigoCliente.ToUpper().Contains(b) ||
                               cliente.RazonSocial.ToUpper().Contains(b) ||
                               cliente.NumeroDocumento.Contains(b);
                    }
                };

                var selector = frmSelector.Mostrar(config);
                
                if (selector.ShowDialog() == DialogResult.OK)
                {
                    var clienteSeleccionado = (ClienteDTO)selector.ElementoSeleccionado;
                    
                    txtCodigoCliente.Text = clienteSeleccionado.CodigoCliente;
                    txtCliente.Text = clienteSeleccionado.RazonSocial;

                    // Cargar vendedor del cliente si tiene
                    if (clienteSeleccionado.IdVendedor.HasValue)
                    {
                        var vendedor = await _vendedorService.GetByIdAsync(clienteSeleccionado.IdVendedor.Value);
                        if (vendedor != null)
                        {
                            txtVendedor.Text = vendedor.Nombre;
                        }
                    }
                    else
                    {
                        txtVendedor.Clear();
                    }

                    // Establecer condición de pago si la tiene
                    if (!string.IsNullOrEmpty(clienteSeleccionado.CondicionPago))
                    {
                        txtCodigoFormaPago.Text = clienteSeleccionado.CondicionPago;
                        // Aquí podrías cargar la descripción de la condición de pago
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar cliente: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        // ============= GRILLA DE DETALLES =============

        private void DgArticulos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F3 && dgArticulos.CurrentCell != null)
            {
                if (dgArticulos.CurrentCell.ColumnIndex == 0) // Columna Código
                {
                    e.Handled = true;
                    AbrirSelectorProducto();
                }
            }
            else if (e.KeyCode == Keys.Delete && dgArticulos.CurrentRow != null)
            {
                // La eliminación se maneja en UserDeletingRow
            }
        }

        private async void AbrirSelectorProducto()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                var productos = await _productoService.GetActivosAsync();
                var productosList = productos.ToList();

                if (!productosList.Any())
                {
                    MessageBox.Show("No hay productos disponibles", "Información",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var config = new SelectorConfig<ProductoDTO>
                {
                    Titulo = "Seleccionar Producto",
                    Datos = productosList,
                    PlaceholderBusqueda = "Buscar por código o descripción...",
                    Columnas = new List<ColumnaConfig>
                    {
                        new ColumnaConfig { NombrePropiedad = "Codigo", TituloColumna = "Código", Ancho = 100 },
                        new ColumnaConfig { NombrePropiedad = "Descripcion", TituloColumna = "Descripción", Ancho = 400 },
                        new ColumnaConfig { NombrePropiedad = "PorcentajeIVA", TituloColumna = "% IVA", Ancho = 80 }
                    },
                    FuncionFiltro = (busqueda, producto) =>
                    {
                        var b = busqueda.ToUpper();
                        return producto.Codigo.ToUpper().Contains(b) ||
                               (producto.Descripcion?.ToUpper().Contains(b) ?? false);
                    }
                };

                var selector = frmSelector.Mostrar(config);
                
                if (selector.ShowDialog() == DialogResult.OK)
                {
                    var productoSeleccionado = (ProductoDTO)selector.ElementoSeleccionado;
                    AgregarProductoAGrilla(productoSeleccionado);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar producto: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void AgregarProductoAGrilla(ProductoDTO producto)
        {
            var nuevoDetalle = new PresupuestoDetalleDTO
            {
                Id = Guid.NewGuid(),
                IdProducto = producto.Id,
                Codigo = producto.Codigo,
                Descripcion = producto.Descripcion,
                Cantidad = 1,
                Precio = 0, // El usuario debe ingresar el precio
                Descuento = 0,
                PorcentajeIVA = producto.PorcentajeIVA,
                Renglon = _detalles.Count + 1
            };

            _detalles.Add(nuevoDetalle);
            CalcularTotales();

            // Enfocar la celda de cantidad para que el usuario pueda editarla
            dgArticulos.CurrentCell = dgArticulos.Rows[dgArticulos.Rows.Count - 1].Cells["Cantidad"];
            dgArticulos.BeginEdit(true);
        }

        private void DgArticulos_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _detalles.Count)
                return;

            var detalle = _detalles[e.RowIndex];

            // Validar cantidad
            if (detalle.Cantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser mayor a cero", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                detalle.Cantidad = 1;
            }

            // Validar precio
            if (detalle.Precio < 0)
            {
                MessageBox.Show("El precio no puede ser negativo", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                detalle.Precio = 0;
            }

            CalcularTotales();
            dgArticulos.Refresh();
        }

        private void DgArticulos_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            var result = MessageBox.Show("¿Está seguro de eliminar este artículo?", "Confirmar eliminación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                // Recalcular después de eliminar
                this.BeginInvoke(new Action(() => {
                    ReordenarRenglones();
                    CalcularTotales();
                }));
            }
        }

        private void ReordenarRenglones()
        {
            for (int i = 0; i < _detalles.Count; i++)
            {
                _detalles[i].Renglon = i + 1;
            }
        }

        private void CalcularTotales()
        {
            decimal subtotal = 0;
            decimal totalIva = 0;

            foreach (var detalle in _detalles)
            {
                subtotal += detalle.Total;
                totalIva += detalle.Iva;
            }

            decimal total = subtotal + totalIva;

            txtSUB.Text = subtotal.ToString("N2");
            txtIva.Text = totalIva.ToString("N2");
            txtTotal.Text = total.ToString("N2");
        }

        // ============= VALIDACIÓN Y CONSTRUCCIÓN DEL DTO =============

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtCodigoCliente.Text))
            {
                MessageBox.Show("Debe seleccionar un cliente", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCodigoCliente.Focus();
                return false;
            }

            if (!_detalles.Any())
            {
                MessageBox.Show("Debe agregar al menos un artículo al presupuesto", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validar que todos los detalles tengan precio y cantidad válidos
            foreach (var detalle in _detalles)
            {
                if (detalle.Cantidad <= 0)
                {
                    MessageBox.Show($"El artículo '{detalle.Codigo}' tiene una cantidad inválida", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (detalle.Precio <= 0)
                {
                    MessageBox.Show($"El artículo '{detalle.Codigo}' debe tener un precio mayor a cero", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        private PresupuestoDTO ConstruirPresupuestoDTO()
        {
            // Obtener el cliente por código
            var clienteCodigo = txtCodigoCliente.Text;
            var clientesTask = _clienteService.GetAllAsync().Result;
            var cliente = clientesTask.FirstOrDefault(c => c.CodigoCliente == clienteCodigo);

            if (cliente == null)
                throw new InvalidOperationException("Cliente no encontrado");

            // Obtener vendedor si existe
            Guid? idVendedor = null;
            if (cliente.IdVendedor.HasValue)
            {
                idVendedor = cliente.IdVendedor.Value;
            }

            var presupuestoDto = new PresupuestoDTO
            {
                Numero = txtCotizacion.Text,
                IdCliente = cliente.Id,
                FechaEmision = txtFecha.Value,
                Estado = cmbEstado.SelectedValue != null ? (int)cmbEstado.SelectedValue : 0,
                FechaVencimiento = dtEntrega.Value,
                IdVendedor = idVendedor,
                Detalles = _detalles.ToList()
            };

            return presupuestoDto;
        }
    }
}
