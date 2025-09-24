using BLL.Contracts;
using BLL.DTOs;
using Services.Services.Contracts;
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
        private readonly IUsuarioService usuarioService;

        public frmUsuarios(IUsuarioService usuarioService)
        {
            InitializeComponent();
            this.usuarioService = usuarioService;
        }
       
        private async void frmUsuarios_Load(object sender, EventArgs e)
        {

            var usuarios = await usuarioService.GetAllAsync();
            dgvUsuarios.DataSource = usuarios.ToList();
            dgvUsuarios.Refresh();
        }
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            // Verificar si el formulario 'OtroFormulario' ya está abierto
            frmAlta formAbierto = Application.OpenForms.OfType<frmAlta>().FirstOrDefault();

            if (formAbierto == null)
            {
                // Crear una nueva instancia si el formulario no está abierto
                frmAlta otroForm = new frmAlta()
                {
                    MdiParent = this.MdiParent // Mantener el mismo MdiParent si lo estás utilizando
                };
                otroForm.Show();
            }
            else
            {
                // Si ya está abierto, traerlo al frente
                formAbierto.BringToFront();
            }

        }


    }
}
