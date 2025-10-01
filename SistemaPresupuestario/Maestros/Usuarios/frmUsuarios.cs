using BLL.Contracts;
using BLL.Contracts.Seguridad;
using BLL.DTOs;
using Services.Services.Contracts;
using SistemaPresupuestario.Maestros.Seguridad;
using SistemaPresupuestario.Maestros.Usuarios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros
{
    public partial class frmUsuarios : Form
    {
        private readonly IUsuarioBusinessService usuarioService;
        private readonly IServiceProvider _serviceProvider;

        public frmUsuarios(IUsuarioBusinessService usuarioService, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            this.usuarioService = usuarioService;
            _serviceProvider = serviceProvider;
        }
       
        private async void frmUsuarios_Load(object sender, EventArgs e)
        {

            var usuarios = await usuarioService.GetAllUsersAsync();
            dgvUsuarios.DataSource = usuarios.ToList();
            dgvUsuarios.Refresh();
        }
        private void btnNuevo_Click(object sender, EventArgs e)
        {
        


            FrmUsuarioEdit formAbierto = Application.OpenForms.OfType<FrmUsuarioEdit>().FirstOrDefault();

            if (formAbierto == null)
            {
                // Crear una nueva instancia si el formulario no está abierto
                FrmUsuarioEdit hijo = _serviceProvider.GetService(typeof(FrmUsuarioEdit)) as FrmUsuarioEdit;
                hijo.MdiParent = this.MdiParent;

                hijo.Show();
            }
            else
            {
                // Si ya está abierto, traerlo al frente
                formAbierto.BringToFront();
            }

        }

        private void btnEditar_Click(object sender, EventArgs e)
        {

        }
    }
}
