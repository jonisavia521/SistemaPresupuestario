using Services.Services;
using Services.Services.Contracts;
using SistemaPresupuestario.Helpers;
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
            
            // ✅ TRADUCCIÓN AUTOMÁTICA: Aplicar traducciones a TODOS los controles
            FormTranslator.Translate(this);
            
            // ✅ TRADUCCIÓN DINÁMICA: Suscribirse al evento de cambio de idioma
            I18n.LanguageChanged += OnLanguageChanged;
            this.FormClosed += (s, e) => I18n.LanguageChanged -= OnLanguageChanged;
        }
        
        /// <summary>
        /// Manejador del evento de cambio de idioma
        /// </summary>
        private void OnLanguageChanged(object sender, EventArgs e)
        {
            FormTranslator.Translate(this);
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (_login.Login(txtUser.Text, txtPassword.Text))
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(
                    I18n.T("Usuario o contraseña invalida"),
                    I18n.T("ADVERTENCIA"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
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
