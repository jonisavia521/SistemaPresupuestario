using Services.DomainModel.Security.Composite;
using Services.Services.Contracts;
using SistemaPresupuestario.Maestros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

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
    }
}