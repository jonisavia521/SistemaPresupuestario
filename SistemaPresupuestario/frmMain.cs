using Services.Services.Contracts;
using SistemaPresupuestario.Maestros;
using System;
using System.Linq;
using System.Windows.Forms;

namespace SistemaPresupuestario
{
    public partial class frmMain : Form
    {
        ILogin _login;
        private readonly IServiceProvider serviceProvider;

        public frmMain(ILogin login, IServiceProvider serviceProvider)
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
            frmUsuarios formAbierto = Application.OpenForms.OfType<frmUsuarios>()
                .FirstOrDefault(f => !f.IsDisposed);

            if (formAbierto != null)
            {
                formAbierto.BringToFront();
            }
            else
            {
                var hijo = serviceProvider.GetService(typeof(frmUsuarios)) as frmUsuarios;
                hijo.MdiParent = this;
                hijo.Show();
            }
        }
    }
}