using BLL.Contracts;
using BLL.DTOs;
using Services.Services.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SistemaPresupuestario.Venta.Factura
{
    public partial class frmFacturar : Form
    {
        private readonly IPresupuestoService _presupuestoService;
        private readonly IClienteService _clienteService;
        private readonly IFacturacionService _facturacionService;
        private List<PresupuestoDTO> _presupuestosAprobados;
        private BindingList<PresupuestoFacturaViewModel> _viewModels;
        private Guid? _clienteSeleccionado = null;

        public frmFacturar(
            IPresupuestoService presupuestoService,
            IClienteService clienteService,
            IFacturacionService facturacionService)
        {
            InitializeComponent();
            _presupuestoService = presupuestoService ?? throw new ArgumentNullException(nameof(presupuestoService));
            _clienteService = clienteService ?? throw new ArgumentNullException(nameof(clienteService));
            _facturacionService = facturacionService ?? throw new ArgumentNullException(nameof(facturacionService));
            
            _presupuestosAprobados = new List<PresupuestoDTO>();
            _viewModels = new BindingList<PresupuestoFacturaViewModel>();
        }

        private void frmFacturar_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                ConfigurarGrilla();
                CargarPresupuestosAprobados();
                ConfigurarControles();
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

        private void ConfigurarControles()
        {
            btnObtenerCAE.Enabled = false;
            lblClienteSeleccionado.Text = "Cliente: (ninguno)";
            lblTotalSeleccionado.Text = "Total: $0.00";
            lblSubtotal.Text = "Subtotal (Neto): $0.00";
            lblIVA.Text = "IVA: $0.00";
            lblIIBBArba.Text = "IIBB ARBA: $0.00";
        }

        private void ConfigurarGrilla()
        {
            dgvPresupuestos.AutoGenerateColumns = false;
            dgvPresupuestos.AllowUserToAddRows = false;
            dgvPresupuestos.AllowUserToDeleteRows = false;
            dgvPresupuestos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPresupuestos.MultiSelect = false;
            dgvPresupuestos.ReadOnly = false;

            dgvPresupuestos.Columns.Clear();

            // Columna checkbox para selección
            var colSeleccionar = new DataGridViewCheckBoxColumn
            {
                Name = "Seleccionar",
                HeaderText = "Sel.",
                DataPropertyName = "Seleccionado",
                Width = 50,
                ReadOnly = false
            };
            dgvPresupuestos.Columns.Add(colSeleccionar);

            // Columna Número de Presupuesto
            var colNumero = new DataGridViewTextBoxColumn
            {
                Name = "Numero",
                HeaderText = "Número",
                DataPropertyName = "Numero",
                Width = 100,
                ReadOnly = true
            };
            dgvPresupuestos.Columns.Add(colNumero);

            // Columna Fecha Emisión
            var colFechaEmision = new DataGridViewTextBoxColumn
            {
                Name = "FechaEmision",
                HeaderText = "Fecha Emisión",
                DataPropertyName = "FechaEmision",
                Width = 100,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy"
                }
            };
            dgvPresupuestos.Columns.Add(colFechaEmision);

            // Columna Código Cliente
            var colCodigoCliente = new DataGridViewTextBoxColumn
            {
                Name = "CodigoCliente",
                HeaderText = "Código Cliente",
                DataPropertyName = "CodigoCliente",
                Width = 100,
                ReadOnly = true
            };
            dgvPresupuestos.Columns.Add(colCodigoCliente);

            // Columna Razón Social
            var colRazonSocial = new DataGridViewTextBoxColumn
            {
                Name = "RazonSocial",
                HeaderText = "Razón Social",
                DataPropertyName = "RazonSocial",
                Width = 250,
                ReadOnly = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            };
            dgvPresupuestos.Columns.Add(colRazonSocial);

            // Columna Vendedor
            var colVendedor = new DataGridViewTextBoxColumn
            {
                Name = "Vendedor",
                HeaderText = "Vendedor",
                DataPropertyName = "Vendedor",
                Width = 150,
                ReadOnly = true
            };
            dgvPresupuestos.Columns.Add(colVendedor);

            // Columna Total
            var colTotal = new DataGridViewTextBoxColumn
            {
                Name = "Total",
                HeaderText = "Total",
                DataPropertyName = "Total",
                Width = 100,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "C2"
                }
            };
            dgvPresupuestos.Columns.Add(colTotal);

            // Configurar eventos
            dgvPresupuestos.CellValueChanged += DgvPresupuestos_CellValueChanged;
            dgvPresupuestos.CurrentCellDirtyStateChanged += DgvPresupuestos_CurrentCellDirtyStateChanged;
        }

        private void CargarPresupuestosAprobados()
        {
            try
            {
                // Estado 3 = Aprobado
                _presupuestosAprobados = _presupuestoService.GetByEstado(3).ToList();

                _viewModels.Clear();
                foreach (var presupuesto in _presupuestosAprobados)
                {
                    _viewModels.Add(new PresupuestoFacturaViewModel
                    {
                        Id = presupuesto.Id,
                        Numero = presupuesto.Numero,
                        FechaEmision = presupuesto.FechaEmision,
                        IdCliente = presupuesto.IdCliente,
                        CodigoCliente = presupuesto.ClienteCodigoCliente ?? "",
                        RazonSocial = presupuesto.ClienteRazonSocial ?? "",
                        Vendedor = presupuesto.VendedorNombre ?? "",
                        Total = presupuesto.Total,
                        Subtotal = presupuesto.Subtotal,
                        TotalIva = presupuesto.TotalIva,
                        ImporteArba = presupuesto.ImporteArba,
                        Seleccionado = false
                    });
                }

                dgvPresupuestos.DataSource = _viewModels;

                lblTotalPresupuestos.Text = $"Total presupuestos: {_viewModels.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar presupuestos aprobados: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvPresupuestos_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvPresupuestos.IsCurrentCellDirty)
            {
                dgvPresupuestos.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void DgvPresupuestos_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            // Solo procesar cambios en la columna de checkbox
            if (dgvPresupuestos.Columns[e.ColumnIndex].Name != "Seleccionar")
                return;

            var viewModel = _viewModels[e.RowIndex];

            // Si está intentando marcar el checkbox
            if (viewModel.Seleccionado)
            {
                // Si ya hay un cliente seleccionado, validar que sea el mismo
                if (_clienteSeleccionado.HasValue)
                {
                    if (viewModel.IdCliente != _clienteSeleccionado.Value)
                    {
                        MessageBox.Show(
                            "No puede seleccionar presupuestos de diferentes clientes.\n\n" +
                            $"Cliente actual: {ObtenerNombreCliente(_clienteSeleccionado.Value)}\n" +
                            $"Presupuesto seleccionado: {viewModel.RazonSocial}",
                            "Validación",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        // Desmarcar el checkbox
                        viewModel.Seleccionado = false;
                        dgvPresupuestos.RefreshEdit();
                        return;
                    }
                }
                else
                {
                    // Establecer el cliente seleccionado
                    _clienteSeleccionado = viewModel.IdCliente;
                }
            }
            else
            {
                // Si está desmarcando, verificar si no quedan presupuestos seleccionados
                if (!_viewModels.Any(vm => vm.Seleccionado))
                {
                    _clienteSeleccionado = null;
                }
            }

            ActualizarInformacionSeleccion();
        }

        private void ActualizarInformacionSeleccion()
        {
            var seleccionados = _viewModels.Where(vm => vm.Seleccionado).ToList();

            if (seleccionados.Any())
            {
                var total = seleccionados.Sum(s => s.Total);
                var subtotal = seleccionados.Sum(s => s.Subtotal);
                var totalIva = seleccionados.Sum(s => s.TotalIva);
                var totalArba = seleccionados.Sum(s => s.ImporteArba);

                lblTotalSeleccionado.Text = $"Total: {total:C2}";
                lblSubtotal.Text = $"Subtotal (Neto): {subtotal:C2}";
                lblIVA.Text = $"IVA: {totalIva:C2}";
                lblIIBBArba.Text = $"IIBB ARBA: {totalArba:C2}";
                lblClienteSeleccionado.Text = $"Cliente: {seleccionados[0].RazonSocial}";
                btnObtenerCAE.Enabled = true;
            }
            else
            {
                lblTotalSeleccionado.Text = "Total: $0.00";
                lblSubtotal.Text = "Subtotal (Neto): $0.00";
                lblIVA.Text = "IVA: $0.00";
                lblIIBBArba.Text = "IIBB ARBA: $0.00";
                lblClienteSeleccionado.Text = "Cliente: (ninguno)";
                btnObtenerCAE.Enabled = false;
            }

            lblPresupuestosSeleccionados.Text = $"Seleccionados: {seleccionados.Count}";
        }

        private string ObtenerNombreCliente(Guid idCliente)
        {
            try
            {
                var cliente = _clienteService.GetById(idCliente);
                return cliente?.RazonSocial ?? "Desconocido";
            }
            catch
            {
                return "Desconocido";
            }
        }

        private void btnObtenerCAE_Click(object sender, EventArgs e)
        {
            var seleccionados = _viewModels.Where(vm => vm.Seleccionado).ToList();

            if (!seleccionados.Any())
            {
                MessageBox.Show("Debe seleccionar al menos un presupuesto para facturar", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Extraer IDs de presupuestos seleccionados
                var idsPresupuestos = seleccionados.Select(s => s.Id).ToArray();

                // Generar factura con CAE (MOCK)
                var factura = _facturacionService.GenerarFacturaConCAE(idsPresupuestos);

                if (!factura.Exitosa)
                {
                    MessageBox.Show(
                        $"Error al obtener el CAE:\n\n{factura.ErrorMessage}",
                        "Error de Facturación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                // Mostrar información de la factura generada
                var resultado = MessageBox.Show(
                    $"¡Factura generada exitosamente!\n\n" +
                    $"Tipo: Factura {factura.TipoFactura}\n" +
                    $"Número: {factura.NumeroFactura}\n" +
                    $"Cliente: {factura.ClienteRazonSocial}\n" +
                    $"Fecha: {factura.FechaEmision:dd/MM/yyyy}\n\n" +
                    $"CAE: {factura.CAE}\n" +
                    $"Vencimiento CAE: {factura.VencimientoCae?.ToString("dd/MM/yyyy") ?? "N/A"}\n\n" +
                    $"Presupuestos: {seleccionados.Count}\n" +
                    $"Subtotal (Neto): {factura.Subtotal:C2}\n" +
                    $"IVA: {factura.TotalIva:C2}\n" +
                    $"IIBB ARBA: {factura.ImporteArba:C2}\n" +
                    $"TOTAL: {factura.Total:C2}\n\n" +
                    $"¿Desea generar el PDF de la factura?",
                    "Factura Generada",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                if (resultado == DialogResult.Yes)
                {
                    // Generar y abrir PDF
                    _facturacionService.GenerarYAbrirPdfFactura(factura);

                    MessageBox.Show(
                        "PDF generado y abierto exitosamente",
                        "Éxito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                // NUEVO: Actualizar estado de presupuestos a "Facturado" (estado 6)
                ActualizarPresupuestosAFacturado(idsPresupuestos);

                // Actualizar la vista
                _clienteSeleccionado = null;
                CargarPresupuestosAprobados();
                ActualizarInformacionSeleccion();

                MessageBox.Show(
                    $"Los {seleccionados.Count} presupuesto(s) fueron marcados como Facturados exitosamente.",
                    "Información",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al procesar la facturación:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Actualiza el estado de los presupuestos a "Facturado" (estado 6)
        /// </summary>
        private void ActualizarPresupuestosAFacturado(Guid[] idsPresupuestos)
        {
            try
            {
                foreach (var idPresupuesto in idsPresupuestos)
                {
                    _presupuestoService.Facturar(idPresupuesto);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Error al actualizar el estado de los presupuestos a Facturado: {ex.Message}", 
                    ex);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                _clienteSeleccionado = null;
                CargarPresupuestosAprobados();
                ActualizarInformacionSeleccion();
                
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

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    /// <summary>
    /// Modelo de vista para mostrar presupuestos en la grilla de facturación
    /// </summary>
    public class PresupuestoFacturaViewModel
    {
        public Guid Id { get; set; }
        public string Numero { get; set; }
        public DateTime FechaEmision { get; set; }
        public Guid IdCliente { get; set; }
        public string CodigoCliente { get; set; }
        public string RazonSocial { get; set; }
        public string Vendedor { get; set; }
        public decimal Total { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalIva { get; set; }
        public decimal ImporteArba { get; set; }
        public bool Seleccionado { get; set; }
    }
}
