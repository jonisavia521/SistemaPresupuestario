using Services.Services.Contracts;
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
        public frmMain(ILogin login)
        {
            InitializeComponent();
            _login = login;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            txtUsuario.Text = $"Bienvenido {_login.user.Nombre}";            
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
