using BLL.Contracts;
using BLL.DTOs;
using SistemaPresupuestario.Maestros.Clientes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros
{
    public partial class frmClientes : Form
    {
        private readonly IClienteService _clienteService;
        private readonly IVendedorService _vendedorService; // AGREGADO
        private List<ClienteDTO> _listaCompletaClientes;

        public frmClientes(IClienteService clienteService, IVendedorService vendedorService) // MODIFICADO
        {
            InitializeComponent();
            dgvClientes.AutoGenerateColumns = false;
            _clienteService = clienteService;
            _vendedorService = vendedorService; // AGREGADO
            _listaCompletaClientes = new List<ClienteDTO>();
        }

        private async void frmClientes_Load(object sender, EventArgs e)
        {
            await CargarClientes();
        }

        private async Task CargarClientes()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                var clientes = await _clienteService.GetAllAsync();
                _listaCompletaClientes = clientes.ToList();
                AplicarFiltros();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar clientes: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            // Verificar si el formulario 'frmClienteAlta' ya está abierto
            var formAbierto = Application.OpenForms.OfType<frmClienteAlta>().FirstOrDefault();

            if (formAbierto == null)
            {
                // Crear una nueva instancia si el formulario no está abierto
                var frmAlta = new frmClienteAlta(_clienteService, _vendedorService) // MODIFICADO
                {
                    MdiParent = this.MdiParent
                };
                
                // Suscribirse al evento de guardado exitoso
                frmAlta.ClienteGuardado += async (s, ev) => await CargarClientes();
                
                frmAlta.Show();
            }
            else
            {
                // Si ya está abierto, traerlo al frente
                formAbierto.BringToFront();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow == null)
            {
                MessageBox.Show("Debe seleccionar un cliente para editar", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var clienteDTO = (ClienteDTO)dgvClientes.CurrentRow.DataBoundItem;

            // Verificar si el formulario ya está abierto
            var formAbierto = Application.OpenForms.OfType<frmClienteAlta>()
                .FirstOrDefault(f => f.ClienteId == clienteDTO.Id);

            if (formAbierto == null)
            {
                var frmAlta = new frmClienteAlta(_clienteService, _vendedorService, clienteDTO.Id) // MODIFICADO
                {
                    MdiParent = this.MdiParent
                };
                
                // Suscribirse al evento de guardado exitoso
                frmAlta.ClienteGuardado += async (s, ev) => await CargarClientes();
                
                frmAlta.Show();
            }
            else
            {
                formAbierto.BringToFront();
            }
        }

        private async void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow == null)
            {
                MessageBox.Show("Debe seleccionar un cliente para eliminar", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var clienteDTO = (ClienteDTO)dgvClientes.CurrentRow.DataBoundItem;

            var result = MessageBox.Show(
                $"¿Está seguro que desea desactivar el cliente '{clienteDTO.RazonSocial}'?",
                "Confirmar Eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    await _clienteService.DeleteAsync(clienteDTO.Id);

                    MessageBox.Show("Cliente desactivado exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    await CargarClientes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al desactivar cliente: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private async void btnReactivar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow == null)
            {
                MessageBox.Show("Debe seleccionar un cliente para reactivar", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var clienteDTO = (ClienteDTO)dgvClientes.CurrentRow.DataBoundItem;

            if (clienteDTO.Activo)
            {
                MessageBox.Show("El cliente ya está activo", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;

                await _clienteService.ReactivarAsync(clienteDTO.Id);

                MessageBox.Show("Cliente reactivado exitosamente", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                await CargarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al reactivar cliente: {ex.Message}", "Error",
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

        private void chkSoloActivos_CheckedChanged(object sender, EventArgs e)
        {
            AplicarFiltros();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            var listaFiltrada = _listaCompletaClientes.AsEnumerable();

            // Filtro por activos
            if (chkSoloActivos.Checked)
            {
                listaFiltrada = listaFiltrada.Where(c => c.Activo);
            }

            // Filtro por búsqueda
            if (!string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                var busqueda = txtBuscar.Text.ToUpper();
                listaFiltrada = listaFiltrada.Where(c =>
                    (c.CodigoCliente != null && c.CodigoCliente.ToUpper().Contains(busqueda)) ||
                    (c.RazonSocial != null && c.RazonSocial.ToUpper().Contains(busqueda)) ||
                    (c.NumeroDocumento != null && c.NumeroDocumento.Contains(busqueda))
                );
            }

            // Ahora sí, asignamos el resultado filtrado al DataSource
            dgvClientes.DataSource = listaFiltrada.ToList();
            dgvClientes.Refresh();
        }
    }
}
