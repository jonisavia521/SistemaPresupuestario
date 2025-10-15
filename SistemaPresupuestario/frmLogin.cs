using Services.Services;
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
    public partial class frmLogin : Form
    {
        private ILogin _login;
        public frmLogin(ILogin login)
        {
            _login = login;
            InitializeComponent();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            
            this.Cursor = Cursors.WaitCursor;
            if (_login.Login(txtUser.Text, txtPassword.Text))
            {
                this.DialogResult = DialogResult.OK;
            }
            else
                MessageBox.Show("Usuario o contraseña invalida","ADVERTENCIA",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            this.Cursor = Cursors.Default;
        }

        private void frmLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnAceptar_Click(sender, e);
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            txtUser.Select();
        }
    }
}
