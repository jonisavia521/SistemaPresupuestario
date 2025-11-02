using BLL.Contracts;
using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaPresupuestario.Maestros.Productos
{
    public partial class frmProductoAlta : Form
    {
        private readonly IProductoService _productoService;
        private Guid? _productoId;
        private bool _esNuevo;

        // Evento para notificar al formulario padre cuando se guarda un producto
        public event EventHandler ProductoGuardado;

        // Propiedad pública para verificar qué producto está siendo editado
        public Guid? ProductoId => _productoId;

        public frmProductoAlta(IProductoService productoService, Guid? productoId = null)
        {
            InitializeComponent();
            _productoService = productoService;
            _productoId = productoId;
            _esNuevo = !productoId.HasValue;
        }

        private async void frmProductoAlta_Load(object sender, EventArgs e)
        {
            try
            {
                if (_esNuevo)
                {
                    this.Text = "Nuevo Producto";
                    chkInhabilitado.Visible = false;
                }
                else
                {
                    this.Text = "Editar Producto";
                    await CargarProducto();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el formulario: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private async Task CargarProducto()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                var producto = await _productoService.GetByIdAsync(_productoId.Value);

                if (producto == null)
                {
                    throw new Exception("No se encontró el producto especificado");
                }

                // Cargar los datos en los controles
                txtCodigo.Text = producto.Codigo;
                txtDescripcion.Text = producto.Descripcion;
                chkInhabilitado.Checked = producto.Inhabilitado;

                // En modo edición, el código no se puede cambiar
                txtCodigo.ReadOnly = true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar el producto: {ex.Message}", ex);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                // Limpiar errores previos
                errorProvider1.Clear();

                // Crear el DTO
                var productoDTO = new ProductoDTO
                {
                    Id = _productoId ?? Guid.Empty,
                    Codigo = txtCodigo.Text.Trim(),
                    Descripcion = txtDescripcion.Text.Trim(),
                    Inhabilitado = chkInhabilitado.Checked
                };

                // Validar el DTO usando DataAnnotations
                var contextoValidacion = new ValidationContext(productoDTO, null, null);
                var resultadosValidacion = new List<ValidationResult>();

                if (!Validator.TryValidateObject(productoDTO, contextoValidacion, resultadosValidacion, true))
                {
                    // Mostrar los errores usando ErrorProvider
                    foreach (var error in resultadosValidacion)
                    {
                        var nombrePropiedad = error.MemberNames.FirstOrDefault();
                        if (!string.IsNullOrEmpty(nombrePropiedad))
                        {
                            Control control = ObtenerControlPorPropiedad(nombrePropiedad);
                            if (control != null)
                            {
                                errorProvider1.SetError(control, error.ErrorMessage);
                            }
                        }
                    }
                    return;
                }

                // Guardar
                this.Cursor = Cursors.WaitCursor;

                bool resultado;
                if (_esNuevo)
                {
                    resultado = await _productoService.AddAsync(productoDTO);
                }
                else
                {
                    resultado = await _productoService.UpdateAsync(productoDTO);
                }

                if (resultado)
                {
                    MessageBox.Show("Producto guardado exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Notificar al formulario padre
                    ProductoGuardado?.Invoke(this, EventArgs.Empty);

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el producto: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Obtiene el control asociado a una propiedad del DTO para mostrar errores
        /// </summary>
        private Control ObtenerControlPorPropiedad(string nombrePropiedad)
        {
            switch (nombrePropiedad)
            {
                case nameof(ProductoDTO.Codigo):
                    return txtCodigo;
                case nameof(ProductoDTO.Descripcion):
                    return txtDescripcion;
                default:
                    return null;
            }
        }
    }
}
