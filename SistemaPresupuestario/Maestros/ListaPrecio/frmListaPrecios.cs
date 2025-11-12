using BLL.Contracts;
using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros.ListaPrecio
{
    public partial class frmListaPrecios : Form
    {
        private readonly IListaPrecioService _listaPrecioService;
        private readonly IServiceProvider _serviceProvider;
        private List<ListaPrecioDTO> _listaCompletaPrecios;

        public frmListaPrecios(IListaPrecioService listaPrecioService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            dgvListaPrecios.AutoGenerateColumns = false;
            _listaPrecioService = listaPrecioService ?? throw new ArgumentNullException(nameof(listaPrecioService));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _listaCompletaPrecios = new List<ListaPrecioDTO>();
        }

        private async void frmListaPrecios_Load(object sender, EventArgs e)
        {
            ConfigurarGrilla();
            await CargarListasPrecios();
        }

        private void ConfigurarGrilla()
        {
            dgvListaPrecios.Columns.Clear();

            // Columna Id (oculta)
            dgvListaPrecios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                DataPropertyName = "Id",
                Visible = false
            });

            // Columna Código
            dgvListaPrecios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Codigo",
                HeaderText = "Código",
                DataPropertyName = "Codigo",
                Width = 100
            });

            // Columna Nombre
            dgvListaPrecios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nombre",
                HeaderText = "Nombre",
                DataPropertyName = "Nombre",
                Width = 350
            });

            // Columna Fecha Alta
            dgvListaPrecios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FechaAlta",
                HeaderText = "Fecha Alta",
                DataPropertyName = "FechaAlta",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });

            // Columna Estado
            dgvListaPrecios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EstadoTexto",
                HeaderText = "Estado",
                DataPropertyName = "EstadoTexto",
                Width = 100
            });
        }

        private async Task CargarListasPrecios()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                var listas = await _listaPrecioService.GetAllAsync();
                _listaCompletaPrecios = listas.ToList();
                AplicarFiltros();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar listas de precios: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void AplicarFiltros()
        {
            var listaFiltrada = _listaCompletaPrecios.AsEnumerable();

            // Filtro por activos
            if (chkSoloActivos.Checked)
            {
                listaFiltrada = listaFiltrada.Where(l => l.Activo);
            }

            // Filtro por búsqueda
            if (!string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                var busqueda = txtBuscar.Text.ToUpper();
                listaFiltrada = listaFiltrada.Where(l =>
                    (l.Codigo != null && l.Codigo.ToUpper().Contains(busqueda)) ||
                    (l.Nombre != null && l.Nombre.ToUpper().Contains(busqueda))
                );
            }

            // Asignar el resultado filtrado al DataSource
            dgvListaPrecios.DataSource = listaFiltrada.ToList();
            dgvListaPrecios.Refresh();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            try
            {
                var frmAlta = _serviceProvider.GetService(typeof(frmListaPrecioAlta)) as frmListaPrecioAlta;
                
                if (frmAlta.ShowDialog() == DialogResult.OK)
                {
                    // Recargar lista si se guardó correctamente
                    _ = CargarListasPrecios();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir formulario de alta: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvListaPrecios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una lista de precios para editar", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var listaId = (Guid)dgvListaPrecios.SelectedRows[0].Cells["Id"].Value;

                var frmEditar = _serviceProvider.GetService(typeof(frmListaPrecioAlta)) as frmListaPrecioAlta;
                frmEditar.CargarListaPrecio(listaId);

                if (frmEditar.ShowDialog() == DialogResult.OK)
                {
                    // Recargar lista si se guardó correctamente
                    _ = CargarListasPrecios();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir formulario de edición: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDesactivar_Click(object sender, EventArgs e)
        {
            if (dgvListaPrecios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una lista de precios para desactivar", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var listaNombre = dgvListaPrecios.SelectedRows[0].Cells["Nombre"].Value.ToString();
            var confirmResult = MessageBox.Show(
                $"¿Está seguro que desea desactivar la lista '{listaNombre}'?",
                "Confirmar Desactivación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    var listaId = (Guid)dgvListaPrecios.SelectedRows[0].Cells["Id"].Value;

                    await _listaPrecioService.DeleteAsync(listaId);

                    MessageBox.Show("Lista de precios desactivada exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await CargarListasPrecios();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al desactivar lista de precios: {ex.Message}", "Error",
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
            if (dgvListaPrecios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una lista de precios para reactivar", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var listaNombre = dgvListaPrecios.SelectedRows[0].Cells["Nombre"].Value.ToString();
            var confirmResult = MessageBox.Show(
                $"¿Está seguro que desea reactivar la lista '{listaNombre}'?",
                "Confirmar Reactivación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    var listaId = (Guid)dgvListaPrecios.SelectedRows[0].Cells["Id"].Value;

                    await _listaPrecioService.ReactivarAsync(listaId);

                    MessageBox.Show("Lista de precios reactivada exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await CargarListasPrecios();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al reactivar lista de precios: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            AplicarFiltros();
        }

        private void chkSoloActivos_CheckedChanged(object sender, EventArgs e)
        {
            AplicarFiltros();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
