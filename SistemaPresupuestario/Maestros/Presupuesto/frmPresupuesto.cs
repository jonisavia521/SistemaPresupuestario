using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros.Presupuesto
{
    public partial class frmPresupuesto : Form
    {
        private List<PresupuestoItem> _presupuestos;
        private int _currentIndex = -1;

        public frmPresupuesto()
        {
            InitializeComponent();
            _presupuestos = new List<PresupuestoItem>();
        }

        private void frmPresupuesto_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Cargar estados del presupuesto
                CargarEstados();

                // Configurar estado inicial de los botones
                ConfigurarBotones(false);

                // Cargar datos iniciales
                CargarDatos();
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

        private void CargarEstados()
        {
            cmbEstado.Items.Clear();
            cmbEstado.Items.Add("Pendiente");
            cmbEstado.Items.Add("Aprobado");
            cmbEstado.Items.Add("Rechazado");
            cmbEstado.Items.Add("Anulado");
            cmbEstado.SelectedIndex = 0;
        }

        private void CargarDatos()
        {
            // TODO: Implementar carga de datos desde la base de datos
            // Por ahora, crear un presupuesto de ejemplo
            if (_presupuestos.Count == 0)
            {
                // Datos de ejemplo
                var presupuesto = new PresupuestoItem
                {
                    Numero = "0001",
                    Fecha = DateTime.Now,
                    Cliente = "",
                    Vendedor = "",
                    Estado = "Pendiente"
                };
                _presupuestos.Add(presupuesto);
                _currentIndex = 0;
            }

            if (_currentIndex >= 0 && _currentIndex < _presupuestos.Count)
            {
                MostrarPresupuesto(_presupuestos[_currentIndex]);
            }

            ActualizarBotonesNavegacion();
        }

        private void MostrarPresupuesto(PresupuestoItem presupuesto)
        {
            txtCotizacion.Text = presupuesto.Numero;
            txtFecha.Value = presupuesto.Fecha;
            txtCliente.Text = presupuesto.Cliente;
            txtVendedor.Text = presupuesto.Vendedor;
            txtSolicitante.Text = presupuesto.Solicitante;
            txtFormaPago.Text = presupuesto.FormaPago;
            txtPlazoEntrega.Text = presupuesto.PlazoEntrega;
            dtEntrega.Value = presupuesto.FechaVencimiento ?? DateTime.Now;
            cmbEstado.SelectedItem = presupuesto.Estado;

            // Cargar detalles en el grid
            dgArticulos.Rows.Clear();
            // TODO: Cargar artículos del presupuesto

            CalcularTotales();
        }

        private void CalcularTotales()
        {
            decimal subtotal = 0;
            decimal totalKG = 0;

            foreach (DataGridViewRow row in dgArticulos.Rows)
            {
                if (row.Cells["Total"].Value != null)
                {
                    decimal total = Convert.ToDecimal(row.Cells["Total"].Value);
                    subtotal += total;
                }
            }

            txtSUB.Text = subtotal.ToString("N2");

            // Calcular IVA (21%)
            decimal iva = subtotal * 0.21m;
            txtIva.Text = iva.ToString("N2");

            // Calcular IIBB (si aplica)
            decimal iibb = 0;
            txtIIBB.Text = iibb.ToString("N2");

            // Total
            decimal total = subtotal + iva + iibb;
            txtTotal.Text = total.ToString("N2");

            txtTotalKG.Text = totalKG.ToString("N2");
        }

        private void ConfigurarBotones(bool modoEdicion)
        {
            btnAceptar.Enabled = modoEdicion;
            btnCancelar.Enabled = modoEdicion;
            btnNuevo.Enabled = !modoEdicion;
            btnModificar.Enabled = !modoEdicion;
            btnEliminar.Enabled = !modoEdicion;
            btnCopiar.Enabled = !modoEdicion;
            btnPrimero.Enabled = !modoEdicion;
            btnAnterior.Enabled = !modoEdicion;
            btnSiguiente.Enabled = !modoEdicion;
            btnUltimo.Enabled = !modoEdicion;
            btnBuscar.Enabled = !modoEdicion;
            btnImprimir.Enabled = !modoEdicion;

            // Habilitar/deshabilitar controles de edición
            groupBox1.Enabled = modoEdicion;
            dgArticulos.ReadOnly = !modoEdicion;
        }

        private void ActualizarBotonesNavegacion()
        {
            if (_presupuestos.Count == 0)
            {
                btnPrimero.Enabled = false;
                btnAnterior.Enabled = false;
                btnSiguiente.Enabled = false;
                btnUltimo.Enabled = false;
                return;
            }

            btnPrimero.Enabled = _currentIndex > 0;
            btnAnterior.Enabled = _currentIndex > 0;
            btnSiguiente.Enabled = _currentIndex < _presupuestos.Count - 1;
            btnUltimo.Enabled = _currentIndex < _presupuestos.Count - 1;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            ConfigurarBotones(true);
            LimpiarFormulario();
            txtCotizacion.Text = GenerarNumeroPresupuesto();
            txtCliente.Focus();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (_currentIndex < 0 || _currentIndex >= _presupuestos.Count)
            {
                MessageBox.Show("No hay presupuesto seleccionado", "Información", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ConfigurarBotones(true);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_currentIndex < 0 || _currentIndex >= _presupuestos.Count)
            {
                MessageBox.Show("No hay presupuesto seleccionado", "Información", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show("¿Está seguro de eliminar el presupuesto?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    // TODO: Implementar eliminación en base de datos
                    _presupuestos.RemoveAt(_currentIndex);
                    
                    if (_currentIndex >= _presupuestos.Count)
                        _currentIndex = _presupuestos.Count - 1;

                    if (_currentIndex >= 0)
                        MostrarPresupuesto(_presupuestos[_currentIndex]);
                    else
                        LimpiarFormulario();

                    ActualizarBotonesNavegacion();
                    MessageBox.Show("Presupuesto eliminado exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar: {ex.Message}", "Error",
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
            if (_currentIndex < 0 || _currentIndex >= _presupuestos.Count)
            {
                MessageBox.Show("No hay presupuesto seleccionado", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ConfigurarBotones(true);
            txtCotizacion.Text = GenerarNumeroPresupuesto();
            txtFecha.Value = DateTime.Now;
            cmbEstado.SelectedIndex = 0;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar datos
                if (string.IsNullOrWhiteSpace(txtCliente.Text))
                {
                    MessageBox.Show("Debe seleccionar un cliente", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtCliente.Focus();
                    return;
                }

                this.Cursor = Cursors.WaitCursor;

                // TODO: Implementar guardado en base de datos
                var presupuesto = new PresupuestoItem
                {
                    Numero = txtCotizacion.Text,
                    Fecha = txtFecha.Value,
                    Cliente = txtCliente.Text,
                    Vendedor = txtVendedor.Text,
                    Solicitante = txtSolicitante.Text,
                    FormaPago = txtFormaPago.Text,
                    PlazoEntrega = txtPlazoEntrega.Text,
                    FechaVencimiento = dtEntrega.Value,
                    Estado = cmbEstado.SelectedItem?.ToString() ?? "Pendiente"
                };

                // Si es nuevo, agregar a la lista
                if (_currentIndex < 0 || _currentIndex >= _presupuestos.Count)
                {
                    _presupuestos.Add(presupuesto);
                    _currentIndex = _presupuestos.Count - 1;
                }
                else
                {
                    _presupuestos[_currentIndex] = presupuesto;
                }

                ConfigurarBotones(false);
                ActualizarBotonesNavegacion();

                MessageBox.Show("Presupuesto guardado exitosamente", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            ConfigurarBotones(false);
            if (_currentIndex >= 0 && _currentIndex < _presupuestos.Count)
                MostrarPresupuesto(_presupuestos[_currentIndex]);
            else
                LimpiarFormulario();
        }

        private void btnPrimero_Click(object sender, EventArgs e)
        {
            if (_presupuestos.Count > 0)
            {
                _currentIndex = 0;
                MostrarPresupuesto(_presupuestos[_currentIndex]);
                ActualizarBotonesNavegacion();
            }
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                MostrarPresupuesto(_presupuestos[_currentIndex]);
                ActualizarBotonesNavegacion();
            }
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (_currentIndex < _presupuestos.Count - 1)
            {
                _currentIndex++;
                MostrarPresupuesto(_presupuestos[_currentIndex]);
                ActualizarBotonesNavegacion();
            }
        }

        private void btnUltimo_Click(object sender, EventArgs e)
        {
            if (_presupuestos.Count > 0)
            {
                _currentIndex = _presupuestos.Count - 1;
                MostrarPresupuesto(_presupuestos[_currentIndex]);
                ActualizarBotonesNavegacion();
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            // TODO: Implementar búsqueda de presupuestos
            MessageBox.Show("Funcionalidad de búsqueda pendiente de implementar", "Información",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            if (_currentIndex < 0 || _currentIndex >= _presupuestos.Count)
            {
                MessageBox.Show("No hay presupuesto seleccionado", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // TODO: Implementar impresión del presupuesto
            MessageBox.Show("Funcionalidad de impresión pendiente de implementar", "Información",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                CargarDatos();
                MessageBox.Show("Datos actualizados", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void LimpiarFormulario()
        {
            txtCotizacion.Text = "";
            txtFecha.Value = DateTime.Now;
            txtCliente.Text = "";
            txtVendedor.Text = "";
            txtSolicitante.Text = "";
            txtFormaPago.Text = "";
            txtPlazoEntrega.Text = "";
            dtEntrega.Value = DateTime.Now;
            cmbEstado.SelectedIndex = 0;
            dgArticulos.Rows.Clear();
            txtSUB.Text = "0.00";
            txtIva.Text = "0.00";
            txtIIBB.Text = "0.00";
            txtTotal.Text = "0.00";
            txtTotalKG.Text = "0.00";
        }

        private string GenerarNumeroPresupuesto()
        {
            // TODO: Implementar generación de número desde base de datos
            int maxNum = 0;
            foreach (var p in _presupuestos)
            {
                if (int.TryParse(p.Numero, out int num))
                {
                    if (num > maxNum)
                        maxNum = num;
                }
            }
            return (maxNum + 1).ToString("0000");
        }

        // Clase auxiliar para manejar los datos del presupuesto
        private class PresupuestoItem
        {
            public string Numero { get; set; }
            public DateTime Fecha { get; set; }
            public string Cliente { get; set; }
            public string Vendedor { get; set; }
            public string Solicitante { get; set; }
            public string FormaPago { get; set; }
            public string PlazoEntrega { get; set; }
            public DateTime? FechaVencimiento { get; set; }
            public string Estado { get; set; }
        }
    }
}
