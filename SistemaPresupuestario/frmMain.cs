using Services.DomainModel.Security.Composite;
using Services.Services.Contracts;
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
using SistemaPresupuestario.Seguridad; // NUEVO

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
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            txtUsuario.Text = $"Bienvenido {_login.user.Nombre}";
            ConfigurarVisibilidadMenu(); // Luego se ocultan los no autorizados
        }
        private void ConfigurarVisibilidadMenu()
        {
            try
            {
                // Obtener todos los FormNames autorizados
                var formNamesAutorizados = new HashSet<string>();
                ObtenerFormNamesAutorizados(_login.user.Permisos, formNamesAutorizados);

                // Procesar cada item del menú
                foreach (ToolStripMenuItem menuItem in menuStrip1.Items.OfType<ToolStripMenuItem>())
                {
                    // Saltar items que siempre deben estar visibles
                    if (EsItemFijo(menuItem))
                        continue;

                    // Procesar item y sus hijos recursivamente
                    ProcesarVisibilidadItem(menuItem, formNamesAutorizados);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al configurar el menú: {ex.Message}", "Error",
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

        // Retorna true si el item o alguno de sus hijos debe estar visible
        private bool ProcesarVisibilidadItem(ToolStripMenuItem menuItem, HashSet<string> formNamesAutorizados)
        {
            bool tieneHijosVisibles = false;

            // Procesar subitems recursivamente
            foreach (ToolStripMenuItem subItem in menuItem.DropDownItems.OfType<ToolStripMenuItem>())
            {
                bool subItemVisible = ProcesarVisibilidadItem(subItem, formNamesAutorizados);
                if (subItemVisible)
                {
                    tieneHijosVisibles = true;
                }
            }

            // El item es visible si está autorizado O tiene hijos visibles
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

        // ============= HANDLERS DE PRESUPUESTO =============

        /// <summary>
        /// Abre el formulario de presupuestos en modo GESTIONAR
        /// Muestra TODOS los presupuestos: Borrador, Emitido, Aprobado, Rechazado, Vencido, Facturado
        /// Permite: Crear, Editar/Eliminar Borradores, Ver el resto, Copiar, Emitir borradores
        /// </summary>
        private void tsGestionarCotizacion_Click(object sender, EventArgs e)
        {
            AbrirFormularioPresupuesto(ModoPresupuesto.Gestionar, "Gestión de Cotizaciones");
        }

        /// <summary>
        /// Abre el formulario de presupuestos en modo APROBAR
        /// Solo muestra presupuestos en estado Emitido
        /// Permite: Aprobar o Rechazar
        /// </summary>
        private void tsAprobarCotizacion_Click(object sender, EventArgs e)
        {
            AbrirFormularioPresupuesto(ModoPresupuesto.Aprobar, "Aprobar Cotizaciones");
        }

        /// <summary>
        /// Método helper para abrir el formulario de presupuestos en el modo especificado
        /// </summary>
        private void AbrirFormularioPresupuesto(ModoPresupuesto modo, string titulo)
        {
            // Verificar si ya hay una instancia abierta con el mismo modo
            var formAbierto = Application.OpenForms.OfType<frmPresupuesto>()
                .FirstOrDefault(f => !f.IsDisposed && f.Text == titulo);

            if (formAbierto != null)
            {
                formAbierto.BringToFront();
            }
            else
            {
                var hijo = _serviceProvider.GetService(typeof(frmPresupuesto)) as frmPresupuesto;
                hijo.EstablecerModo(modo); // IMPORTANTE: establecer modo antes de mostrar
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
                // Este formulario es autocontenido y no requiere inyección de dependencias
                var hijo = new frmDemoVerificadorProductos();
                hijo.MdiParent = this;
                hijo.Show();
            }
        }
    }
}