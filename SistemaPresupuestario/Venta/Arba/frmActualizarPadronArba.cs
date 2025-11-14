using BLL.Contracts;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace SistemaPresupuestario.Venta.Arba
{
    /// <summary>
    /// Formulario para actualizar masivamente las alícuotas ARBA
    /// de todos los clientes Responsables Inscriptos
    /// </summary>
    public partial class frmActualizarPadronArba : Form
    {
        private readonly IClienteService _clienteService;
        private BackgroundWorker _worker;
        private int _clientesProcesados = 0;
        private int _totalClientes = 0;

        public frmActualizarPadronArba(IClienteService clienteService)
        {
            InitializeComponent();
            _clienteService = clienteService ?? throw new ArgumentNullException(nameof(clienteService));
            
            ConfigurarBackgroundWorker();
        }

        /// <summary>
        /// Configura el BackgroundWorker para procesar la actualización en segundo plano
        /// </summary>
        private void ConfigurarBackgroundWorker()
        {
            _worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            _worker.DoWork += Worker_DoWork;
            _worker.ProgressChanged += Worker_ProgressChanged;
            _worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void frmActualizarPadronArba_Load(object sender, EventArgs e)
        {
            // Inicializar controles
            progressBar.Value = 0;
            progressBar.Style = ProgressBarStyle.Blocks;
            lblEstado.Text = "Listo para actualizar padrón ARBA";
            lblProgreso.Text = "";
            
            btnActualizar.Enabled = true;
            btnCancelar.Enabled = false;
            btnCerrar.Enabled = true;
        }

        /// <summary>
        /// Inicia el proceso de actualización
        /// </summary>
        private void btnActualizar_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "¿Está seguro de actualizar las alícuotas ARBA?\n\n" +
                "Esta operación actualizará TODOS los clientes con tipo de IVA 'RESPONSABLE INSCRIPTO' " +
                "asignándoles alícuotas aleatorias entre 0.5% y 5%.\n\n" +
                "Este proceso puede tardar unos momentos.",
                "Confirmar Actualización",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            // Deshabilitar botones
            btnActualizar.Enabled = false;
            btnCancelar.Enabled = true;
            btnCerrar.Enabled = false;

            // Resetear progreso
            progressBar.Value = 0;
            lblEstado.Text = "Iniciando actualización...";
            lblProgreso.Text = "";

            // Iniciar el proceso en segundo plano
            _worker.RunWorkerAsync();
        }

        /// <summary>
        /// Trabajo que se ejecuta en segundo plano
        /// </summary>
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var worker = sender as BackgroundWorker;

                // Reportar inicio
                worker.ReportProgress(0, "Obteniendo clientes Responsables Inscriptos...");
                System.Threading.Thread.Sleep(500); // Simular carga

                // Obtener total de clientes para calcular progreso
                var clientesTotal = _clienteService.GetAll();
                _totalClientes = 0;
                
                foreach (var cliente in clientesTotal)
                {
                    if (cliente.TipoIva != null && 
                        cliente.TipoIva.ToUpper().Contains("RESPONSABLE INSCRIPTO"))
                    {
                        _totalClientes++;
                    }
                }

                if (_totalClientes == 0)
                {
                    worker.ReportProgress(0, "No hay clientes Responsables Inscriptos para actualizar.");
                    e.Result = 0;
                    return;
                }

                worker.ReportProgress(5, $"Se encontraron {_totalClientes} clientes para actualizar...");
                System.Threading.Thread.Sleep(500);

                // Realizar la actualización
                worker.ReportProgress(10, "Actualizando alícuotas...");
                
                // Simular progreso mientras se actualiza
                for (int i = 10; i <= 90; i += 5)
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    worker.ReportProgress(i, "Procesando clientes...");
                    System.Threading.Thread.Sleep(200); // Simular trabajo
                }

                // Ejecutar la actualización real
                int clientesActualizados = _clienteService.ActualizarAlicuotasArba();

                worker.ReportProgress(100, "Actualización completada.");
                e.Result = clientesActualizados;
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        /// <summary>
        /// Se ejecuta cuando hay cambios de progreso
        /// </summary>
        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            lblEstado.Text = e.UserState?.ToString() ?? "Procesando...";
            
            if (e.ProgressPercentage > 0 && _totalClientes > 0)
            {
                lblProgreso.Text = $"{e.ProgressPercentage}% completado";
            }
        }

        /// <summary>
        /// Se ejecuta cuando el trabajo termina
        /// </summary>
        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Rehabilitar botones
            btnActualizar.Enabled = true;
            btnCancelar.Enabled = false;
            btnCerrar.Enabled = true;

            if (e.Cancelled)
            {
                lblEstado.Text = "Actualización cancelada por el usuario.";
                lblProgreso.Text = "";
                progressBar.Value = 0;
                
                MessageBox.Show(
                    "La actualización fue cancelada.",
                    "Cancelado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (e.Error != null || e.Result is Exception)
            {
                var error = e.Error ?? (Exception)e.Result;
                
                lblEstado.Text = "Error al actualizar padrón.";
                lblProgreso.Text = "";
                progressBar.Value = 0;
                
                MessageBox.Show(
                    $"Error al actualizar el padrón ARBA:\n\n{error.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Actualización exitosa
            int clientesActualizados = (int)e.Result;
            
            lblEstado.Text = $"Actualización completada: {clientesActualizados} clientes actualizados.";
            lblProgreso.Text = "100% completado";
            progressBar.Value = 100;

            MessageBox.Show(
                $"Actualización completada exitosamente.\n\n" +
                $"Clientes actualizados: {clientesActualizados}",
                "Éxito",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// Cancela el proceso en ejecución
        /// </summary>
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (_worker != null && _worker.IsBusy)
            {
                lblEstado.Text = "Cancelando...";
                _worker.CancelAsync();
            }
        }

        /// <summary>
        /// Cierra el formulario
        /// </summary>
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            if (_worker != null && _worker.IsBusy)
            {
                MessageBox.Show(
                    "No se puede cerrar el formulario mientras se está actualizando el padrón.\n\n" +
                    "Por favor, espere a que termine o cancele la operación.",
                    "Advertencia",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            this.Close();
        }

        /// <summary>
        /// Limpia los recursos al cerrar
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_worker != null && _worker.IsBusy)
            {
                e.Cancel = true;
                MessageBox.Show(
                    "No se puede cerrar el formulario mientras se está actualizando el padrón.\n\n" +
                    "Por favor, espere a que termine o cancele la operación.",
                    "Advertencia",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            base.OnFormClosing(e);
        }
    }
}
