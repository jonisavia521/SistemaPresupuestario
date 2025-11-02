using BLL.Contracts;
using BLL.DTOs;
using SistemaPresupuestario.Maestros.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros.Clientes
{
    public partial class frmClienteAlta : Form
    {
        private readonly IClienteService _clienteService;
        private readonly IVendedorService _vendedorService; // AGREGADO
        private Guid? _clienteId;
        private Guid? _vendedorSeleccionadoId; // AGREGADO para guardar el ID del vendedor

        // Evento para notificar que se guardó exitosamente
        public event EventHandler ClienteGuardado;

        // Propiedad pública para identificar el cliente en edición
        public Guid? ClienteId => _clienteId;

        // Constructor para modo NUEVO
        public frmClienteAlta(IClienteService clienteService, IVendedorService vendedorService) // MODIFICADO
        {
            InitializeComponent();
            _clienteService = clienteService;
            _vendedorService = vendedorService; // AGREGADO
            _clienteId = null;
        }

        // Constructor para modo EDICIÓN
        public frmClienteAlta(IClienteService clienteService, IVendedorService vendedorService, Guid clienteId) // MODIFICADO
        {
            InitializeComponent();
            _clienteService = clienteService;
            _vendedorService = vendedorService; // AGREGADO
            _clienteId = clienteId;
        }

        private async void frmClienteAlta_Load(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // Cargar ComboBoxes
                CargarTiposDocumento();
                CargarTiposIva();
                CargarCondicionesPago();

                // Si es modo edición, cargar datos
                if (_clienteId.HasValue)
                {
                    await CargarDatosCliente();
                    this.Text = "Editar Cliente";
                    txtCodigoCliente.Enabled = false; // El código no se modifica
                }
                else
                {
                    this.Text = "Nuevo Cliente";
                    GenerarCodigoCliente();
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void CargarTiposDocumento()
        {
            cboTipoDocumento.Items.Clear();
            cboTipoDocumento.Items.AddRange(new object[] { "DNI", "CUIT", "CUIL" });
            cboTipoDocumento.SelectedIndex = 1; // CUIT por defecto
        }

        private void CargarTiposIva()
        {
            cboTipoIva.Items.Clear();
            cboTipoIva.Items.AddRange(new object[]
            {
                "RESPONSABLE INSCRIPTO",
                "MONOTRIBUTISTA",
                "EXENTO",
                "CONSUMIDOR FINAL",
                "NO RESPONSABLE"
            });
            cboTipoIva.SelectedIndex = 0; // Responsable Inscripto por defecto
        }

        private void CargarCondicionesPago()
        {
            cboCondicionPago.Items.Clear();
            
            // Diccionario con códigos y descripciones
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

        private void GenerarCodigoCliente()
        {
            // Generar código automático basado en timestamp
            txtCodigoCliente.Text = $"CLI-{DateTime.Now:yyyyMMddHHmmss}";
        }

        private async System.Threading.Tasks.Task CargarDatosCliente()
        {
            var cliente = await _clienteService.GetByIdAsync(_clienteId.Value);

            if (cliente == null)
            {
                MessageBox.Show("No se encontró el cliente", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            txtCodigoCliente.Text = cliente.CodigoCliente;
            txtRazonSocial.Text = cliente.RazonSocial;
            cboTipoDocumento.SelectedItem = cliente.TipoDocumento;
            txtNumeroDocumento.Text = cliente.NumeroDocumento;
            
            // MODIFICADO: Cargar vendedor si existe
            _vendedorSeleccionadoId = cliente.IdVendedor;
            if (_vendedorSeleccionadoId.HasValue)
            {
                await CargarVendedorEnTextBox(_vendedorSeleccionadoId.Value);
            }
            
            // Seleccionar el tipo de IVA
            var indexIva = cboTipoIva.Items.IndexOf(cliente.TipoIva);
            if (indexIva >= 0)
                cboTipoIva.SelectedIndex = indexIva;

            // Seleccionar la condición de pago
            for (int i = 0; i < cboCondicionPago.Items.Count; i++)
            {
                dynamic item = cboCondicionPago.Items[i];
                if (item.Value == cliente.CondicionPago)
                {
                    cboCondicionPago.SelectedIndex = i;
                    break;
                }
            }

            txtEmail.Text = cliente.Email;
            txtTelefono.Text = cliente.Telefono;
            txtDireccion.Text = cliente.Direccion;
            txtLocalidad.Text = cliente.Localidad;
        }

        // NUEVO: Cargar información del vendedor en el TextBox
        private async System.Threading.Tasks.Task CargarVendedorEnTextBox(Guid idVendedor)
        {
            try
            {
                var vendedor = await _vendedorService.GetByIdAsync(idVendedor);
                if (vendedor != null)
                {
                    txtCodigoVendedor.Text = $"{vendedor.CodigoVendedor} - {vendedor.Nombre}";
                }
            }
            catch
            {
                txtCodigoVendedor.Text = "";
            }
        }

        // NUEVO: Evento KeyDown para abrir selector de vendedores
        private void txtCodigoVendedor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.F3)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                AbrirSelectorVendedor();
            }
        }

        // NUEVO: Método para abrir el selector de vendedores
        private void AbrirSelectorVendedor()
        {
            // Configurar el selector dinámico
            var config = new SelectorConfig<VendedorDTO>
            {
                Titulo = "Seleccionar Vendedor",
                Datos = System.Threading.Tasks.Task.Run(() => _vendedorService.GetActivosAsync()).Result,
                PlaceholderBusqueda = "Buscar por código, nombre o CUIT...",
                PermitirSeleccionMultiple = false,
                
                // Definir columnas a mostrar
                Columnas = new List<ColumnaConfig>
                {
                    new ColumnaConfig { NombrePropiedad = "Id", TituloColumna = "Id", Visible = false },
                    new ColumnaConfig { NombrePropiedad = "CodigoVendedor", TituloColumna = "Código", Ancho = 80 },
                    new ColumnaConfig { NombrePropiedad = "Nombre", TituloColumna = "Nombre", Ancho = 250 },
                    new ColumnaConfig { NombrePropiedad = "CUITFormateado", TituloColumna = "CUIT", Ancho = 150 },
                    new ColumnaConfig { NombrePropiedad = "Email", TituloColumna = "Email", Ancho = 200 }
                },
                
                // Función de filtro personalizada
                FuncionFiltro = (busqueda, vendedor) =>
                {
                    var b = busqueda.ToUpper();
                    return (vendedor.CodigoVendedor != null && vendedor.CodigoVendedor.Contains(b)) ||
                           (vendedor.Nombre != null && vendedor.Nombre.ToUpper().Contains(b)) ||
                           (vendedor.CUIT != null && vendedor.CUIT.Contains(b));
                }
            };

            // Mostrar el selector
            var selector = frmSelector.Mostrar(config);
            if (selector.ShowDialog() == DialogResult.OK && selector.ElementoSeleccionado != null)
            {
                var vendedor = (VendedorDTO)selector.ElementoSeleccionado;
                _vendedorSeleccionadoId = vendedor.Id;
                txtCodigoVendedor.Text = $"{vendedor.CodigoVendedor} - {vendedor.Nombre}";
            }
            selector.Dispose();
        }

        private async void btnAceptar_Click(object sender, EventArgs e)
        {
            var clienteDTO = new ClienteDTO
            {
                Id = _clienteId ?? Guid.Empty,
                CodigoCliente = txtCodigoCliente.Text.Trim(),
                RazonSocial = txtRazonSocial.Text.Trim(),
                TipoDocumento = cboTipoDocumento.SelectedItem.ToString(),
                NumeroDocumento = txtNumeroDocumento.Text.Trim(),
                IdVendedor = _vendedorSeleccionadoId, // MODIFICADO: Usar IdVendedor en lugar de CodigoVendedor
                TipoIva = cboTipoIva.SelectedItem.ToString(),
                CondicionPago = ((dynamic)cboCondicionPago.SelectedItem).Value,
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                Direccion = txtDireccion.Text.Trim(),
                Localidad = txtLocalidad.Text.Trim()
            };

            if (!ValidarConDataAnnotations(clienteDTO))
                return;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                bool resultado;

                if (_clienteId.HasValue)
                {
                    // Modo EDICIÓN
                    resultado = await _clienteService.UpdateAsync(clienteDTO);
                    MessageBox.Show("Cliente actualizado exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Modo NUEVO
                    resultado = await _clienteService.AddAsync(clienteDTO);
                    MessageBox.Show("Cliente creado exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (resultado)
                {
                    // Notificar que se guardó exitosamente
                    ClienteGuardado?.Invoke(this, EventArgs.Empty);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar cliente: {ex.Message}", "Error",
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

        private bool ValidarConDataAnnotations(ClienteDTO dto)
        {
            // 1. Limpia todos los errores anteriores
            errorProvider1.Clear();

            var validationContext = new ValidationContext(dto, null, null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

            if (isValid)
            {
                return true; // No hay errores
            }

            // 2. Si no es válido, itera TODOS los resultados
            foreach (var validationResult in validationResults)
            {
                // Obtiene el nombre del campo (ej. "CodigoCliente")
                string memberName = validationResult.MemberNames.FirstOrDefault();

                if (memberName != null)
                {
                    // Usa el mapeador para encontrar el control (ej. txtCodigoCliente)
                    Control control = GetControlByPropertyName(memberName);
                    if (control != null)
                    {
                        // 3. Asigna el mensaje de error a ese control específico
                        errorProvider1.SetError(control, validationResult.ErrorMessage);
                    }
                }
            }

            // (Opcional) Enfoca el primer control que tuvo un error
            var primerError = validationResults.First().MemberNames.FirstOrDefault();
            if (primerError != null)
            {
                GetControlByPropertyName(primerError)?.Focus();
            }

            return false; // El formulario no es válido
        }

        private Control GetControlByPropertyName(string nombrePropiedad)
        {
            // Mapea el nombre de la propiedad del DTO al control de la UI
            switch (nombrePropiedad)
            {
                case nameof(ClienteDTO.CodigoCliente):
                    return txtCodigoCliente;
                case nameof(ClienteDTO.RazonSocial):
                    return txtRazonSocial;
                case nameof(ClienteDTO.TipoDocumento):
                    return cboTipoDocumento;
                case nameof(ClienteDTO.NumeroDocumento):
                    return txtNumeroDocumento;
                case nameof(ClienteDTO.TipoIva):
                    return cboTipoIva;
                case nameof(ClienteDTO.CondicionPago):
                    return cboCondicionPago;
                case nameof(ClienteDTO.Email):
                    return txtEmail;
                case nameof(ClienteDTO.Telefono):
                    return txtTelefono;
                case nameof(ClienteDTO.Direccion):
                    return txtDireccion;
                default:
                    return null; // No se encontró un control
            }
        }

        private void txtNumeroDocumento_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Solo permitir números y teclas de control
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
