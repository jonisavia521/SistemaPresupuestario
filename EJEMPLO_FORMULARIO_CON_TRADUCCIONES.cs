using SistemaPresupuestario.Helpers;
using System;
using System.Windows.Forms;

namespace SistemaPresupuestario.Ejemplo
{
    /// <summary>
    /// EJEMPLO COMPLETO: Formulario con traducciones dinámicas
    /// Este es un ejemplo de referencia de cómo implementar traducciones en cualquier formulario
    /// </summary>
    public partial class frmEjemploConTraducciones : Form
    {
        public frmEjemploConTraducciones()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Evento Load del formulario
        /// IMPORTANTE: Siempre llamar a AplicarTraducciones() aquí
        /// </summary>
        private void frmEjemploConTraducciones_Load(object sender, EventArgs e)
        {
            // 1. PRIMERO: Aplicar traducciones
            AplicarTraducciones();
            
            // 2. DESPUÉS: Cargar datos y realizar otras inicializaciones
            CargarDatos();
        }

        /// <summary>
        /// Aplica las traducciones dinámicas a todos los controles del formulario
        /// Este método debe ser llamado en el evento Load
        /// </summary>
        private void AplicarTraducciones()
        {
            try
            {
                // ==================== TÍTULO DEL FORMULARIO ====================
                this.Text = I18n.T("Gestión de Clientes");
                
                // ==================== BOTONES ====================
                btnNuevo.Text = I18n.T("Nuevo");
                btnEditar.Text = I18n.T("Editar");
                btnDesactivar.Text = I18n.T("Desactivar");
                btnReactivar.Text = I18n.T("Reactivar");
                btnCerrar.Text = I18n.T("Cerrar");
                
                // ==================== LABELS ====================
                lblBuscar.Text = I18n.T("Buscar") + ":";
                lblTotalRegistros.Text = I18n.T("Total registros") + ":";
                
                // ==================== CHECKBOX ====================
                chkSoloActivos.Text = I18n.T("Solo ver activos");
                
                // ==================== GROUPBOX ====================
                grpDatosBasicos.Text = I18n.T("Datos Básicos");
                grpDatosContacto.Text = I18n.T("Datos de Contacto (Opcional)");
                
                // ==================== COLUMNAS DE DATAGRIDVIEW ====================
                colCodigo.HeaderText = I18n.T("Código");
                colRazonSocial.HeaderText = I18n.T("Razón Social");
                colDocumento.HeaderText = I18n.T("Documento");
                colTipoIVA.HeaderText = I18n.T("Tipo IVA");
                colEmail.HeaderText = I18n.T("Email");
                colTelefono.HeaderText = I18n.T("Teléfono");
                colEstado.HeaderText = I18n.T("Estado");
                
                // ==================== TABS (SI HAY PESTAÑAS) ====================
                tabDatos.Text = I18n.T("Datos Básicos");
                tabContacto.Text = I18n.T("Datos de Contacto (Opcional)");
                tabComercial.Text = I18n.T("Datos Comerciales");
                
                // ==================== TOOLTIPS ====================
                toolTip1.SetToolTip(btnNuevo, I18n.T("Crear un nuevo registro"));
                toolTip1.SetToolTip(btnEditar, I18n.T("Modificar el registro seleccionado"));
                toolTip1.SetToolTip(btnDesactivar, I18n.T("Desactivar el registro seleccionado"));
            }
            catch (Exception ex)
            {
                // En caso de error, continuar sin traducciones
                // No interrumpir el funcionamiento del formulario
                System.Diagnostics.Debug.WriteLine($"Error al aplicar traducciones: {ex.Message}");
            }
        }

        /// <summary>
        /// EJEMPLO: Validación con mensaje traducido
        /// </summary>
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    I18n.T("Debe seleccionar un cliente para editar"),
                    I18n.T("Validación"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            
            // Abrir formulario de edición
            AbrirFormularioEdicion();
        }

        /// <summary>
        /// EJEMPLO: Confirmación con mensajes traducidos
        /// </summary>
        private void btnDesactivar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    I18n.T("Debe seleccionar un cliente para desactivar"),
                    I18n.T("Validación"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Obtener el nombre del cliente seleccionado
            string nombreCliente = dgvClientes.SelectedRows[0].Cells["colRazonSocial"].Value?.ToString();

            // Confirmación
            var resultado = MessageBox.Show(
                $"{I18n.T("¿Está seguro que desea desactivar el cliente")} {nombreCliente}?",
                I18n.T("Confirmar Eliminación"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2); // Botón No por defecto

            if (resultado == DialogResult.Yes)
            {
                try
                {
                    // Lógica para desactivar el cliente
                    DesactivarCliente();
                    
                    // Mensaje de éxito
                    MessageBox.Show(
                        I18n.T("Cliente desactivado exitosamente"),
                        I18n.T("Éxito"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    
                    // Recargar datos
                    CargarDatos();
                }
                catch (Exception ex)
                {
                    // Mensaje de error
                    MessageBox.Show(
                        $"{I18n.T("Error al desactivar cliente")}: {ex.Message}",
                        I18n.T("Error"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// EJEMPLO: Reactivación con validación
        /// </summary>
        private void btnReactivar_Click(object sender, EventArgs e)
        {
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    I18n.T("Debe seleccionar un cliente para reactivar"),
                    I18n.T("Validación"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Verificar si ya está activo
            bool estaActivo = (bool)dgvClientes.SelectedRows[0].Cells["colEstado"].Value;
            if (estaActivo)
            {
                MessageBox.Show(
                    I18n.T("El cliente ya está activo"),
                    I18n.T("Información"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            try
            {
                ReactivarCliente();
                
                MessageBox.Show(
                    I18n.T("Cliente reactivado exitosamente"),
                    I18n.T("Éxito"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                
                CargarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{I18n.T("Error al reactivar cliente")}: {ex.Message}",
                    I18n.T("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// EJEMPLO: Búsqueda con placeholder traducido
        /// </summary>
        private void txtBuscar_Enter(object sender, EventArgs e)
        {
            if (txtBuscar.Text == I18n.T("Buscar por código, razón social o CUIT..."))
            {
                txtBuscar.Text = "";
                txtBuscar.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void txtBuscar_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBuscar.Text))
            {
                txtBuscar.Text = I18n.T("Buscar por código, razón social o CUIT...");
                txtBuscar.ForeColor = System.Drawing.Color.Gray;
            }
        }

        /// <summary>
        /// EJEMPLO: Actualización de contador con texto traducido
        /// </summary>
        private void ActualizarContador()
        {
            int totalRegistros = dgvClientes.Rows.Count;
            lblTotalRegistros.Text = $"{I18n.T("Total registros")}: {totalRegistros}";
        }

        /// <summary>
        /// EJEMPLO: ComboBox con items traducidos
        /// </summary>
        private void CargarComboTipoIVA()
        {
            cboTipoIVA.Items.Clear();
            cboTipoIVA.Items.Add(I18n.T("RESPONSABLE INSCRIPTO"));
            cboTipoIVA.Items.Add(I18n.T("MONOTRIBUTISTA"));
            cboTipoIVA.Items.Add(I18n.T("EXENTO"));
            cboTipoIVA.Items.Add(I18n.T("CONSUMIDOR FINAL"));
            cboTipoIVA.Items.Add(I18n.T("NO RESPONSABLE"));
        }

        /// <summary>
        /// EJEMPLO: SaveFileDialog con filtros traducidos
        /// </summary>
        private void ExportarDatos()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = I18n.T("Archivos de Excel (*.xlsx)|*.xlsx|Todos los archivos (*.*)|*.*");
                saveFileDialog.Title = I18n.T("Exportar Datos");
                saveFileDialog.DefaultExt = "xlsx";
                saveFileDialog.FileName = $"Clientes_{DateTime.Now:yyyyMMdd}.xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Lógica de exportación
                        MessageBox.Show(
                            $"{I18n.T("Datos exportados exitosamente a")}: {saveFileDialog.FileName}",
                            I18n.T("Éxito"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"{I18n.T("Error al exportar datos")}: {ex.Message}",
                            I18n.T("Error"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// EJEMPLO: Uso de traducciones en lógica de negocio
        /// </summary>
        private void ValidarFormulario()
        {
            var errores = new System.Text.StringBuilder();

            if (string.IsNullOrWhiteSpace(txtRazonSocial.Text))
            {
                errores.AppendLine($"• {I18n.T("Razón Social")}: {I18n.T("El campo es requerido")}");
            }

            if (string.IsNullOrWhiteSpace(txtCUIT.Text))
            {
                errores.AppendLine($"• {I18n.T("CUIT")}: {I18n.T("El campo es requerido")}");
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                errores.AppendLine($"• {I18n.T("Email")}: {I18n.T("El campo es requerido")}");
            }
            else if (!EsEmailValido(txtEmail.Text))
            {
                errores.AppendLine($"• {I18n.T("Email")}: {I18n.T("Formato inválido")}");
            }

            if (errores.Length > 0)
            {
                MessageBox.Show(
                    errores.ToString(),
                    I18n.T("Validación"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // ==================== MÉTODOS AUXILIARES (STUB) ====================
        
        private void CargarDatos()
        {
            // Implementar lógica de carga de datos
            ActualizarContador();
        }

        private void AbrirFormularioEdicion()
        {
            // Implementar apertura de formulario de edición
        }

        private void DesactivarCliente()
        {
            // Implementar lógica de desactivación
        }

        private void ReactivarCliente()
        {
            // Implementar lógica de reactivación
        }

        private bool EsEmailValido(string email)
        {
            // Implementar validación de email
            return email.Contains("@");
        }
    }
}
