using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainModel.Domain
{
    /// <summary>
    /// Entidad de dominio que representa un presupuesto o cotización comercial.
    /// 
    /// Esta es una de las entidades principales del sistema. Encapsula toda la lógica
    /// de negocio de cotizaciones, incluyendo validaciones de estado, gestión de detalles
    /// y cálculos de totales. Sigue el ciclo de vida completo desde creación hasta facturación.
    /// 
    /// Responsabilidades:
    /// - Gestionar ciclo de vida de estados del presupuesto
    /// - Validar transiciones de estado permitidas
    /// - Gestionar líneas de detalle (productos)
    /// - Calcular y persistir totales
    /// - Determinar vencimiento
    /// - Auditar creación y modificación
    /// 
    /// Estados y Transiciones:
    /// - 1 (Borrado): Estado final, no puede cambiar. Presupuesto eliminado lógicamente.
    /// - 2 (Emitido): Estado inicial. Puede cambiar a Aprobado, Rechazado o Borrado.
    /// - 3 (Aprobado): Listo para facturar. Solo puede cambiar a Facturado.
    /// - 4 (Rechazado): Estado final, no puede cambiar. Cliente rechazó la propuesta.
    /// - 5 (Vencido): Estado implícito si Emitido o Aprobado con fecha vencida.
    /// - 6 (Facturado): Estado final, no puede cambiar. Genera obligación tributaria.
    /// 
    /// Invariantes:
    /// - Debe tener al menos un detalle para ser emitido
    /// - Totales no deben ser negativos
    /// - Fecha vencimiento no puede ser anterior a emisión
    /// - Un presupuesto aprobado solo puede ir a facturado
    /// 
    /// </summary>
    public class PresupuestoDM
    {
        /// <summary>Identificador único del presupuesto en el sistema</summary>
        public Guid Id { get; private set; }
        
        /// <summary>Número único del presupuesto para referencia comercial. Máximo 50 caracteres.</summary>
        public string Numero { get; private set; }
        
        /// <summary>GUID del cliente al cual se dirige el presupuesto</summary>
        public Guid IdCliente { get; private set; }
        
        /// <summary>Fecha y hora de emisión del presupuesto</summary>
        public DateTime FechaEmision { get; private set; }
        
        /// <summary>Estado actual del presupuesto (1-6): Borrado, Emitido, Aprobado, Rechazado, Vencido, Facturado</summary>
        public int Estado { get; private set; }
        
        /// <summary>Fecha hasta la cual el presupuesto es válido (opcional). Después se considera vencido.</summary>
        public DateTime? FechaVencimiento { get; private set; }
        
        /// <summary>GUID del presupuesto padre si este fue copiado de otro. Para trazabilidad.</summary>
        public Guid? IdPresupuestoPadre { get; private set; }
        
        /// <summary>GUID del vendedor responsable del presupuesto (opcional). Usado para comisiones.</summary>
        public Guid? IdVendedor { get; private set; }
        
        /// <summary>Colección de líneas de detalle del presupuesto (productos cotizados)</summary>
        public List<PresupuestoDetalleDM> Detalles { get; private set; }

        /// <summary>Suma de totales de cada detalle sin IVA (neto)</summary>
        public decimal Subtotal { get; private set; }
        
        /// <summary>Suma total de IVA de todos los detalles</summary>
        public decimal TotalIva { get; private set; }
        
        /// <summary>Subtotal + IVA + ARBA (monto total a pagar)</summary>
        public decimal Total { get; private set; }
        
        /// <summary>Importe retenido por IIBB ARBA (Buenos Aires) si aplica</summary>
        public decimal ImporteArba { get; private set; }

        /// <summary>
        /// Crea un nuevo presupuesto en estado Emitido.
        /// 
        /// Se validan automáticamente número de cliente y fecha de vencimiento.
        /// Se inicializan los totales en cero, deben completarse posteriormente
        /// con el método EstablecerTotales() después de agregar detalles.
        /// </summary>
        /// <param name="numero">Número único del presupuesto (1-50 caracteres)</param>
        /// <param name="idCliente">GUID del cliente (no puede ser vacío)</param>
        /// <param name="fechaEmision">Fecha de emisión del presupuesto</param>
        /// <param name="fechaVencimiento">Fecha hasta la cual es válido (opcional, no puede ser anterior a emisión)</param>
        /// <param name="idVendedor">GUID del vendedor asignado (opcional)</param>
        /// <param name="idPresupuestoPadre">GUID del presupuesto original si fue copiado (opcional)</param>
        /// <exception cref="ArgumentException">Si número está vacío/nulo o cliente es GUID vacío</exception>
        /// <remarks>
        /// El presupuesto se crea siempre en estado 2 (Emitido).
        /// Debe agregarse al menos un detalle antes de poder emitirlo formalmente.
        /// </remarks>
        // Constructor para creación inicial (nuevo presupuesto)
        public PresupuestoDM(
            string numero,
            Guid idCliente,
            DateTime fechaEmision,
            DateTime? fechaVencimiento,
            Guid? idVendedor,
            Guid? idPresupuestoPadre = null)
        {
            Id = Guid.NewGuid();
            Estado = 2; // Emitido por defecto
            Detalles = new List<PresupuestoDetalleDM>();

            ValidarYEstablecerNumero(numero);
            ValidarYEstablecerCliente(idCliente);
            FechaEmision = fechaEmision;
            ValidarYEstablecerFechaVencimiento(fechaVencimiento);
            IdVendedor = idVendedor;
            IdPresupuestoPadre = idPresupuestoPadre;

            // Inicializar totales en 0
            Subtotal = 0;
            TotalIva = 0;
            Total = 0;
            ImporteArba = 0;
        }

        /// <summary>
        /// Constructor para hidratar un presupuesto desde la base de datos.
        /// 
        /// Se utiliza internamente por repositorios para reconstruir objetos
        /// desde registros persistidos, incluyendo sus detalles y totales.
        /// </summary>
        /// <param name="id">GUID único del presupuesto</param>
        /// <param name="numero">Número del presupuesto</param>
        /// <param name="idCliente">GUID del cliente</param>
        /// <param name="fechaEmision">Fecha de emisión</param>
        /// <param name="estado">Estado actual (1-6)</param>
        /// <param name="fechaVencimiento">Fecha de vencimiento (puede ser nula)</param>
        /// <param name="idPresupuestoPadre">GUID del presupuesto padre (puede ser nulo)</param>
        /// <param name="idVendedor">GUID del vendedor (puede ser nulo)</param>
        /// <param name="detalles">Lista de detalles (puede ser nula)</param>
        /// <param name="subtotal">Subtotal calculado</param>
        /// <param name="totalIva">Total IVA calculado</param>
        /// <param name="total">Total final calculado</param>
        /// <param name="importeArba">Importe ARBA aplicable</param>
        /// <exception cref="ArgumentException">Si el ID es un GUID vacío</exception>
        // Constructor para cargar desde base de datos
        public PresupuestoDM(
            Guid id,
            string numero,
            Guid idCliente,
            DateTime fechaEmision,
            int estado,
            DateTime? fechaVencimiento,
            Guid? idPresupuestoPadre,
            Guid? idVendedor,
            List<PresupuestoDetalleDM> detalles = null,
            decimal subtotal = 0,
            decimal totalIva = 0,
            decimal total = 0,
            decimal importeArba = 0)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("El ID del presupuesto no puede ser vacío.", nameof(id));

            Id = id;
            Numero = numero;
            IdCliente = idCliente;
            FechaEmision = fechaEmision;
            Estado = estado;
            FechaVencimiento = fechaVencimiento;
            IdPresupuestoPadre = idPresupuestoPadre;
            IdVendedor = idVendedor;
            Detalles = detalles ?? new List<PresupuestoDetalleDM>();

            Subtotal = subtotal;
            TotalIva = totalIva;
            Total = total;
            ImporteArba = importeArba;
        }

        /// <summary>
        /// Actualiza los datos principales del presupuesto sin cambiar estado ni detalles.
        /// 
        /// Se utiliza para correcciones menores después de la emisión.
        /// Valida cliente y fecha vencimiento según las reglas de negocio.
        /// </summary>
        /// <param name="idCliente">Nuevo cliente del presupuesto</param>
        /// <param name="fechaEmision">Nueva fecha de emisión</param>
        /// <param name="fechaVencimiento">Nueva fecha de vencimiento (puede ser nula)</param>
        /// <param name="idVendedor">Nuevo vendedor (puede ser nulo)</param>
        /// <exception cref="ArgumentException">Si cliente es GUID vacío o fecha vencimiento es anterior a emisión</exception>
        // Método de actualización
        public void ActualizarDatos(
            Guid idCliente,
            DateTime fechaEmision,
            DateTime? fechaVencimiento,
            Guid? idVendedor)
        {
            ValidarYEstablecerCliente(idCliente);
            FechaEmision = fechaEmision;
            ValidarYEstablecerFechaVencimiento(fechaVencimiento);
            IdVendedor = idVendedor;
        }

        /// <summary>
        /// Agrega una línea de detalle (producto) al presupuesto.
        /// 
        /// Solo se permite en estados Emitido (2). El detalle se renumera
        /// automáticamente según su posición en la colección.
        /// </summary>
        /// <param name="detalle">Línea de detalle a agregar (no puede ser nula)</param>
        /// <exception cref="ArgumentNullException">Si el detalle es nulo</exception>
        /// <exception cref="InvalidOperationException">Si el presupuesto está en estado que no permite agregar detalles</exception>
        // Gestión de detalles
        public void AgregarDetalle(PresupuestoDetalleDM detalle)
        {
            if (detalle == null)
                throw new ArgumentNullException(nameof(detalle));

            if (Estado == 3)
                throw new InvalidOperationException("No se pueden agregar detalles a un presupuesto aprobado.");

            if (Estado == 4)
                throw new InvalidOperationException("No se pueden agregar detalles a un presupuesto rechazado.");

            if (Estado == 6)
                throw new InvalidOperationException("No se pueden agregar detalles a un presupuesto facturado.");

            if (Estado == 1)
                throw new InvalidOperationException("No se pueden agregar detalles a un presupuesto borrado.");

            detalle.EstablecerPresupuesto(Id);
            detalle.EstablecerRenglon(Detalles.Count + 1);
            Detalles.Add(detalle);
        }

        /// <summary>
        /// Remueve una línea de detalle del presupuesto y renumera los restantes.
        /// 
        /// Solo se permite en estado Emitido. Reorganiza automáticamente
        /// los números de renglón de los detalles.
        /// </summary>
        /// <param name="detalleId">GUID del detalle a remover</param>
        /// <exception cref="InvalidOperationException">Si el presupuesto no está en estado Emitido</exception>
        public void RemoverDetalle(Guid detalleId)
        {
            if (Estado == 3)
                throw new InvalidOperationException("No se pueden eliminar detalles de un presupuesto aprobado.");

            if (Estado == 4)
                throw new InvalidOperationException("No se pueden eliminar detalles de un presupuesto rechazado.");

            if (Estado == 6)
                throw new InvalidOperationException("No se pueden eliminar detalles de un presupuesto facturado.");

            if (Estado == 1)
                throw new InvalidOperationException("No se pueden eliminar detalles de un presupuesto borrado.");

            var detalle = Detalles.FirstOrDefault(d => d.Id == detalleId);
            if (detalle != null)
            {
                Detalles.Remove(detalle);
                ReordenarRenglones();
            }
        }

        /// <summary>
        /// Elimina todos los detalles del presupuesto.
        /// 
        /// Solo se permite en estado Emitido. Deja el presupuesto vacío de productos.
        /// </summary>
        /// <exception cref="InvalidOperationException">Si el presupuesto está en estado Aprobado o Facturado</exception>
        public void LimpiarDetalles()
        {
            if (Estado == 3)
                throw new InvalidOperationException("No se pueden eliminar detalles de un presupuesto aprobado.");

            if (Estado == 6)
                throw new InvalidOperationException("No se pueden eliminar detalles de un presupuesto facturado.");

            Detalles.Clear();
        }

        /// <summary>
        /// Realiza el reordenamiento de números de renglón tras eliminar un detalle.
        /// 
        /// Se utiliza internamente para mantener secuencia de números de línea.
        /// </summary>
        private void ReordenarRenglones()
        {
            for (int i = 0; i < Detalles.Count; i++)
            {
                Detalles[i].EstablecerRenglon(i + 1);
            }
        }

        /// <summary>
        /// Cambia el estado del presupuesto validando transiciones permitidas.
        /// 
        /// Realiza validaciones para asegurar que la transición es válida según
        /// las reglas de negocio. Algunos estados son terminales y no pueden cambiar.
        /// </summary>
        /// <param name="nuevoEstado">Nuevo estado (1-6)</param>
        /// <exception cref="ArgumentException">Si el estado está fuera de rango (1-6)</exception>
        /// <exception cref="InvalidOperationException">Si la transición de estado no está permitida</exception>
        /// <remarks>
        /// Transiciones NO permitidas:
        /// - Desde Facturado (6) a cualquier otro estado
        /// - Desde Borrado (1) a cualquier otro estado
        /// - Desde Aprobado (3) a cualquier estado excepto Facturado (6)
        /// </remarks>
        // Gestión de estados
        public void CambiarEstado(int nuevoEstado)
        {
            if (nuevoEstado < 1 || nuevoEstado > 6)
                throw new ArgumentException("Estado inválido. Debe ser entre 1 y 6.", nameof(nuevoEstado));

            if (Estado == 3 && nuevoEstado != 3 && nuevoEstado != 6)
                throw new InvalidOperationException("Un presupuesto aprobado solo puede cambiar a estado Facturado.");

            if (Estado == 6)
                throw new InvalidOperationException("Un presupuesto facturado no puede cambiar de estado.");

            if (Estado == 1)
                throw new InvalidOperationException("Un presupuesto borrado no puede cambiar de estado.");

            Estado = nuevoEstado;
        }

        /// <summary>
        /// Verifica que el presupuesto tenga detalles y cambia a estado Emitido (2).
        /// 
        /// Validación de negocio antes de emitir formalmente la cotización.
        /// </summary>
        /// <exception cref="InvalidOperationException">Si el presupuesto no tiene detalles</exception>
        public void Emitir()
        {
            if (!Detalles.Any())
                throw new InvalidOperationException("No se puede emitir un presupuesto sin detalles.");

            CambiarEstado(2);
        }

        /// <summary>
        /// Aprueba el presupuesto, cambiando su estado de Emitido (2) a Aprobado (3).
        /// 
        /// El cliente ha aceptado la propuesta y está listo para facturación.
        /// </summary>
        /// <exception cref="InvalidOperationException">Si el presupuesto no está en estado Emitido</exception>
        public void Aprobar()
        {
            if (Estado != 2)
                throw new InvalidOperationException("Solo se pueden aprobar presupuestos emitidos.");

            CambiarEstado(3);
        }

        /// <summary>
        /// Rechaza el presupuesto, cambiando de Emitido (2) a Rechazado (4).
        /// 
        /// El cliente ha rechazado la propuesta. Este es un estado final.
        /// </summary>
        /// <exception cref="InvalidOperationException">Si el presupuesto no está en estado Emitido</exception>
        public void Rechazar()
        {
            if (Estado != 2)
                throw new InvalidOperationException("Solo se pueden rechazar presupuestos emitidos.");

            CambiarEstado(4);
        }

        /// <summary>
        /// Factura el presupuesto, cambiando de Aprobado (3) a Facturado (6).
        /// 
        /// Esta es la operación final que genera una obligación tributaria.
        /// Solo presupuestos aprobados pueden facturarse.
        /// </summary>
        /// <exception cref="InvalidOperationException">Si el presupuesto no está en estado Aprobado</exception>
        public void Facturar()
        {
            if (Estado != 3)
                throw new InvalidOperationException("Solo se pueden facturar presupuestos aprobados.");

            CambiarEstado(6);
        }

        /// <summary>
        /// Borra lógicamente el presupuesto, cambiando de Emitido (2) a Borrado (1).
        /// 
        /// El registro permanece en BD pero se considera eliminado. Es un estado final.
        /// </summary>
        /// <exception cref="InvalidOperationException">Si el presupuesto no está en estado Emitido</exception>
        public void Borrar()
        {
            if (Estado != 2)
                throw new InvalidOperationException("Solo se pueden borrar presupuestos emitidos.");

            CambiarEstado(1);
        }

        /// <summary>
        /// Determina si el presupuesto está vencido basado en fecha de vencimiento.
        /// 
        /// Solo presupuestos en estado Emitido (2) o Aprobado (3) pueden estar vencidos.
        /// Si no tiene fecha vencimiento, nunca se considera vencido.
        /// </summary>
        /// <returns>true si está vencido, false en caso contrario</returns>
        /// <remarks>
        /// La comparación se realiza por fecha (sin hora) usando DateTime.Now
        /// </remarks>
        public bool EstaVencido()
        {
            if (!FechaVencimiento.HasValue)
                return false;

            if (Estado != 2 && Estado != 3)
                return false;

            return FechaVencimiento.Value.Date < DateTime.Now.Date;
        }

        /// <summary>
        /// Obtiene el estado "efectivo" del presupuesto considerando si está vencido.
        /// 
        /// Si el presupuesto está vencido, retorna 5 (Vencido), caso contrario
        /// retorna el estado normal.
        /// </summary>
        /// <returns>Estado actual considerando vencimiento (1-6, donde 5 es vencido)</returns>
        public int ObtenerEstadoEfectivo()
        {
            if (EstaVencido())
                return 5;

            return Estado;
        }

        /// <summary>
        /// Calcula el subtotal (suma de totales de detalles sin IVA).
        /// 
        /// Función de cálculo que consolida todos los totales netos de detalles.
        /// Se utiliza para validar coherencia de datos.
        /// </summary>
        /// <returns>Suma de totales de detalles (sin IVA)</returns>
        public decimal CalcularSubtotal()
        {
            return Detalles.Sum(d => d.CalcularTotal());
        }

        public decimal CalcularIva()
        {
            return Detalles.Sum(d => d.CalcularIva());
        }

        public decimal CalcularTotal()
        {
            return Detalles.Sum(d => d.CalcularTotalConIva());
        }

        /// <summary>
        /// Establece los totales del presupuesto para persistencia.
        /// 
        /// Este método es invocado desde la capa BLL después de calcular
        /// todos los totales, impuestos y retenciones.
        /// 
        /// Valida que ningún total sea negativo.
        /// </summary>
        /// <param name="subtotal">Suma de totales de detalles (neto)</param>
        /// <param name="totalIva">Suma de IVA de todos los detalles</param>
        /// <param name="total">Monto final a pagar (subtotal + IVA + ARBA)</param>
        /// <param name="importeArba">Retención IIBB ARBA aplicable (predeterminado: 0)</param>
        /// <exception cref="ArgumentException">Si algún total es negativo</exception>
        /// <remarks>
        /// Los totales se persisten en BD para auditoría histórica.
        /// En caso de modificación posterior, estos valores permiten
        /// reconstruir el cálculo original.
        /// </remarks>
        public void EstablecerTotales(decimal subtotal, decimal totalIva, decimal total, decimal importeArba = 0)
        {
            if (subtotal < 0)
                throw new ArgumentException("El subtotal no puede ser negativo.", nameof(subtotal));

            if (totalIva < 0)
                throw new ArgumentException("El total de IVA no puede ser negativo.", nameof(totalIva));

            if (total < 0)
                throw new ArgumentException("El total no puede ser negativo.", nameof(total));

            if (importeArba < 0)
                throw new ArgumentException("El importe ARBA no puede ser negativo.", nameof(importeArba));

            Subtotal = subtotal;
            TotalIva = totalIva;
            Total = total;
            ImporteArba = importeArba;
        }

        // Validaciones de negocio
        private void ValidarYEstablecerNumero(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
                throw new ArgumentException("El número de presupuesto es obligatorio.", nameof(numero));

            if (numero.Length > 50)
                throw new ArgumentException("El número de presupuesto no puede exceder los 50 caracteres.", nameof(numero));

            Numero = numero.Trim().ToUpper();
        }

        private void ValidarYEstablecerCliente(Guid idCliente)
        {
            if (idCliente == Guid.Empty)
                throw new ArgumentException("Debe seleccionar un cliente.", nameof(idCliente));

            IdCliente = idCliente;
        }

        private void ValidarYEstablecerFechaVencimiento(DateTime? fechaVencimiento)
        {
            if (fechaVencimiento.HasValue && fechaVencimiento.Value < FechaEmision)
                throw new ArgumentException("La fecha de vencimiento no puede ser anterior a la fecha de emisión.", nameof(fechaVencimiento));

            FechaVencimiento = fechaVencimiento;
        }

        public void ValidarNegocio()
        {
            ValidarYEstablecerNumero(Numero);
            ValidarYEstablecerCliente(IdCliente);
            
            if (FechaVencimiento.HasValue)
                ValidarYEstablecerFechaVencimiento(FechaVencimiento);

            if (!Detalles.Any() && Estado >= 2)
                throw new InvalidOperationException("Un presupuesto debe tener al menos un detalle.");

            foreach (var detalle in Detalles)
            {
                detalle.ValidarNegocio();
            }
        }
    }

    /// <summary>
    /// Entidad de dominio que representa una línea de detalle de un presupuesto.
    /// 
    /// Cada detalle es un producto o servicio cotizado con su cantidad, precio,
    /// descuento e impuesto. Múltiples detalles conforman un presupuesto completo.
    /// 
    /// Responsabilidades:
    /// - Validar producto, cantidad, precio y descuento
    /// - Calcular subtotal, monto de descuento, total con IVA
    /// - Persistir totales para auditoría
    /// - Mantener número de renglón secuencial
    /// 
    /// Invariantes:
    /// - Cantidad > 0 y <= 999,999
    /// - Precio >= 0 y <= 999,999,999
    /// - Descuento >= 0 y <= 100 (%)
    /// - Producto siempre debe estar asignado (no GUID vacío)
    /// 
    /// Cálculos automáticos:
    /// - Subtotal = Cantidad * Precio
    /// - DescuentoMonto = Subtotal * (Descuento / 100)
    /// - Total = Subtotal - DescuentoMonto
    /// - IVA = Total * (PorcentajeIVA / 100)
    /// - TotalConIVA = Total + IVA
    /// 
    /// </summary>
    public class PresupuestoDetalleDM
    {
        /// <summary>Identificador único del detalle en el sistema</summary>
        public Guid Id { get; private set; }
        
        /// <summary>Número o código interno del detalle (para referencia)</summary>
        public string Numero { get; private set; }
        
        /// <summary>GUID del presupuesto padre al que pertenece este detalle</summary>
        public Guid? IdPresupuesto { get; private set; }
        
        /// <summary>GUID del producto o servicio siendo cotizado</summary>
        public Guid? IdProducto { get; private set; }
        
        /// <summary>Cantidad del producto. Debe ser > 0 y <= 999,999</summary>
        public decimal Cantidad { get; private set; }
        
        /// <summary>Precio unitario del producto (sin descuentos, sin IVA). Debe ser >= 0 y <= 999,999,999</summary>
        public decimal Precio { get; private set; }
        
        /// <summary>Descuento a aplicar como porcentaje. Debe estar entre 0 y 100.</summary>
        public decimal Descuento { get; private set; }
        
        /// <summary>Número secuencial del renglón dentro del presupuesto</summary>
        public int Renglon { get; private set; }
        
        /// <summary>Porcentaje de IVA a aplicar sobre el total (ej: 21.00, 10.50, 0)</summary>
        public decimal PorcentajeIVA { get; private set; }
        
        /// <summary>Total del detalle persistido en BD para auditoría (con IVA)</summary>
        public decimal TotalPersistido { get; private set; }

        /// <summary>
        /// Crea un nuevo detalle de presupuesto para un producto específico.
        /// 
        /// Se validan automáticamente cantidad, precio y descuento.
        /// El total persistido se calcula al crearse.
        /// </summary>
        /// <param name="idProducto">GUID del producto (no puede ser nulo o vacío)</param>
        /// <param name="cantidad">Cantidad del producto (> 0 y <= 999,999)</param>
        /// <param name="precio">Precio unitario (>= 0 y <= 999,999,999)</param>
        /// <param name="porcentajeIVA">Porcentaje de IVA a aplicar</param>
        /// <param name="descuento">Descuento porcentual (0-100, predeterminado: 0)</param>
        /// <exception cref="ArgumentException">Si alguno de los parámetros no cumple validaciones</exception>
        /// <exception cref="ArgumentNullException">Si el producto es nulo</exception>
        // Constructor para creación inicial
        public PresupuestoDetalleDM(
            Guid? idProducto,
            decimal cantidad,
            decimal precio,
            decimal porcentajeIVA,
            decimal descuento = 0)
        {
            Id = Guid.NewGuid();
            
            ValidarYEstablecerProducto(idProducto);
            ValidarYEstablecerCantidad(cantidad);
            ValidarYEstablecerPrecio(precio);
            ValidarYEstablecerDescuento(descuento);
            PorcentajeIVA = porcentajeIVA;

            TotalPersistido = CalcularTotal();
        }

        /// <summary>
        /// Constructor para hidratar un detalle desde la base de datos.
        /// 
        /// Se utiliza internamente por repositorios para reconstruir
        /// detalles con sus valores persistidos.
        /// </summary>
        /// <param name="id">GUID único del detalle</param>
        /// <param name="numero">Número del detalle</param>
        /// <param name="idPresupuesto">GUID del presupuesto padre (puede ser nulo)</param>
        /// <param name="idProducto">GUID del producto (puede ser nulo)</param>
        /// <param name="cantidad">Cantidad cotizada</param>
        /// <param name="precio">Precio unitario</param>
        /// <param name="descuento">Descuento aplicado</param>
        /// <param name="renglon">Número de renglón</param>
        /// <param name="porcentajeIVA">Porcentaje de IVA</param>
        /// <param name="totalPersistido">Total persistido con IVA (puede ser nulo)</param>
        /// <exception cref="ArgumentException">Si el ID es un GUID vacío</exception>
        // Constructor para cargar desde base de datos
        public PresupuestoDetalleDM(
            Guid id,
            string numero,
            Guid? idPresupuesto,
            Guid? idProducto,
            decimal cantidad,
            decimal precio,
            decimal descuento,
            int renglon,
            decimal porcentajeIVA,
            decimal? totalPersistido = null)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("El ID del detalle no puede ser vacío.", nameof(id));

            Id = id;
            Numero = numero;
            IdPresupuesto = idPresupuesto;
            IdProducto = idProducto;
            Cantidad = cantidad;
            Precio = precio;
            Descuento = descuento;
            Renglon = renglon;
            PorcentajeIVA = porcentajeIVA;
            TotalPersistido = totalPersistido.HasValue ? totalPersistido.Value : 0m;
        }

        /// <summary>
        /// Método interno para establecer la relación con el presupuesto padre.
        /// 
        /// Solo debe ser invocado por PresupuestoDM.AgregarDetalle().
        /// </summary>
        /// <param name="idPresupuesto">GUID del presupuesto padre</param>
        internal void EstablecerPresupuesto(Guid idPresupuesto)
        {
            IdPresupuesto = idPresupuesto;
        }

        /// <summary>
        /// Método interno para establecer el número de renglón del detalle.
        /// 
        /// Se utiliza cuando se agregan o remueven detalles para mantener
        /// secuencia numérica. Solo debe ser invocado por PresupuestoDM.
        /// </summary>
        /// <param name="renglon">Número de renglón (debe ser > 0)</param>
        /// <exception cref="ArgumentException">Si el renglón es <= 0</exception>
        internal void EstablecerRenglon(int renglon)
        {
            if (renglon <= 0)
                throw new ArgumentException("El renglón debe ser mayor a cero.", nameof(renglon));

            Renglon = renglon;
        }

        /// <summary>
        /// Método interno para establecer el número del detalle.
        /// 
        /// Solo debe ser invocado por la capa persistencia (repositorio).
        /// </summary>
        /// <param name="numero">Número o código del detalle</param>
        internal void EstablecerNumero(string numero)
        {
            Numero = numero;
        }

        /// <summary>
        /// Actualiza los datos cuantitativos del detalle (cantidad, precio, descuento, IVA).
        /// 
        /// Valida todos los parámetros y recalcula el total persistido.
        /// </summary>
        /// <param name="cantidad">Nueva cantidad</param>
        /// <param name="precio">Nuevo precio unitario</param>
        /// <param name="descuento">Nuevo descuento porcentual</param>
        /// <param name="porcentajeIVA">Nuevo porcentaje de IVA</param>
        /// <exception cref="ArgumentException">Si algún parámetro no cumple validaciones</exception>
        // Actualización
        public void ActualizarDatos(
            decimal cantidad,
            decimal precio,
            decimal descuento,
            decimal porcentajeIVA)
        {
            ValidarYEstablecerCantidad(cantidad);
            ValidarYEstablecerPrecio(precio);
            ValidarYEstablecerDescuento(descuento);
            PorcentajeIVA = porcentajeIVA;

            TotalPersistido = CalcularTotal();
        }

        /// <summary>
        /// Calcula el subtotal (cantidad * precio) sin aplicar descuento ni IVA.
        /// </summary>
        /// <returns>Subtotal neto del detalle</returns>
        // Cálculos
        public decimal CalcularSubtotal()
        {
            return Cantidad * Precio;
        }

        /// <summary>
        /// Calcula el monto del descuento a aplicar sobre el subtotal.
        /// </summary>
        /// <returns>Monto a descontar = Subtotal * (Descuento / 100)</returns>
        public decimal CalcularDescuentoMonto()
        {
            return CalcularSubtotal() * (Descuento / 100);
        }

        public decimal CalcularTotal()
        {
            return CalcularSubtotal() - CalcularDescuentoMonto();
        }

        public decimal CalcularIva()
        {
            return CalcularTotal() * (PorcentajeIVA / 100);
        }

        /// <summary>
        /// Calcula el total final del detalle incluyendo IVA.
        /// 
        /// Fórmula: Total + IVA
        /// </summary>
        /// <returns>Total del detalle con IVA incluido</returns>
        public decimal CalcularTotalConIva()
        {
            return CalcularTotal() + CalcularIva();
        }

        /// <summary>
        /// Establece el total persistido del detalle para auditoría.
        /// 
        /// Se invoca desde la capa BLL después de todos los cálculos
        /// para guardar el valor final exacto.
        /// </summary>
        /// <param name="total">Total final del detalle (no puede ser negativo)</param>
        /// <exception cref="ArgumentException">Si el total es negativo</exception>
        public void EstablecerTotalPersistido(decimal total)
        {
            if (total < 0)
                throw new ArgumentException("El total no puede ser negativo.", nameof(total));

            TotalPersistido = total;
        }

        // Validaciones de negocio
        private void ValidarYEstablecerProducto(Guid? idProducto)
        {
            if (!idProducto.HasValue || idProducto.Value == Guid.Empty)
                throw new ArgumentException("Debe seleccionar un producto.", nameof(idProducto));

            IdProducto = idProducto;
        }

        private void ValidarYEstablecerCantidad(decimal cantidad)
        {
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(cantidad));

            if (cantidad > 999999)
                throw new ArgumentException("La cantidad no puede exceder 999,999.", nameof(cantidad));

            Cantidad = cantidad;
        }

        private void ValidarYEstablecerPrecio(decimal precio)
        {
            if (precio < 0)
                throw new ArgumentException("El precio no puede ser negativo.", nameof(precio));

            if (precio > 999999999)
                throw new ArgumentException("El precio no puede exceder 999,999,999.", nameof(precio));

            Precio = precio;
        }

        private void ValidarYEstablecerDescuento(decimal descuento)
        {
            if (descuento < 0)
                throw new ArgumentException("El descuento no puede ser negativo.", nameof(descuento));

            if (descuento > 100)
                throw new ArgumentException("El descuento no puede ser mayor al 100%.", nameof(descuento));

            Descuento = descuento;
        }

        public void ValidarNegocio()
        {
            ValidarYEstablecerProducto(IdProducto);
            ValidarYEstablecerCantidad(Cantidad);
            ValidarYEstablecerPrecio(Precio);
            ValidarYEstablecerDescuento(Descuento);

            if (Renglon <= 0)
                throw new InvalidOperationException("El renglón debe estar establecido.");
        }
    }
}
