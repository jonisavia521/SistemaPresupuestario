using Services.Services.Contracts;
using SistemaPresupuestario.Maestros;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaPresupuestario
{
    public partial class frmMain : Form
    {
        ILogin _login;
        private readonly IServiceProvider serviceProvider;

        public frmMain(ILogin login,IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _login = login;
            this.serviceProvider = serviceProvider;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            txtUsuario.Text = $"Bienvenido {_login.user.Nombre}";            
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Verificar si el formulario hijo ya está abierto
            frmUsuarios formAbierto = Application.OpenForms.OfType<frmUsuarios>().FirstOrDefault();

            if (formAbierto == null)
            {
                // Crear una nueva instancia si el formulario no está abierto
                //Microsoft.Extensions.Logging.Abstractions
                frmUsuarios hijo = serviceProvider.GetService(typeof(frmUsuarios)) as frmUsuarios;
                hijo.MdiParent = this;
                
                hijo.Show();
            }
            else
            {
                // Si ya está abierto, traerlo al frente
                formAbierto.BringToFront();
            }
        }
    }
}
