using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// Objeto de Transferencia de Datos (DTO) para Lista de Precios.
    /// 
    /// Una lista de precios es un conjunto de precios asignados a productos específicos.
    /// Las listas permiten tener diferentes estrategias de precios para distintos clientes,
    /// segmentos de mercado o períodos específicos.
    /// 
    /// Cuando se crea una cotización para un cliente que tiene una lista de precios asignada,
    /// los precios de los productos se cargan automáticamente de esa lista.
    /// 
    /// Cada lista contiene múltiples detalles, siendo cada detalle un producto con su precio específico.
    /// </summary>
    public class ListaPrecioDTO
    {
        /// <summary>
        /// Identificador único de la lista de precios (GUID).
        /// Se genera automáticamente al crear la lista y no es modificable.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Código interno asignado a la lista de precios para identificación rápida.
        /// Formato: Exactamente 2 dígitos (ej: "01", "02", "03").
        /// Debe ser único dentro del sistema.
        /// </summary>
        [Required(ErrorMessage = "El código es obligatorio")]
        [RegularExpression(@"^\d{2}$", ErrorMessage = "El código debe tener exactamente 2 dígitos")]
        public string Codigo { get; set; }

        /// <summary>
        /// Nombre descriptivo de la lista de precios.
        /// Identifica el propósito o aplicación de la lista (ej: "Cliente Especial", "Distribuidores", etc.).
        /// Se utiliza en listas desplegables y reportes.
        /// </summary>
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Nombre { get; set; }

        /// <summary>
        /// Indicador del estado de la lista de precios.
        /// true = lista activa (disponible para asignar a clientes)
        /// false = lista desactivada (no se puede utilizar, se mantiene en histórico)
        /// </summary>
        public bool Activo { get; set; }

        /// <summary>
        /// Indicador de si los precios de la lista incluyen IVA.
        /// true = los precios ya incluyen el IVA
        /// false = los precios son netos, se debe agregar IVA al calcular
        /// Se utiliza para cálculos correctos en cotizaciones y facturas.
        /// </summary>
        public bool IncluyeIva { get; set; }

        /// <summary>
        /// Fecha y hora en que se creó la lista de precios.
        /// Se establece automáticamente al crear la lista y no es modificable.
        /// </summary>
        public DateTime FechaAlta { get; set; }

        /// <summary>
        /// Fecha y hora de la última modificación de la lista de precios.
        /// Se actualiza automáticamente cada vez que se modifica la lista.
        /// Nullable porque listas recién creadas no tienen modificaciones.
        /// </summary>
        public DateTime? FechaModificacion { get; set; }

        /// <summary>
        /// Colección de detalles (productos con sus precios) que integran la lista.
        /// Cada detalle asocia un producto específico con su precio en esta lista.
        /// Una lista de precios debe tener al menos un detalle para ser válida.
        /// Se inicializa como lista vacía por defecto en el constructor.
        /// </summary>
        public List<ListaPrecioDetalleDTO> Detalles { get; set; }

        /// <summary>
        /// Propiedad calculada que retorna el estado de la lista en formato legible.
        /// Retorna "Activo" si Activo es true, "Inactivo" si es false.
        /// Se utiliza en listados de la UI para mostrar el estado de manera clara.
        /// </summary>
        public string EstadoTexto => Activo ? "Activo" : "Inactivo";

        /// <summary>
        /// Constructor por defecto del ListaPrecioDTO.
        /// Inicializa valores por defecto y la colección de detalles.
        /// </summary>
        public ListaPrecioDTO()
        {
            Activo = true;
            IncluyeIva = false;
            FechaAlta = DateTime.Now;
            Detalles = new List<ListaPrecioDetalleDTO>();
        }
    }

    /// <summary>
    /// Objeto de Transferencia de Datos (DTO) para Detalle de Lista de Precios.
    /// 
    /// Representa la asociación entre un producto y su precio específico dentro
    /// de una lista de precios. Un detalle define el precio que tendrá un producto
    /// cuando se utiliza esta lista en una cotización.
    /// </summary>
    public class ListaPrecioDetalleDTO
    {
        /// <summary>
        /// Identificador único del detalle (GUID).
        /// Se genera automáticamente al crear el detalle y no es modificable.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador (GUID) de la lista de precios padre a la cual pertenece este detalle.
        /// Esta es una clave foránea a la entidad ListaPrecio.
        /// </summary>
        public Guid IdListaPrecio { get; set; }

        /// <summary>
        /// Identificador (GUID) del producto cuyo precio se define en este detalle.
        /// Esta es una clave foránea obligatoria a la entidad Producto.
        /// </summary>
        [Required(ErrorMessage = "Debe seleccionar un producto")]
        public Guid IdProducto { get; set; }

        /// <summary>
        /// Precio específico del producto en esta lista de precios.
        /// Rango permitido: 0 a 999,999,999
        /// Puede ser cero si el producto es un obsequio o tiene precio especial.
        /// El formato (si incluye IVA o no) se define en la propiedad IncluyeIva de la lista padre.
        /// </summary>
        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0, 999999999, ErrorMessage = "El precio debe estar entre 0 y 999,999,999")]
        public decimal Precio { get; set; }

        /// <summary>
        /// Propiedad auxiliar que contiene el código del producto.
        /// Se utiliza en la UI para mostrar el producto sin requerer carga adicional.
        /// No se persiste en la base de datos, es solo para presentación.
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Propiedad auxiliar que contiene la descripción del producto.
        /// Se utiliza en la UI para mostrar la descripción del producto.
        /// No se persiste en la base de datos, es solo para presentación.
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Constructor por defecto del ListaPrecioDetalleDTO.
        /// Inicializa el precio con valor cero.
        /// </summary>
        public ListaPrecioDetalleDTO()
        {
            Precio = 0M;
        }
    }
}
