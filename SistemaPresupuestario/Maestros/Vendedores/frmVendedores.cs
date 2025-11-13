using BLL.Contracts;
using BLL.DTOs;
using SistemaPresupuestario.Maestros.Vendedores;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros
{
    public partial class frmVendedores : Form
    {
        private readonly IVendedorService _vendedorService;
        private System.Collections.Generic.List<VendedorDTO> _listaCompleta;

        public frmVendedores(IVendedorService vendedorService)
        {
            InitializeComponent();
            _vendedorService = vendedorService;
            
            dgvVendedores.AutoGenerateColumns = false;
        }

        private  void frmVendedores_Load(object sender, EventArgs e)
        {
            CargarVendedores();
        }

        private  void CargarVendedores()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                var vendedores = _vendedorService.GetAll();
                _listaCompleta = vendedores.ToList(); 
                
                dgvVendedores.DataSource = _listaCompleta;
                dgvVendedores.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar vendedores: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            var formAbierto = Application.OpenForms.OfType<frmVendedorAlta>().FirstOrDefault();

            if (formAbierto == null)
            {
                var frmAlta = new frmVendedorAlta(_vendedorService)
                {
                    MdiParent = this.MdiParent
                };
                
                frmAlta.VendedorGuardado +=  (s, ev) => CargarVendedores();
                
                frmAlta.Show();
            }
            else
            {
                formAbierto.BringToFront();
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvVendedores.CurrentRow == null)
            {
                MessageBox.Show("Debe seleccionar un vendedor para editar", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var vendedorDTO = (VendedorDTO)dgvVendedores.CurrentRow.DataBoundItem;

            var formAbierto = Application.OpenForms.OfType<frmVendedorAlta>()
                .FirstOrDefault(f => f.VendedorId == vendedorDTO.Id);

            if (formAbierto == null)
            {
                var frmAlta = new frmVendedorAlta(_vendedorService, vendedorDTO.Id)
                {
                    MdiParent = this.MdiParent
                };
                
                frmAlta.VendedorGuardado +=  (s, ev) => CargarVendedores();
                
                frmAlta.Show();
            }
            else
            {
                formAbierto.BringToFront();
            }
        }

        private  void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvVendedores.CurrentRow == null)
            {
                MessageBox.Show("Debe seleccionar un vendedor para desactivar", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var vendedorDTO = (VendedorDTO)dgvVendedores.CurrentRow.DataBoundItem;

            var result = MessageBox.Show(
                $"¿Está seguro que desea desactivar el vendedor '{vendedorDTO.Nombre}'?",
                "Confirmar Eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;

                     _vendedorService.Delete(vendedorDTO.Id);

                    MessageBox.Show("Vendedor desactivado exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CargarVendedores();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al desactivar vendedor: {ex.Message}", "Error",
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
            if (dgvVendedores.CurrentRow == null)
            {
                MessageBox.Show("Debe seleccionar un vendedor para reactivar", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var vendedorDTO = (VendedorDTO)dgvVendedores.CurrentRow.DataBoundItem;

            if (vendedorDTO.Activo)
            {
                MessageBox.Show("El vendedor ya está activo", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;

                _vendedorService.Reactivar(vendedorDTO.Id);

                MessageBox.Show("Vendedor reactivado exitosamente", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarVendedores();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al reactivar vendedor: {ex.Message}", "Error",
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
            if (_listaCompleta == null)
                return;

            var listaFiltrada = _listaCompleta.AsEnumerable();

            // Filtro por activos
            if (chkSoloActivos.Checked)
            {
                listaFiltrada = listaFiltrada.Where(v => v.Activo);
            }

            // Filtro por búsqueda
            if (!string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                var busqueda = txtBuscar.Text.ToUpper();
                listaFiltrada = listaFiltrada.Where(v =>
                    (v.CodigoVendedor != null && v.CodigoVendedor.ToUpper().Contains(busqueda)) ||
                    (v.Nombre != null && v.Nombre.ToUpper().Contains(busqueda)) ||
                    (v.CUIT != null && v.CUIT.Contains(busqueda))
                );
            }

            dgvVendedores.DataSource = listaFiltrada.ToList();
            dgvVendedores.Refresh();
        }
    }
}
