using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainModel.Domain
{
    /// <summary>
    /// Entidad de dominio ListaPrecio - Representa una lista de precios con sus detalles
    /// </summary>
    public class ListaPrecioDM
    {
        public Guid Id { get; private set; }
        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public bool Activo { get; private set; }
        public bool IncluyeIva { get; private set; }
        public DateTime FechaAlta { get; private set; }
        public DateTime? FechaModificacion { get; private set; }
        public List<ListaPrecioDetalleDM> Detalles { get; private set; }

        // Constructor para creación inicial
        public ListaPrecioDM(string codigo, string nombre, bool incluyeIva = false)
        {
            Id = Guid.NewGuid();
            Activo = true;
            IncluyeIva = incluyeIva;
            FechaAlta = DateTime.Now;
            Detalles = new List<ListaPrecioDetalleDM>();

            ValidarYEstablecerCodigo(codigo);
            ValidarYEstablecerNombre(nombre);
        }

        // Constructor para cargar desde base de datos
        public ListaPrecioDM(
            Guid id,
            string codigo,
            string nombre,
            bool activo,
            DateTime fechaAlta,
            DateTime? fechaModificacion,
            List<ListaPrecioDetalleDM> detalles = null,
            bool incluyeIva = false)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("El ID no puede ser vacío.", nameof(id));

            Id = id;
            Codigo = codigo;
            Nombre = nombre;
            Activo = activo;
            IncluyeIva = incluyeIva;
            FechaAlta = fechaAlta;
            FechaModificacion = fechaModificacion;
            Detalles = detalles ?? new List<ListaPrecioDetalleDM>();
        }

        // Método de actualización
        public void ActualizarDatos(string nombre, bool incluyeIva)
        {
            ValidarYEstablecerNombre(nombre);
            IncluyeIva = incluyeIva;
            FechaModificacion = DateTime.Now;
        }

        // Gestión de detalles
        public void AgregarDetalle(ListaPrecioDetalleDM detalle)
        {
            if (detalle == null)
                throw new ArgumentNullException(nameof(detalle));

            var detalleExistente = Detalles.FirstOrDefault(d => d.IdProducto == detalle.IdProducto);
            if (detalleExistente != null)
            {
                detalleExistente.ActualizarPrecio(detalle.Precio);
            }
            else
            {
                detalle.EstablecerListaPrecio(Id);
                Detalles.Add(detalle);
            }

            FechaModificacion = DateTime.Now;
        }

        public void RemoverDetalle(Guid detalleId)
        {
            var detalle = Detalles.FirstOrDefault(d => d.Id == detalleId);
            if (detalle != null)
            {
                Detalles.Remove(detalle);
                FechaModificacion = DateTime.Now;
            }
        }

        public void LimpiarDetalles()
        {
            Detalles.Clear();
            FechaModificacion = DateTime.Now;
        }

        // Gestión de estado
        public void Activar()
        {
            Activo = true;
            FechaModificacion = DateTime.Now;
        }

        public void Desactivar()
        {
            Activo = false;
            FechaModificacion = DateTime.Now;
        }

        /// <summary>
        /// Obtiene el precio de un producto en esta lista
        /// </summary>
        public decimal? ObtenerPrecioProducto(Guid idProducto)
        {
            var detalle = Detalles.FirstOrDefault(d => d.IdProducto == idProducto);
            return detalle?.Precio;
        }

        // Validaciones de negocio
        private void ValidarYEstablecerCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("El código es obligatorio.", nameof(codigo));

            if (codigo.Length != 2)
                throw new ArgumentException("El código debe tener exactamente 2 caracteres.", nameof(codigo));

            if (!codigo.All(char.IsDigit))
                throw new ArgumentException("El código solo puede contener dígitos.", nameof(codigo));

            Codigo = codigo.Trim();
        }

        private void ValidarYEstablecerNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));

            if (nombre.Length < 3)
                throw new ArgumentException("El nombre debe tener al menos 3 caracteres.", nameof(nombre));

            if (nombre.Length > 100)
                throw new ArgumentException("El nombre no puede exceder los 100 caracteres.", nameof(nombre));

            Nombre = nombre.Trim();
        }

        public void ValidarNegocio()
        {
            ValidarYEstablecerCodigo(Codigo);
            ValidarYEstablecerNombre(Nombre);

            foreach (var detalle in Detalles)
            {
                detalle.ValidarNegocio();
            }
        }
    }

    /// <summary>
    /// Entidad de dominio ListaPrecioDetalle - Representa un precio de producto en una lista
    /// </summary>
    public class ListaPrecioDetalleDM
    {
        public Guid Id { get; private set; }
        public Guid IdListaPrecio { get; private set; }
        public Guid IdProducto { get; private set; }
        public decimal Precio { get; private set; }

        // Constructor para creación inicial
        public ListaPrecioDetalleDM(Guid idProducto, decimal precio)
        {
            Id = Guid.NewGuid();

            ValidarYEstablecerProducto(idProducto);
            ValidarYEstablecerPrecio(precio);
        }

        // Constructor para cargar desde base de datos
        public ListaPrecioDetalleDM(
            Guid id,
            Guid idListaPrecio,
            Guid idProducto,
            decimal precio)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("El ID no puede ser vacío.", nameof(id));

            Id = id;
            IdListaPrecio = idListaPrecio;
            IdProducto = idProducto;
            Precio = precio;
        }

        // Métodos internos para la entidad padre
        internal void EstablecerListaPrecio(Guid idListaPrecio)
        {
            if (idListaPrecio == Guid.Empty)
                throw new ArgumentException("El ID de lista de precio no puede ser vacío.", nameof(idListaPrecio));

            IdListaPrecio = idListaPrecio;
        }

        // Actualización
        public void ActualizarPrecio(decimal precio)
        {
            ValidarYEstablecerPrecio(precio);
        }

        // Validaciones de negocio
        private void ValidarYEstablecerProducto(Guid idProducto)
        {
            if (idProducto == Guid.Empty)
                throw new ArgumentException("Debe seleccionar un producto.", nameof(idProducto));

            IdProducto = idProducto;
        }

        private void ValidarYEstablecerPrecio(decimal precio)
        {
            if (precio < 0)
                throw new ArgumentException("El precio no puede ser negativo.", nameof(precio));

            if (precio > 999999999)
                throw new ArgumentException("El precio no puede exceder 999,999,999.", nameof(precio));

            Precio = precio;
        }

        public void ValidarNegocio()
        {
            ValidarYEstablecerProducto(IdProducto);
            ValidarYEstablecerPrecio(Precio);
        }
    }
}
