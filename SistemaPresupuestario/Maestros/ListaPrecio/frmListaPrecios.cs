using BLL.Contracts;
using BLL.DTOs;
using SistemaPresupuestario.Helpers; // NUEVO
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
            
            // ? TRADUCCIÓN AUTOMÁTICA
            FormTranslator.Translate(this);
            
            // ? TRADUCCIÓN DINÁMICA
            I18n.LanguageChanged += OnLanguageChanged;
            this.FormClosed += (s, e) => I18n.LanguageChanged -= OnLanguageChanged;
        }
        
        /// <summary>
        /// Manejador del evento de cambio de idioma
        /// </summary>
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            FormTranslator.Translate(this);

            if (dgvListaPrecios.Columns.Count > 0)
            {
                ActualizarColumnasGrilla();
            }
        }
        
        /// <summary>
        /// Actualiza los encabezados de columnas de la grilla
        /// </summary>
        private void ActualizarColumnasGrilla()
        {
            foreach (DataGridViewColumn column in dgvListaPrecios.Columns)
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

        private  void frmListaPrecios_Load(object sender, EventArgs e)
        {
            ConfigurarGrilla();
            CargarListasPrecios();
        }

        private void ConfigurarGrilla()
        {
            dgvListaPrecios.Columns.Clear();

            // Columna Id (oculta)
            dgvListaPrecios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = I18n.T("Id"),
                DataPropertyName = "Id",
                Visible = false,
                Tag = "Id"
            });

            // Columna Código
            dgvListaPrecios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Codigo",
                HeaderText = I18n.T("Código"),
                DataPropertyName = "Codigo",
                Width = 100,
                Tag = "Código"
            });

            // Columna Nombre
            dgvListaPrecios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Nombre",
                HeaderText = I18n.T("Nombre"),
                DataPropertyName = "Nombre",
                Width = 350,
                Tag = "Nombre"
            });

            // Columna Fecha Alta
            dgvListaPrecios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "FechaAlta",
                HeaderText = I18n.T("Fecha Alta"),
                DataPropertyName = "FechaAlta",
                Width = 120,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" },
                Tag = "Fecha Alta"
            });

            // Columna Estado
            dgvListaPrecios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "EstadoTexto",
                HeaderText = I18n.T("Estado"),
                DataPropertyName = "EstadoTexto",
                Width = 100,
                Tag = "Estado"
            });
        }

        private  void CargarListasPrecios()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                var listas = _listaPrecioService.GetAll();
                _listaCompletaPrecios = listas.ToList();
                AplicarFiltros();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{I18n.T("Error al cargar listas de precios")}: {ex.Message}", I18n.T("Error"),
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
                    CargarListasPrecios();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{I18n.T("Error al abrir formulario de alta")}: {ex.Message}", I18n.T("Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvListaPrecios.SelectedRows.Count == 0)
            {
                MessageBox.Show(I18n.T("Seleccione una lista de precios para editar"), I18n.T("Advertencia"),
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
                    CargarListasPrecios();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{I18n.T("Error al abrir formulario de edición")}: {ex.Message}", I18n.T("Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private  void btnDesactivar_Click(object sender, EventArgs e)
        {
            if (dgvListaPrecios.SelectedRows.Count == 0)
            {
                MessageBox.Show(I18n.T("Seleccione una lista de precios para desactivar"), I18n.T("Advertencia"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var listaNombre = dgvListaPrecios.SelectedRows[0].Cells["Nombre"].Value.ToString();
            var confirmResult = MessageBox.Show(
                $"{I18n.T("¿Está seguro que desea desactivar la lista")} '{listaNombre}'?",
                I18n.T("Confirmar Desactivación"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    var listaId = (Guid)dgvListaPrecios.SelectedRows[0].Cells["Id"].Value;

                    _listaPrecioService.Delete(listaId);

                    MessageBox.Show(I18n.T("Lista de precios desactivada exitosamente"), I18n.T("Éxito"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarListasPrecios();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{I18n.T("Error al desactivar lista de precios")}: {ex.Message}", I18n.T("Error"),
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
            if (dgvListaPrecios.SelectedRows.Count == 0)
            {
                MessageBox.Show(I18n.T("Seleccione una lista de precios para reactivar"), I18n.T("Advertencia"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var listaNombre = dgvListaPrecios.SelectedRows[0].Cells["Nombre"].Value.ToString();
            var confirmResult = MessageBox.Show(
                $"{I18n.T("¿Está seguro que desea reactivar la lista")} '{listaNombre}'?",
                I18n.T("Confirmar Reactivación"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmResult == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;
                    var listaId = (Guid)dgvListaPrecios.SelectedRows[0].Cells["Id"].Value;

                    _listaPrecioService.Reactivar(listaId);

                    MessageBox.Show(I18n.T("Lista de precios reactivada exitosamente"), I18n.T("Éxito"),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarListasPrecios();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{I18n.T("Error al reactivar lista de precios")}: {ex.Message}", I18n.T("Error"),
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
