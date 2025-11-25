using Services.DomainModel.Security.Composite;
using Services.Services.Contracts;
using SistemaPresupuestario.Helpers;
using SistemaPresupuestario.Maestros;
using SistemaPresupuestario.Maestros.Productos;
using SistemaPresupuestario.Presupuesto;
using BLL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SistemaPresupuestario.Maestros.ListaPrecio;
using SistemaPresupuestario.Configuracion;
using SistemaPresupuestario.Venta.Arba;
using SistemaPresupuestario.Venta.Factura;
using SistemaPresupuestario.Seguridad;

namespace SistemaPresupuestario
{
    public partial class frmMain : Form
    {
        private readonly ILogin _login;
        private readonly IServiceProvider _serviceProvider;

        public frmMain(ILogin login, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _login = login;
            _serviceProvider = serviceProvider;
            
            FormTranslator.Translate(this);
            
            I18n.LanguageChanged += OnLanguageChanged;
            this.FormClosed += (s, e) => I18n.LanguageChanged -= OnLanguageChanged;
        }
        
        /// <summary>
        /// Manejador del evento de cambio de idioma
        /// </summary>
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            FormTranslator.Translate(this);
            txtUsuario.Text = $"{I18n.T("Bienvenido")} {_login.user.Nombre}";
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            txtUsuario.Text = $"{I18n.T("Bienvenido")} {_login.user.Nombre}";
            ConfigurarVisibilidadMenu();
        }
        
        private void ConfigurarVisibilidadMenu()
        {
            try
            {
                var formNamesAutorizados = new HashSet<string>();
                ObtenerFormNamesAutorizados(_login.user.Permisos, formNamesAutorizados);

                foreach (ToolStripMenuItem menuItem in menuStrip1.Items.OfType<ToolStripMenuItem>())
                {
                    if (EsItemFijo(menuItem))
                        continue;

                    ProcesarVisibilidadItem(menuItem, formNamesAutorizados);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{I18n.T("Error al configurar el menú")}: {ex.Message}", I18n.T("Error"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool EsItemFijo(ToolStripItem item)
        {
            return item == tsPrincipalArchivo ||
                   item == tsPrincipalAyuda ||
                   item == txtUsuario;
        }

        private void ObtenerFormNamesAutorizados(List<Component> permisos, HashSet<string> formNamesAutorizados)
        {
            foreach (var permiso in permisos)
            {
                if (permiso is Familia familia)
                {
                    if (!string.IsNullOrWhiteSpace(familia.FormName))
                    {
                        formNamesAutorizados.Add(familia.FormName);
                    }
                    ObtenerFormNamesAutorizados(familia.GetChildrens(), formNamesAutorizados);
                }
                else if (permiso is Patente patente)
                {
                    if (!string.IsNullOrWhiteSpace(patente.FormName))
                    {
                        formNamesAutorizados.Add(patente.FormName);
                    }
                }
            }
        }

        /// <summary>
        /// Determina si el item o alguno de sus hijos debe estar visible
        /// </summary>
        private bool ProcesarVisibilidadItem(ToolStripMenuItem menuItem, HashSet<string> formNamesAutorizados)
        {
            bool tieneHijosVisibles = false;

            foreach (ToolStripMenuItem subItem in menuItem.DropDownItems.OfType<ToolStripMenuItem>())
            {
                bool subItemVisible = ProcesarVisibilidadItem(subItem, formNamesAutorizados);
                if (subItemVisible)
                {
                    tieneHijosVisibles = true;
                }
            }

            bool estaAutorizado = formNamesAutorizados.Contains(menuItem.Name);
            bool debeSerVisible = estaAutorizado || tieneHijosVisibles;

            menuItem.Visible = debeSerVisible;
            return debeSerVisible;
        }


        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var formAbierto = Application.OpenForms.OfType<frmUsuarios>()
                .FirstOrDefault(f => !f.IsDisposed);

            if (formAbierto != null)
            {
                formAbierto.BringToFront();
            }
            else
            {
                var hijo = _serviceProvider.GetService(typeof(frmUsuarios)) as frmUsuarios;
                hijo.MdiParent = this;
                hijo.Show();
            }
        }

        private void tsCliente_Click(object sender, EventArgs e)
        {
            var formAbierto = Application.OpenForms.OfType<frmClientes>()
                .FirstOrDefault(f => !f.IsDisposed);

            if (formAbierto != null)
            {
                formAbierto.BringToFront();
            }
            else
            {
                var hijo = _serviceProvider.GetService(typeof(frmClientes)) as frmClientes;
                hijo.MdiParent = this;
                hijo.Show();
            }
        }

        private void tsVendedor_Click(object sender, EventArgs e)
        {
            var formAbierto = Application.OpenForms.OfType<frmVendedores>()
                .FirstOrDefault(f => !f.IsDisposed);

            if (formAbierto != null)
            {
                formAbierto.BringToFront();
            }
            else
            {
                var hijo = _serviceProvider.GetService(typeof(frmVendedores)) as frmVendedores;
                hijo.MdiParent = this;
                hijo.Show();
            }
        }

        private void tsProducto_Click(object sender, EventArgs e)
        {
            var formAbierto = Application.OpenForms.OfType<frmProductos>()
                .FirstOrDefault(f => !f.IsDisposed);

            if (formAbierto != null)
            {
                formAbierto.BringToFront();
            }
            else
            {
                var hijo = _serviceProvider.GetService(typeof(frmProductos)) as frmProductos;
                hijo.MdiParent = this;
                hijo.Show();
            }
        }

        /// <summary>
        /// Abre el formulario de presupuestos en modo GESTIONAR.
        /// Muestra todos los presupuestos y permite crear, editar, eliminar y copiar.
        /// </summary>
        private void tsGestionarCotizacion_Click(object sender, EventArgs e)
        {
            AbrirFormularioPresupuesto(ModoPresupuesto.Gestionar, I18n.T("Gestión de Cotizaciones"));
        }

        /// <summary>
        /// Abre el formulario de presupuestos en modo APROBAR.
        /// Solo muestra presupuestos en estado Emitido y permite aprobar o rechazar.
        /// </summary>
        private void tsAprobarCotizacion_Click(object sender, EventArgs e)
        {
            AbrirFormularioPresupuesto(ModoPresupuesto.Aprobar, I18n.T("Aprobar Cotizaciones"));
        }

        /// <summary>
        /// Abre el formulario de presupuestos en el modo especificado
        /// </summary>
        private void AbrirFormularioPresupuesto(ModoPresupuesto modo, string titulo)
        {
            var formAbierto = Application.OpenForms.OfType<frmPresupuesto>()
                .FirstOrDefault(f => !f.IsDisposed && f.Text == titulo);

            if (formAbierto != null)
            {
                formAbierto.BringToFront();
            }
            else
            {
                var hijo = _serviceProvider.GetService(typeof(frmPresupuesto)) as frmPresupuesto;
                hijo.EstablecerModo(modo);
                hijo.MdiParent = this;
                hijo.Text = titulo;
                hijo.Show();
            }
        }

        private void tsListaPrecio_Click(object sender, EventArgs e)
        {
            var formAbierto = Application.OpenForms.OfType<frmListaPrecios>()
               .FirstOrDefault(f => !f.IsDisposed);

            if (formAbierto != null)
            {
                formAbierto.BringToFront();
            }
            else
            {
                var hijo = _serviceProvider.GetService(typeof(frmListaPrecios)) as frmListaPrecios;
                hijo.MdiParent = this;
                hijo.Show();
            }
        }

        private void tsConfiguracion_Click(object sender, EventArgs e)
        {
            var formAbierto = Application.OpenForms.OfType<frmConfiguacionGeneral>()
               .FirstOrDefault(f => !f.IsDisposed);

            if (formAbierto != null)
            {
                formAbierto.BringToFront();
            }
            else
            {
                var hijo = _serviceProvider.GetService(typeof(frmConfiguacionGeneral)) as frmConfiguacionGeneral;
                hijo.MdiParent = this;
                hijo.Show();
            }
        }

        private void tsArba_Click(object sender, EventArgs e)
        {
            var formAbierto = Application.OpenForms.OfType<frmActualizarPadronArba>()
               .FirstOrDefault(f => !f.IsDisposed);

            if (formAbierto != null)
            {
                formAbierto.BringToFront();
            }
            else
            {
                var hijo = _serviceProvider.GetService(typeof(frmActualizarPadronArba)) as frmActualizarPadronArba;
                hijo.MdiParent = this;
                hijo.Show();
            }
        }

        private void tsFactura_Click(object sender, EventArgs e)
        {
            var formAbierto = Application.OpenForms.OfType<frmFacturar>()
               .FirstOrDefault(f => !f.IsDisposed);

            if (formAbierto != null)
            {
                formAbierto.BringToFront();
            }
            else
            {
                var hijo = _serviceProvider.GetService(typeof(frmFacturar)) as frmFacturar;
                hijo.MdiParent = this;
                hijo.Show();
            }
        }

        private void tsDigitoVerificador_Click(object sender, EventArgs e)
        {
            var formAbierto = Application.OpenForms.OfType<frmDemoVerificadorProductos>()
               .FirstOrDefault(f => !f.IsDisposed);

            if (formAbierto != null)
            {
                formAbierto.BringToFront();
            }
            else
            {
                var hijo = new frmDemoVerificadorProductos();
                hijo.MdiParent = this;
                hijo.Show();
            }
        }

        private void tsManualDeUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                // Ruta al archivo CHM compilado
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string chmFilePath = System.IO.Path.Combine(basePath, "Documentacion_CHM", "SistemaPresupuestario_Ayuda.chm");

                // Verificar si el archivo existe
                if (!System.IO.File.Exists(chmFilePath))
                {
                    MessageBox.Show(
                        $"{I18n.T("El archivo de ayuda no se encuentra en la ruta")}:\n{chmFilePath}\n\n",
                        I18n.T("Archivo no encontrado"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // Abrir el archivo CHM
                Help.ShowHelp(this, chmFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"{I18n.T("Error al abrir el manual de usuario")}:\n{ex.Message}",
                    I18n.T("Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}