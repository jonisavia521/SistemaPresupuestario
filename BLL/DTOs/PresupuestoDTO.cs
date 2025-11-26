using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs
{
    /// <summary>
    /// Objeto de Transferencia de Datos (DTO) para Presupuesto.
    /// 
    /// Esta clase actúa como intermediaria para transferir información de presupuestos
    /// (también llamados cotizaciones) entre la capa de Presentación (UI) y la capa de Lógica de Negocio (BLL).
    /// 
    /// Un presupuesto es una propuesta de venta detallada que se presenta a un cliente,
    /// especificando los productos o servicios ofrecidos, cantidades, precios y condiciones.
    /// Transita por varios estados hasta su conversión en factura.
    /// 
    /// ESTADOS DEL PRESUPUESTO:
    /// - 1 = Borrado: Eliminado lógicamente
    /// - 2 = Emitido: Recién creado, disponible para modificaciones
    /// - 3 = Aprobado: Aceptado por el cliente
    /// - 4 = Rechazado: Rechazado por el cliente
    /// - 5 = Vencido: Cuya fecha de validez expiró
    /// - 6 = Facturado: Convertido en factura legal
    /// 
    /// Contiene atributos de validación DataAnnotations para realizar validaciones
    /// de entrada de datos en tiempo de enlace de modelos.
    /// </summary>
    public class PresupuestoDTO
    {
        /// <summary>
        /// Identificador único del presupuesto (GUID).
        /// Se genera automáticamente al crear el presupuesto y no es modificable.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Número secuencial único que identifica el presupuesto.
        /// Es el identificador "amigable" utilizado en documentos y comunicaciones con el cliente.
        /// Formato típico: Números secuenciales (ej: "00001", "00002").
        /// </summary>
        [Required(ErrorMessage = "El número de presupuesto es obligatorio")]
        [MaxLength(50, ErrorMessage = "El número no puede exceder los 50 caracteres")]
        public string Numero { get; set; }

        /// <summary>
        /// Identificador (GUID) del cliente para el cual se emitió el presupuesto.
        /// Esta es una clave foránea obligatoria a la entidad Cliente.
        /// </summary>
        [Required(ErrorMessage = "Debe seleccionar un cliente")]
        public Guid IdCliente { get; set; }

        /// <summary>
        /// Fecha y hora en que se emitió el presupuesto.
        /// Se establece automáticamente al crear el presupuesto y no es modificable.
        /// Se utiliza para cálculos de vencimiento y reportes.
        /// </summary>
        [Required(ErrorMessage = "La fecha de emisión es obligatoria")]
        public DateTime FechaEmision { get; set; }

        /// <summary>
        /// Estado actual del presupuesto (1-6).
        /// Define en qué fase del ciclo de vida se encuentra el presupuesto.
        /// Consultar documento ESTADOS DEL PRESUPUESTO más arriba.
        /// </summary>
        [Required(ErrorMessage = "El estado es obligatorio")]
        [Range(0, 6, ErrorMessage = "Estado inválido")]
        public int Estado { get; set; }

        /// <summary>
        /// Fecha y hora de vencimiento de la validez del presupuesto.
        /// Después de esta fecha, el presupuesto se considera implícitamente vencido.
        /// Nullable porque algunos presupuestos pueden no tener vencimiento específico.
        /// </summary>
        public DateTime? FechaVencimiento { get; set; }

        /// <summary>
        /// Identificador del presupuesto padre en caso de que este sea una copia.
        /// Permite rastrear la linaje de presupuestos copiados/modificados.
        /// Nullable porque no todos los presupuestos son copias.
        /// </summary>
        public Guid? IdPresupuestoPadre { get; set; }

        /// <summary>
        /// Identificador (GUID) del vendedor que creó el presupuesto.
        /// Esta es una clave foránea a la entidad Vendedor.
        /// Nullable porque un presupuesto podría no tener vendedor asignado.
        /// </summary>
        public Guid? IdVendedor { get; set; }

        /// <summary>
        /// Identificador (GUID) de la lista de precios aplicada al presupuesto.
        /// Define qué precios se utilizan para los productos en esta cotización.
        /// Nullable porque un presupuesto podría utilizar precios manuales.
        /// </summary>
        public Guid? IdListaPrecio { get; set; }

        /// <summary>
        /// Propiedad auxiliar que contiene el código del cliente.
        /// Se utiliza en la UI para mostrar información del cliente sin requerer carga adicional.
        /// No se persiste en la base de datos, es solo para presentación.
        /// </summary>
        public string ClienteCodigoCliente { get; set; }

        /// <summary>
        /// Propiedad auxiliar que contiene la razón social del cliente.
        /// Se utiliza en la UI para mostrar el nombre del cliente.
        /// No se persiste en la base de datos, es solo para presentación.
        /// </summary>
        public string ClienteRazonSocial { get; set; }

        /// <summary>
        /// Propiedad auxiliar que contiene el nombre del vendedor.
        /// Se utiliza en la UI para mostrar el vendedor sin requerer carga adicional.
        /// No se persiste en la base de datos, es solo para presentación.
        /// </summary>
        public string VendedorNombre { get; set; }

        /// <summary>
        /// Propiedad auxiliar que contiene la descripción del estado.
        /// Se utiliza en la UI para mostrar el estado de forma legible.
        /// No se persiste en la base de datos, es solo para presentación.
        /// </summary>
        public string EstadoDescripcion { get; set; }

        /// <summary>
        /// Propiedad auxiliar que contiene el código de la lista de precios.
        /// Se utiliza en la UI para mostrar la lista sin requerer carga adicional.
        /// No se persiste en la base de datos, es solo para presentación.
        /// </summary>
        public string ListaPrecioCodigo { get; set; }

        /// <summary>
        /// Propiedad auxiliar que contiene el nombre de la lista de precios.
        /// Se utiliza en la UI para mostrar la lista.
        /// No se persiste en la base de datos, es solo para presentación.
        /// </summary>
        public string ListaPrecioNombre { get; set; }

        /// <summary>
        /// Propiedad calculada que retorna el texto del estado en formato legible.
        /// Interpreta el código de estado numérico en su descripción textual:
        /// - 1 = "Borrado"
        /// - 2 = "Emitido"
        /// - 3 = "Aprobado"
        /// - 4 = "Rechazado"
        /// - 5 = "Vencido"
        /// - 6 = "Facturado"
        /// - Otros = "Desconocido"
        /// Se utiliza en grillas y listados de la UI.
        /// </summary>
        public string EstadoTexto
        {
            get
            {
                switch (Estado)
                {
                    case 1: return "Borrado";
                    case 2: return "Emitido";
                    case 3: return "Aprobado";
                    case 4: return "Rechazado";
                    case 5: return "Vencido";
                    case 6: return "Facturado";
                    default: return "Desconocido";
                }
            }
        }

        /// <summary>
        /// Colección de detalles (líneas de artículos) que integran el presupuesto.
        /// Cada detalle representa un producto/servicio con cantidad, precio y descuentos.
        /// Un presupuesto debe tener al menos un detalle para ser válido.
        /// Se inicializa como lista vacía por defecto.
        /// </summary>
        public List<PresupuestoDetalleDTO> Detalles { get; set; } = new List<PresupuestoDetalleDTO>();

        /// <summary>
        /// Suma del monto neto de todos los detalles sin considerar IVA.
        /// Se calcula automáticamente como sum(detalle.Total para cada detalle).
        /// </summary>
        public decimal Subtotal { get; set; }

        /// <summary>
        /// Suma del IVA calculado sobre todos los detalles.
        /// Se calcula automáticamente considerando el porcentaje de IVA de cada producto.
        /// </summary>
        public decimal TotalIva { get; set; }

        /// <summary>
        /// Monto total del presupuesto incluyendo IVA.
        /// Equivale a: Subtotal + TotalIva
        /// Este es el monto final a cobrar al cliente.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Importe de percepción IIBB ARBA (Impuesto de Ingresos Brutos).
        /// Se calcula como: Total * AlicuotaArbaCliente / 100
        /// Solo se aplica a clientes con tipo de IVA "RESPONSABLE_INSCRIPTO".
        /// Se descuenta del pago al cliente (es un anticipo de impuesto).
        /// </summary>
        public decimal ImporteArba { get; set; }

        /// <summary>
        /// Monto bruto total antes de descuentos y sin IVA.
        /// Se utiliza para compatibilidad con cálculos en el formulario.
        /// </summary>
        public decimal TotalBruto { get; set; }

        /// <summary>
        /// Suma total de descuentos aplicados a todos los detalles.
        /// Se utiliza para compatibilidad con cálculos en el formulario.
        /// </summary>
        public decimal TotalDescuentos { get; set; }

        /// <summary>
        /// Monto neto total después de descuentos pero antes de IVA.
        /// Se utiliza para compatibilidad con cálculos en el formulario.
        /// </summary>
        public decimal TotalNeto { get; set; }

        /// <summary>
        /// Monto final a cobrar, incluyendo IVA, descuentos y deducciones.
        /// Este es el monto total que aparecerá en la factura.
        /// Se utiliza para compatibilidad con cálculos en el formulario.
        /// </summary>
        public decimal TotalFinal { get; set; }

        /// <summary>
        /// Constructor por defecto del PresupuestoDTO.
        /// Inicializa la colección de detalles como una lista vacía.
        /// </summary>
        public PresupuestoDTO()
        {
        }
    }

    /// <summary>
    /// Objeto de Transferencia de Datos (DTO) para Detalle de Presupuesto.
    /// 
    /// Representa una línea individual dentro de un presupuesto, incluyendo
    /// información del producto, cantidad, precio unitario, descuentos e impuestos.
    /// 
    /// Un presupuesto está compuesto por uno o más detalles. Cada detalle es una
    /// ocurrencia específica de un producto con sus términos de venta en el presupuesto.
    /// 
    /// Contiene atributos de validación DataAnnotations y propiedades calculadas
    /// para realizar cálculos de montos durante edición en el formulario.
    /// </summary>
    public class PresupuestoDetalleDTO
    {
        /// <summary>
        /// Identificador único del detalle (GUID).
        /// Se genera automáticamente al crear el detalle y no es modificable.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Número del presupuesto padre al cual pertenece este detalle.
        /// Se utiliza para mostrar información del presupuesto.
        /// </summary>
        public string Numero { get; set; }

        /// <summary>
        /// Identificador (GUID) del presupuesto al cual pertenece este detalle.
        /// Esta es una clave foránea a la entidad Presupuesto.
        /// Nullable porque un detalle podría crearse antes de ser asignado a un presupuesto.
        /// </summary>
        public Guid? IdPresupuesto { get; set; }

        /// <summary>
        /// Identificador (GUID) del producto en este detalle.
        /// Esta es una clave foránea obligatoria a la entidad Producto.
        /// </summary>
        [Required(ErrorMessage = "Debe seleccionar un producto")]
        public Guid? IdProducto { get; set; }

        /// <summary>
        /// Cantidad de unidades del producto en este detalle.
        /// Valor por defecto: 1
        /// Rango permitido: 0.01 a 999,999
        /// </summary>
        private decimal _cantidad = 1M;
        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(0.01, 999999, ErrorMessage = "La cantidad debe estar entre 0.01 y 999,999")]
        public decimal Cantidad 
        { 
            get => _cantidad;
            set => _cantidad = value;
        }

        /// <summary>
        /// Precio unitario del producto en este detalle.
        /// Valor por defecto: 0
        /// Rango permitido: 0 a 999,999,999
        /// Se puede establecer manualmente o cargarse de la lista de precios.
        /// </summary>
        private decimal _precio = 0M;
        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0, 999999999, ErrorMessage = "El precio debe estar entre 0 y 999,999,999")]
        public decimal Precio 
        { 
            get => _precio;
            set => _precio = value;
        }

        /// <summary>
        /// Porcentaje de descuento aplicado a este detalle.
        /// Valor por defecto: 0
        /// Rango permitido: 0 a 100 (representando 0% a 100%)
        /// Se aplica sobre el subtotal para calcular el monto neto.
        /// </summary>
        private decimal _descuento = 0M;
        [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100")]
        public decimal Descuento 
        { 
            get => _descuento;
            set => _descuento = value;
        }

        /// <summary>
        /// Número de renglón o línea del detalle dentro del presupuesto.
        /// Se utiliza para ordenamiento y referencia en documentos impresos.
        /// </summary>
        public int Renglon { get; set; }

        /// <summary>
        /// Porcentaje de Impuesto al Valor Agregado (IVA) aplicable a este detalle.
        /// Valor por defecto: 21 (IVA general)
        /// Rango permitido: 0 a 100
        /// Se obtiene automáticamente del porcentaje IVA del producto.
        /// </summary>
        private decimal _porcentajeIVA = 21M;
        [Range(0, 100, ErrorMessage = "El porcentaje de IVA debe estar entre 0 y 100")]
        public decimal PorcentajeIVA 
        { 
            get => _porcentajeIVA;
            set => _porcentajeIVA = value;
        }

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
        /// Monto neto total del detalle después de aplicar cantidad y descuentos, sin IVA.
        /// IMPORTANTE: Esta propiedad es persistible. Se carga desde la BD y NO se recalcula automáticamente.
        /// El recálculo debe hacerse explícitamente llamando a RecalcularTotal().
        /// </summary>
        private decimal _total = 0M;
        public decimal Total 
        { 
            get => _total;
            set => _total = value;
        }

        /// <summary>
        /// Propiedad calculada que retorna el subtotal (cantidad * precio).
        /// Se calcula dinámicamente, no se persiste.
        /// Subtotal = Cantidad * Precio
        /// </summary>
        public decimal Subtotal => Cantidad * Precio;

        /// <summary>
        /// Propiedad calculada que retorna el monto del descuento en la moneda del presupuesto.
        /// Se calcula dinámicamente, no se persiste.
        /// DescuentoMonto = Subtotal * (Descuento / 100)
        /// </summary>
        public decimal DescuentoMonto => Subtotal * (Descuento / 100);

        /// <summary>
        /// Propiedad calculada que retorna el monto de IVA del detalle.
        /// Se calcula dinámicamente, no se persiste.
        /// Iva = Total * (PorcentajeIVA / 100)
        /// </summary>
        public decimal Iva => Total * (PorcentajeIVA / 100);

        /// <summary>
        /// Propiedad calculada que retorna el monto total incluyendo IVA.
        /// Se calcula dinámicamente, no se persiste.
        /// TotalConIva = Total + Iva
        /// </summary>
        public decimal TotalConIva => Total + Iva;

        /// <summary>
        /// Alias para la propiedad Precio. Se utiliza para compatibilidad
        /// con cálculos en el formulario que usan este nombre.
        /// </summary>
        public decimal PrecioUnitario
        {
            get => Precio;
            set => Precio = value;
        }

        /// <summary>
        /// Alias para TotalConIva. Se utiliza para compatibilidad con
        /// cálculos en el formulario que necesitan mostrar el importe total.
        /// </summary>
        public decimal ImporteTotal => TotalConIva;

        /// <summary>
        /// Alias para Total. Se utiliza para compatibilidad con
        /// cálculos en el formulario que necesitan el importe neto sin IVA.
        /// </summary>
        public decimal ImporteNeto => Total;

        /// <summary>
        /// Alias para Subtotal. Se utiliza para compatibilidad con
        /// cálculos en el formulario que necesitan el monto bruto (cantidad * precio).
        /// </summary>
        public decimal ImporteBruto => Subtotal;

        /// <summary>
        /// Método público para recalcular manualmente el total del detalle.
        /// 
        /// IMPORTANTE: SOLO debe ser llamado desde el formulario cuando el usuario
        /// edita una celda de cantidad, precio o descuento. No se recalcula automáticamente
        /// para evitar inconsistencias con datos persistidos en BD.
        /// 
        /// Fórmula de cálculo:
        /// 1. Calcular subtotal: Cantidad * Precio
        /// 2. Calcular descuento: Subtotal * (Descuento / 100)
        /// 3. Calcular total: Subtotal - DescuentoMonto
        /// 
        /// Después de llamar a este método, se debe recalcular el presupuesto padre
        /// para obtener nuevos totales agregados (subtotal, IVA, total).
        /// </summary>
        public void RecalcularTotal()
        {
            decimal subtotal = Cantidad * Precio;
            decimal descuentoMonto = subtotal * (Descuento / 100);
            _total = subtotal - descuentoMonto;
        }

        /// <summary>
        /// Constructor por defecto del PresupuestoDetalleDTO.
        /// Inicializa propiedades de texto vacías.
        /// </summary>
        public PresupuestoDetalleDTO()
        {
            // Valores por defecto
            Codigo = string.Empty;
            Descripcion = string.Empty;
        }
    }
}
