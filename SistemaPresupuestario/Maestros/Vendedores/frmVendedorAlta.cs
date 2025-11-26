using BLL.Contracts;
using BLL.DTOs;
using SistemaPresupuestario.Helpers; // NUEVO
using System;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros.Vendedores
{
    public partial class frmVendedorAlta : FormBase
    {
        private readonly IVendedorService _vendedorService;
        private Guid? _vendedorId;

        // Evento para notificar que se guardó exitosamente
        public event EventHandler VendedorGuardado;

        // Propiedad pública para identificar el vendedor en edición
        public Guid? VendedorId => _vendedorId;

        // Constructor para modo NUEVO
        public frmVendedorAlta(IVendedorService vendedorService)
        {
            InitializeComponent();
            _vendedorService = vendedorService;
            _vendedorId = null;
            
            base.InitializeTranslation();
        }

        // Constructor para modo EDICIÓN
        public frmVendedorAlta(IVendedorService vendedorService, Guid vendedorId)
        {
            InitializeComponent();
            _vendedorService = vendedorService;
            _vendedorId = vendedorId;
            
            base.InitializeTranslation();
        }

        private void frmVendedorAlta_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Si es modo edición, cargar datos
                if (_vendedorId.HasValue)
                {
                    CargarDatosVendedor();
                    this.Text = "Editar Vendedor";
                    txtCodigoVendedor.Enabled = false; // El código no se modifica
                }
                else
                {
                    this.Text = "Nuevo Vendedor";
                    numPorcentajeComision.Value = 0; // Por defecto 0%
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void CargarDatosVendedor()
        {
            var vendedor = _vendedorService.GetById(_vendedorId.Value);

            if (vendedor == null)
            {
                MessageBox.Show("No se encontró el vendedor", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            txtCodigoVendedor.Text = vendedor.CodigoVendedor;
            txtNombre.Text = vendedor.Nombre;
            txtCUIT.Text = vendedor.CUIT;
            numPorcentajeComision.Value = vendedor.PorcentajeComision;
            txtEmail.Text = vendedor.Email;
            txtTelefono.Text = vendedor.Telefono;
            txtDireccion.Text = vendedor.Direccion;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario())
                return;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                var vendedorDTO = new VendedorDTO
                {
                    Id = _vendedorId ?? Guid.Empty,
                    CodigoVendedor = txtCodigoVendedor.Text.Trim(),
                    Nombre = txtNombre.Text.Trim(),
                    CUIT = txtCUIT.Text.Trim().Replace("-", ""),
                    PorcentajeComision = numPorcentajeComision.Value,
                    Email = txtEmail.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim(),
                    Direccion = txtDireccion.Text.Trim()
                };

                bool resultado;

                if (_vendedorId.HasValue)
                {
                    // Modo EDICIÓN
                    resultado = _vendedorService.Update(vendedorDTO);
                    MessageBox.Show("Vendedor actualizado exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Modo NUEVO
                    resultado = _vendedorService.Add(vendedorDTO);
                    MessageBox.Show("Vendedor creado exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (resultado)
                {
                    // Notificar que se guardó exitosamente
                    VendedorGuardado?.Invoke(this, EventArgs.Empty);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar vendedor: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private bool ValidarFormulario()
        {
            // Limpiar errores previos
            errorProvider1.Clear();

            bool esValido = true;

            if (string.IsNullOrWhiteSpace(txtCodigoVendedor.Text))
            {
                errorProvider1.SetError(txtCodigoVendedor, "El código de vendedor es obligatorio");
                esValido = false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(txtCodigoVendedor.Text, @"^\d{2}$"))
            {
                errorProvider1.SetError(txtCodigoVendedor, "El código debe tener exactamente 2 dígitos");
                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                errorProvider1.SetError(txtNombre, "El nombre es obligatorio");
                esValido = false;
            }

            var cuitLimpio = txtCUIT.Text.Replace("-", "").Trim();
            if (string.IsNullOrWhiteSpace(cuitLimpio))
            {
                errorProvider1.SetError(txtCUIT, "El CUIT es obligatorio");
                esValido = false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(cuitLimpio, @"^\d{11}$"))
            {
                errorProvider1.SetError(txtCUIT, "El CUIT debe tener 11 dígitos");
                esValido = false;
            }

            if (numPorcentajeComision.Value < 0 || numPorcentajeComision.Value > 100)
            {
                errorProvider1.SetError(numPorcentajeComision, "La comisión debe estar entre 0 y 100");
                esValido = false;
            }

            if (!esValido)
            {
                MessageBox.Show("Por favor, corrija los errores señalados", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return esValido;
        }

        private void txtCUIT_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir números, guión y teclas de control
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '-')
            {
                e.Handled = true;
            }
        }

        private void txtCodigoVendedor_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Solo permitir números y teclas de control
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
