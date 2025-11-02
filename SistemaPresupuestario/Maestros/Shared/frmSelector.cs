using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros.Shared
{
    /// <summary>
    /// Formulario dinámico y genérico para seleccionar elementos de cualquier lista
    /// Puede ser reutilizado en toda la aplicación pasándole la configuración necesaria
    /// </summary>
    /// <typeparam name="T">Tipo de objeto a seleccionar</typeparam>
    public partial class frmSelector : Form
    {
        private SelectorConfig<object> _config;
        private List<object> _datosCompletos;
        private List<object> _datosFiltrados;
        
        /// <summary>
        /// Elemento seleccionado por el usuario (modo selección simple)
        /// </summary>
        public object ElementoSeleccionado { get; private set; }

        /// <summary>
        /// Elementos seleccionados por el usuario (modo selección múltiple)
        /// </summary>
        public List<object> ElementosSeleccionados { get; private set; }

        /// <summary>
        /// Constructor privado - usar el método estático Mostrar<T>
        /// </summary>
        private frmSelector()
        {
            InitializeComponent();
            dgvDatos.AutoGenerateColumns = false;
            ElementosSeleccionados = new List<object>();
        }

        /// <summary>
        /// Método estático para mostrar el selector con tipado genérico
        /// </summary>
        /// <typeparam name="T">Tipo de objeto a seleccionar</typeparam>
        /// <param name="config">Configuración del selector</param>
        /// <returns>Formulario configurado</returns>
        public static frmSelector Mostrar<T>(SelectorConfig<T> config)
        {
            var form = new frmSelector();
            
            // Convertir la configuración genérica a object
            form._config = new SelectorConfig<object>
            {
                Titulo = config.Titulo,
                Datos = config.Datos?.Cast<object>(),
                Columnas = config.Columnas,
                PlaceholderBusqueda = config.PlaceholderBusqueda,
                PermitirSeleccionMultiple = config.PermitirSeleccionMultiple
            };

            // Asignar función de filtro si existe (compatible con C# 7.3)
            if (config.FuncionFiltro != null)
            {
                form._config.FuncionFiltro = (busqueda, obj) => config.FuncionFiltro(busqueda, (T)obj);
            }
            
            return form;
        }

        private void frmSelector_Load(object sender, EventArgs e)
        {
            try
            {
                // Aplicar configuración
                AplicarConfiguracion();
                
                // Cargar datos
                CargarDatos();
                
                // Enfocar el cuadro de búsqueda
                txtBuscar.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el selector: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AplicarConfiguracion()
        {
            if (_config == null)
            {
                throw new InvalidOperationException("Debe configurar el selector antes de mostrarlo");
            }

            // Aplicar título
            this.Text = _config.Titulo ?? "Seleccionar";

            // Aplicar placeholder de búsqueda
            if (!string.IsNullOrEmpty(_config.PlaceholderBusqueda))
            {
                lblBuscar.Text = _config.PlaceholderBusqueda;
            }

            // Configurar selección múltiple
            if (_config.PermitirSeleccionMultiple)
            {
                dgvDatos.MultiSelect = true;
                dgvDatos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                btnSeleccionar.Text = "Seleccionar";
            }
            else
            {
                dgvDatos.MultiSelect = false;
                dgvDatos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }

            // Configurar columnas
            ConfigurarColumnas();
        }

        private void ConfigurarColumnas()
        {
            dgvDatos.Columns.Clear();

            if (_config.Columnas == null || _config.Columnas.Count == 0)
            {
                // Si no se especifican columnas, auto-generar
                dgvDatos.AutoGenerateColumns = true;
                return;
            }

            dgvDatos.AutoGenerateColumns = false;

            foreach (var columnaConfig in _config.Columnas)
            {
                var columna = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = columnaConfig.NombrePropiedad,
                    HeaderText = columnaConfig.TituloColumna ?? columnaConfig.NombrePropiedad,
                    Name = $"col{columnaConfig.NombrePropiedad}",
                    Width = columnaConfig.Ancho,
                    Visible = columnaConfig.Visible,
                    ReadOnly = true
                };

                dgvDatos.Columns.Add(columna);
            }
        }

        private void CargarDatos()
        {
            if (_config.Datos == null)
            {
                _datosCompletos = new List<object>();
            }
            else
            {
                _datosCompletos = _config.Datos.ToList();
            }

            _datosFiltrados = new List<object>(_datosCompletos);
            
            ActualizarGrilla();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            var textoBusqueda = txtBuscar.Text.Trim();

            if (string.IsNullOrEmpty(textoBusqueda))
            {
                // Sin filtro, mostrar todos
                _datosFiltrados = new List<object>(_datosCompletos);
            }
            else
            {
                if (_config.FuncionFiltro != null)
                {
                    // Usar función de filtro personalizada
                    _datosFiltrados = _datosCompletos
                        .Where(item => _config.FuncionFiltro(textoBusqueda, item))
                        .ToList();
                }
                else
                {
                    // Filtro por defecto: busca en todas las propiedades string
                    _datosFiltrados = FiltroGenerico(textoBusqueda);
                }
            }

            ActualizarGrilla();
        }

        private List<object> FiltroGenerico(string textoBusqueda)
        {
            var busquedaUpper = textoBusqueda.ToUpper();
            var resultado = new List<object>();

            foreach (var item in _datosCompletos)
            {
                // Obtener todas las propiedades del objeto
                var propiedades = item.GetType().GetProperties();
                
                foreach (var propiedad in propiedades)
                {
                    try
                    {
                        var valor = propiedad.GetValue(item);
                        
                        // Solo buscar en propiedades string
                        if (valor != null && propiedad.PropertyType == typeof(string))
                        {
                            var valorString = valor.ToString().ToUpper();
                            
                            if (valorString.Contains(busquedaUpper))
                            {
                                resultado.Add(item);
                                break; // Ya encontramos coincidencia, no seguir buscando en este objeto
                            }
                        }
                    }
                    catch
                    {
                        // Ignorar propiedades que no se pueden leer
                    }
                }
            }

            return resultado;
        }

        private void ActualizarGrilla()
        {
            dgvDatos.DataSource = null;
            dgvDatos.DataSource = _datosFiltrados;

            // Actualizar contador de resultados
            lblResultados.Text = $"Total registros: {_datosFiltrados.Count}";

            // Seleccionar el primer elemento si hay alguno
            if (dgvDatos.Rows.Count > 0)
            {
                dgvDatos.Rows[0].Selected = true;
                dgvDatos.FirstDisplayedScrollingRowIndex = 0;
            }

            dgvDatos.Refresh();
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            SeleccionarElemento();
        }

        private void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                SeleccionarElemento();
            }
        }

        private void dgvDatos_KeyDown(object sender, KeyEventArgs e)
        {
            // Permitir seleccionar con Enter
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                SeleccionarElemento();
            }
        }

        private void SeleccionarElemento()
        {
            if (_config.PermitirSeleccionMultiple)
            {
                // Modo selección múltiple
                if (dgvDatos.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Debe seleccionar al menos un elemento", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ElementosSeleccionados.Clear();
                foreach (DataGridViewRow row in dgvDatos.SelectedRows)
                {
                    ElementosSeleccionados.Add(row.DataBoundItem);
                }
            }
            else
            {
                // Modo selección simple
                if (dgvDatos.CurrentRow == null)
                {
                    MessageBox.Show("Debe seleccionar un elemento", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ElementoSeleccionado = dgvDatos.CurrentRow.DataBoundItem;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            ElementoSeleccionado = null;
            ElementosSeleccionados.Clear();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
