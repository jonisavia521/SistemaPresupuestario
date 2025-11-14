using BLL.Contracts;
using BLL.DTOs;
using SistemaPresupuestario.Maestros.Clientes;
using SistemaPresupuestario.Helpers; // NUEVO
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
        private readonly IVendedorService _vendedorService;
        private readonly IProvinciaService _provinciaService;
        private List<ClienteDTO> _listaCompletaClientes;

        public frmClientes(IClienteService clienteService, IVendedorService vendedorService, IProvinciaService provinciaService)
        {
            InitializeComponent();
            dgvClientes.AutoGenerateColumns = false;
            _clienteService = clienteService;
            _vendedorService = vendedorService;
            _provinciaService = provinciaService;
            _listaCompletaClientes = new List<ClienteDTO>();
            
            // ? TRADUCCIÓN AUTOMÁTICA: Aplicar traducciones a TODOS los controles
            FormTranslator.Translate(this);
            
            // ? TRADUCCIÓN DINÁMICA: Suscribirse al evento de cambio de idioma
            I18n.LanguageChanged += OnLanguageChanged;
            this.FormClosed += (s, e) => I18n.LanguageChanged -= OnLanguageChanged;
        }
        
        /// <summary>
        /// Manejador del evento de cambio de idioma
        /// </summary>
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            FormTranslator.Translate(this);

            if (dgvClientes.Columns.Count > 0)
            {
                ActualizarColumnasGrilla();
            }
        }
        
        /// <summary>
        /// Actualiza los encabezados de columnas de la grilla
        /// </summary>
        private void ActualizarColumnasGrilla()
        {
            foreach (DataGridViewColumn column in dgvClientes.Columns)
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

        private  void frmClientes_Load(object sender, EventArgs e)
        {
            CargarClientes();
        }

        private  void CargarClientes()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                var clientes = _clienteService.GetAll();
                _listaCompletaClientes = clientes.ToList();
                AplicarFiltros();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{I18n.T("Error al cargar clientes")}: {ex.Message}", I18n.T("Error"),
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
                var frmAlta = new frmClienteAlta(_clienteService, _vendedorService, _provinciaService) // MODIFICADO
                {
                    MdiParent = this.MdiParent
                };
                
                // Suscribirse al evento de guardado exitoso
                frmAlta.ClienteGuardado +=  (s, ev) => CargarClientes();
                
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
                MessageBox.Show(I18n.T("Debe seleccionar un cliente para editar"), I18n.T("Validación"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var clienteDTO = (ClienteDTO)dgvClientes.CurrentRow.DataBoundItem;

            // Verificar si el formulario ya está abierto
            var formAbierto = Application.OpenForms.OfType<frmClienteAlta>()
                .FirstOrDefault(f => f.ClienteId == clienteDTO.Id);

            if (formAbierto == null)
            {
                var frmAlta = new frmClienteAlta(_clienteService, _vendedorService, _provinciaService, clienteDTO.Id) // MODIFICADO
                {
                    MdiParent = this.MdiParent
                };
                
                // Suscribirse al evento de guardado exitoso
                frmAlta.ClienteGuardado +=  (s, ev) => CargarClientes();
                
                frmAlta.Show();
            }
            else
            {
                formAbierto.BringToFront();
            }
        }

        private  void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow == null)
            {
                MessageBox.Show(I18n.T("Debe seleccionar un cliente para eliminar"), I18n.T("Validación"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var clienteDTO = (ClienteDTO)dgvClientes.CurrentRow.DataBoundItem;

            var result = MessageBox.Show(
                $"{I18n.T("¿Está seguro que desea desactivar el cliente")} '{clienteDTO.RazonSocial}'?",
                I18n.T("Confirmar Eliminación"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    _clienteService.Delete(clienteDTO.Id);

                    MessageBox.Show(I18n.T("Cliente desactivado exitosamente"), I18n.T("Éxito"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CargarClientes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{I18n.T("Error al desactivar cliente")}: {ex.Message}", I18n.T("Error"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private  void btnReactivar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.CurrentRow == null)
            {
                MessageBox.Show(I18n.T("Debe seleccionar un cliente para reactivar"), I18n.T("Validación"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var clienteDTO = (ClienteDTO)dgvClientes.CurrentRow.DataBoundItem;

            if (clienteDTO.Activo)
            {
                MessageBox.Show(I18n.T("El cliente ya está activo"), I18n.T("Información"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;

                _clienteService.Reactivar(clienteDTO.Id);

                MessageBox.Show(I18n.T("Cliente reactivado exitosamente"), I18n.T("Éxito"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarClientes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{I18n.T("Error al reactivar cliente")}: {ex.Message}", I18n.T("Error"),
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
