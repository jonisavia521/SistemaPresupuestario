using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainModel.Domain
{
    /// <summary>
    /// Entidad de dominio que representa una lista de precios con sus detalles.
    /// 
    /// Una lista de precios permite ofrecer precios diferenciados según cliente
    /// o segmento. Contiene una coleccción de precios para diferentes productos.
    /// 
    /// Responsabilidades:
    /// - Validar código y nombre únicos
    /// - Gestionar detalles de precios por producto
    /// - Gestionar activación/desactivación
    /// - Auditar creación y modificación
    /// 
    /// Invariantes:
    /// - Código exactamente 2 dígitos
    /// - Nombre entre 3 y 100 caracteres
    /// - No puede haber duplicación de productos en detalles
    /// 
    /// Estados:
    /// - Activa: Disponible para asignar a clientes
    /// - Inactiva: No disponible en listas de selección
    /// 
    /// </summary>
    public class ListaPrecioDM
    {
        /// <summary>Identificador único de la lista de precios</summary>
        public Guid Id { get; private set; }
        
        /// <summary>Código único de la lista. Exactamente 2 dígitos numéricos.</summary>
        public string Codigo { get; private set; }
        
        /// <summary>Nombre descriptivo de la lista de precios. Entre 3 y 100 caracteres.</summary>
        public string Nombre { get; private set; }
        
        /// <summary>Indica si la lista de precios está activa en el sistema</summary>
        public bool Activo { get; private set; }
        
        /// <summary>Indica si los precios de esta lista incluyen IVA o son netos</summary>
        public bool IncluyeIva { get; private set; }
        
        /// <summary>Fecha y hora de creación de la lista</summary>
        public DateTime FechaAlta { get; private set; }
        
        /// <summary>Fecha y hora de la última modificación. Nula si nunca fue modificada.</summary>
        public DateTime? FechaModificacion { get; private set; }
        
        /// <summary>Colección de precios por producto en esta lista</summary>
        public List<ListaPrecioDetalleDM> Detalles { get; private set; }

        /// <summary>
        /// Crea una nueva instancia de lista de precios.
        /// 
        /// Se validan automáticamente código y nombre. Se crea activa
        /// sin detalles (se agregan posteriormente).
        /// </summary>
        /// <param name="codigo">Código único de 2 dígitos numéricos</param>
        /// <param name="nombre">Nombre descriptivo (3-100 caracteres)</param>
        /// <param name="incluyeIva">Si los precios incluyen IVA (predeterminado: false)</param>
        /// <exception cref="ArgumentException">Si los parámetros no cumplen validaciones</exception>
        /// <exception cref="ArgumentNullException">Si un parámetro requerido es nulo</exception>
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

        /// <summary>
        /// Constructor para hidratar una lista de precios desde la base de datos.
        /// </summary>
        /// <param name="id">GUID único de la lista</param>
        /// <param name="codigo">Código de la lista</param>
        /// <param name="nombre">Nombre de la lista</param>
        /// <param name="activo">Estado de activación</param>
        /// <param name="fechaAlta">Fecha de creación</param>
        /// <param name="fechaModificacion">Fecha de última modificación (puede ser nula)</param>
        /// <param name="detalles">Precios por producto (puede ser nula)</param>
        /// <param name="incluyeIva">Si los precios incluyen IVA</param>
        /// <exception cref="ArgumentException">Si el ID es un GUID vacío</exception>
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

        /// <summary>
        /// Actualiza nombre e indicador de inclusión de IVA de la lista.
        /// 
        /// No modifica el código (es inmutable).
        /// </summary>
        /// <param name="nombre">Nuevo nombre de la lista (3-100 caracteres)</param>
        /// <param name="incluyeIva">Si los precios incluyen IVA</param>
        /// <exception cref="ArgumentException">Si el nombre no cumple validaciones</exception>
        public void ActualizarDatos(string nombre, bool incluyeIva)
        {
            ValidarYEstablecerNombre(nombre);
            IncluyeIva = incluyeIva;
            FechaModificacion = DateTime.Now;
        }

        /// <summary>
        /// Agrega un nuevo precio de producto a la lista.
        /// 
        /// Si el producto ya existe en la lista, actualiza su precio.
        /// Si es nuevo, lo agrega a la colección.
        /// </summary>
        /// <param name="detalle">Detalle de precio a agregar</param>
        /// <exception cref="ArgumentNullException">Si el detalle es nulo</exception>
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

        /// <summary>
        /// Remueve un precio de producto de la lista.
        /// </summary>
        /// <param name="detalleId">GUID del detalle a remover</param>
        public void RemoverDetalle(Guid detalleId)
        {
            var detalle = Detalles.FirstOrDefault(d => d.Id == detalleId);
            if (detalle != null)
            {
                Detalles.Remove(detalle);
                FechaModificacion = DateTime.Now;
            }
        }

        /// <summary>
        /// Elimina todos los precios de la lista.
        /// 
        /// Deja la lista vacía de detalles.
        /// </summary>
        public void LimpiarDetalles()
        {
            Detalles.Clear();
            FechaModificacion = DateTime.Now;
        }

        /// <summary>
        /// Activa la lista de precios.
        /// 
        /// Permite que vuelva a aparecer en listas de selección.
        /// </summary>
        public void Activar()
        {
            Activo = true;
            FechaModificacion = DateTime.Now;
        }

        /// <summary>
        /// Desactiva la lista de precios.
        /// 
        /// Los clientes no podrán usar esta lista para nuevos presupuestos,
        /// pero los presupuestos existentes mantienen sus precios.
        /// </summary>
        public void Desactivar()
        {
            Activo = false;
            FechaModificacion = DateTime.Now;
        }

        /// <summary>
        /// Obtiene el precio de un producto específico en esta lista.
        /// 
        /// Busca entre los detalles el precio del producto solicitado.
        /// </summary>
        /// <param name="idProducto">GUID del producto a buscar</param>
        /// <returns>El precio del producto si existe en la lista, nulo si no</returns>
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
