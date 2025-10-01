using BLL.Contracts;
using BLL.Contracts.Seguridad;
using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros.Usuarios
{
    public partial class frmAlta : Form
    {
        private readonly IUsuarioBusinessService usuarioService;

        public frmAlta(IUsuarioBusinessService usuarioService)
        {
            InitializeComponent();
            this.usuarioService = usuarioService;
        }

        private void frmAlta_Load(object sender, EventArgs e)
        {
            
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            // TODO: Implementar lógica de alta de usuario usando IUsuarioService
            // Ejemplo de cómo sería:
            //var nuevoUsuario = new UsuarioDTO { ... };
            //usuarioService.Add(nuevoUsuario);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
