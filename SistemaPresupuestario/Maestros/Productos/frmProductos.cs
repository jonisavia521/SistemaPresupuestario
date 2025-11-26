using BLL.Contracts;
using BLL.DTOs;
using SistemaPresupuestario.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros.Productos
{
    public partial class frmProductos : FormBase
    {
        private readonly IProductoService _productoService;
        private List<ProductoDTO> _listaCompletaProductos;

        public frmProductos(IProductoService productoService)
        {
            InitializeComponent();
            dgvProductos.AutoGenerateColumns = false;
            _productoService = productoService;
            _listaCompletaProductos = new List<ProductoDTO>();
            
            base.InitializeTranslation();
        }
        
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            if (dgvProductos.Columns.Count > 0)
            {
                ActualizarColumnasGrilla();
            }
        }
        
        /// <summary>
        /// Actualiza los encabezados de columnas de la grilla
        /// </summary>
        private void ActualizarColumnasGrilla()
        {
            foreach (DataGridViewColumn column in dgvProductos.Columns)
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

        private void frmProductos_Load(object sender, EventArgs e)
        {
            CargarProductos();
        }

        private void CargarProductos()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                var productos = _productoService.GetAll();
                _listaCompletaProductos = productos.ToList();
                AplicarFiltros();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{I18n.T("Error al cargar productos")}: {ex.Message}", I18n.T("Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            // Verificar si el formulario 'frmProductoAlta' ya está abierto
            var formAbierto = Application.OpenForms.OfType<frmProductoAlta>().FirstOrDefault();

            if (formAbierto == null)
            {
                // Crear una nueva instancia si el formulario no está abierto
                var frmAlta = new frmProductoAlta(_productoService)
                {
                    MdiParent = this.MdiParent
                };
                
                // Suscribirse al evento de guardado exitoso
                frmAlta.ProductoGuardado += (s, ev) => CargarProductos();
                
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
            if (dgvProductos.CurrentRow == null)
            {
                MessageBox.Show(I18n.T("Debe seleccionar un producto para editar"), I18n.T("Validación"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var productoDTO = (ProductoDTO)dgvProductos.CurrentRow.DataBoundItem;

            // Verificar si el formulario ya está abierto
            var formAbierto = Application.OpenForms.OfType<frmProductoAlta>()
                .FirstOrDefault(f => f.ProductoId == productoDTO.Id);

            if (formAbierto == null)
            {
                var frmAlta = new frmProductoAlta(_productoService, productoDTO.Id)
                {
                    MdiParent = this.MdiParent
                };
                
                // Suscribirse al evento de guardado exitoso
                frmAlta.ProductoGuardado += (s, ev) => CargarProductos();
                
                frmAlta.Show();
            }
            else
            {
                formAbierto.BringToFront();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.CurrentRow == null)
            {
                MessageBox.Show(I18n.T("Debe seleccionar un producto para inhabilitar"), I18n.T("Validación"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var productoDTO = (ProductoDTO)dgvProductos.CurrentRow.DataBoundItem;

            if (productoDTO.Inhabilitado)
            {
                MessageBox.Show(I18n.T("El producto ya está inhabilitado"), I18n.T("Información"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                $"{I18n.T("¿Está seguro que desea inhabilitar el producto")} '{productoDTO.Codigo}'?",
                I18n.T("Confirmar Inhabilitación"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    _productoService.Delete(productoDTO.Id);

                    MessageBox.Show(I18n.T("Producto inhabilitado exitosamente"), I18n.T("Éxito"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CargarProductos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{I18n.T("Error al inhabilitar producto")}: {ex.Message}", I18n.T("Error"),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
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
            var listaFiltrada = _listaCompletaProductos.AsEnumerable();

            // Filtro por activos
            if (chkSoloActivos.Checked)
            {
                listaFiltrada = listaFiltrada.Where(p => !p.Inhabilitado);
            }

            // Filtro por búsqueda
            if (!string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                var busqueda = txtBuscar.Text.ToUpper();
                listaFiltrada = listaFiltrada.Where(p =>
                    (p.Codigo != null && p.Codigo.ToUpper().Contains(busqueda)) ||
                    (p.Descripcion != null && p.Descripcion.ToUpper().Contains(busqueda))
                );
            }

            // Ahora sí, asignamos el resultado filtrado al DataSource
            dgvProductos.DataSource = listaFiltrada.ToList();
            dgvProductos.Refresh();
        }
    }
}
