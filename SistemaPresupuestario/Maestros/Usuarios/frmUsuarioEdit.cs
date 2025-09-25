using BLL.DTOs;
using Services.Services.Seguridad;
using System;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros.Usuarios
{
    /// <summary>
    /// Formulario para crear y editar usuarios
    /// </summary>
    public partial class frmUsuarioEdit : Form
    {
        private readonly IUsuarioService _usuarioService;
        private Guid? _usuarioId;

        /// <summary>
        /// Constructor para crear nuevo usuario
        /// </summary>
        public frmUsuarioEdit(IUsuarioService usuarioService)
        {
            InitializeComponent();
            _usuarioService = usuarioService;
            _usuarioId = null;
        }

        /// <summary>
        /// Constructor para editar usuario existente
        /// </summary>
        public frmUsuarioEdit(IUsuarioService usuarioService, Guid usuarioId)
        {
            InitializeComponent();
            _usuarioService = usuarioService;
            _usuarioId = usuarioId;
        }
    }
}