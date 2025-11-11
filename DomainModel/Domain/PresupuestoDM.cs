using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainModel.Domain
{
    /// <summary>
    /// Entidad de dominio Presupuesto - Representa la lógica de negocio pura de un presupuesto/cotización
    /// Estados:
    /// 1 = Borrado (eliminado lógicamente)
    /// 2 = Emitido (estado inicial al crear)
    /// 3 = Aprobado
    /// 4 = Rechazado
    /// 5 = Vencido (implícito: Emitido o Aprobado con fecha vencida)
    /// 6 = Facturado
    /// </summary>
    public class PresupuestoDM
    {
        public Guid Id { get; private set; }
        public string Numero { get; private set; }
        public Guid IdCliente { get; private set; }
        public DateTime FechaEmision { get; private set; }
        public int Estado { get; private set; }
        public DateTime? FechaVencimiento { get; private set; }
        public Guid? IdPresupuestoPadre { get; private set; } // Para presupuestos derivados/copias
        public Guid? IdVendedor { get; private set; }
        public List<PresupuestoDetalleDM> Detalles { get; private set; }

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
            Estado = 2; // Emitido por defecto (estado inicial)
            Detalles = new List<PresupuestoDetalleDM>();

            ValidarYEstablecerNumero(numero);
            ValidarYEstablecerCliente(idCliente);
            FechaEmision = fechaEmision;
            ValidarYEstablecerFechaVencimiento(fechaVencimiento);
            IdVendedor = idVendedor;
            IdPresupuestoPadre = idPresupuestoPadre;
        }

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
            List<PresupuestoDetalleDM> detalles = null)
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
        }

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

        // Gestión de detalles
        public void AgregarDetalle(PresupuestoDetalleDM detalle)
        {
            if (detalle == null)
                throw new ArgumentNullException(nameof(detalle));

            if (Estado == 3) // Aprobado
                throw new InvalidOperationException("No se pueden agregar detalles a un presupuesto aprobado.");

            if (Estado == 4) // Rechazado
                throw new InvalidOperationException("No se pueden agregar detalles a un presupuesto rechazado.");

            if (Estado == 6) // Facturado
                throw new InvalidOperationException("No se pueden agregar detalles a un presupuesto facturado.");

            if (Estado == 1) // Borrado
                throw new InvalidOperationException("No se pueden agregar detalles a un presupuesto borrado.");

            detalle.EstablecerPresupuesto(Id);
            detalle.EstablecerRenglon(Detalles.Count + 1);
            Detalles.Add(detalle);
        }

        public void RemoverDetalle(Guid detalleId)
        {
            if (Estado == 3) // Aprobado
                throw new InvalidOperationException("No se pueden eliminar detalles de un presupuesto aprobado.");

            if (Estado == 4) // Rechazado
                throw new InvalidOperationException("No se pueden eliminar detalles de un presupuesto rechazado.");

            if (Estado == 6) // Facturado
                throw new InvalidOperationException("No se pueden eliminar detalles de un presupuesto facturado.");

            if (Estado == 1) // Borrado
                throw new InvalidOperationException("No se pueden eliminar detalles de un presupuesto borrado.");

            var detalle = Detalles.FirstOrDefault(d => d.Id == detalleId);
            if (detalle != null)
            {
                Detalles.Remove(detalle);
                ReordenarRenglones();
            }
        }

        public void LimpiarDetalles()
        {
            if (Estado == 3) // Aprobado
                throw new InvalidOperationException("No se pueden eliminar detalles de un presupuesto aprobado.");

            if (Estado == 6) // Facturado
                throw new InvalidOperationException("No se pueden eliminar detalles de un presupuesto facturado.");

            Detalles.Clear();
        }

        private void ReordenarRenglones()
        {
            for (int i = 0; i < Detalles.Count; i++)
            {
                Detalles[i].EstablecerRenglon(i + 1);
            }
        }

        // Gestión de estados
        public void CambiarEstado(int nuevoEstado)
        {
            if (nuevoEstado < 1 || nuevoEstado > 6)
                throw new ArgumentException("Estado inválido. Debe ser entre 1 y 6.", nameof(nuevoEstado));

            // Validar transiciones de estado
            if (Estado == 3 && nuevoEstado != 3 && nuevoEstado != 6) // Desde Aprobado solo puede ir a Facturado
                throw new InvalidOperationException("Un presupuesto aprobado solo puede cambiar a estado Facturado.");

            if (Estado == 6) // Facturado es estado final
                throw new InvalidOperationException("Un presupuesto facturado no puede cambiar de estado.");

            if (Estado == 1) // Borrado es estado final
                throw new InvalidOperationException("Un presupuesto borrado no puede cambiar de estado.");

            Estado = nuevoEstado;
        }

        public void Emitir()
        {
            if (!Detalles.Any())
                throw new InvalidOperationException("No se puede emitir un presupuesto sin detalles.");

            CambiarEstado(2); // Emitido
        }

        public void Aprobar()
        {
            if (Estado != 2)
                throw new InvalidOperationException("Solo se pueden aprobar presupuestos emitidos.");

            CambiarEstado(3); // Aprobado
        }

        public void Rechazar()
        {
            if (Estado != 2)
                throw new InvalidOperationException("Solo se pueden rechazar presupuestos emitidos.");

            CambiarEstado(4); // Rechazado
        }

        public void Facturar()
        {
            if (Estado != 3)
                throw new InvalidOperationException("Solo se pueden facturar presupuestos aprobados.");

            CambiarEstado(6); // Facturado
        }

        public void Borrar()
        {
            if (Estado != 2)
                throw new InvalidOperationException("Solo se pueden borrar presupuestos emitidos.");

            CambiarEstado(1); // Borrado
        }

        /// <summary>
        /// Determina si el presupuesto está vencido (estado implícito)
        /// </summary>
        public bool EstaVencido()
        {
            if (!FechaVencimiento.HasValue)
                return false;

            // Solo puede estar vencido si está Emitido o Aprobado
            if (Estado != 2 && Estado != 3)
                return false;

            return FechaVencimiento.Value.Date < DateTime.Now.Date;
        }

        /// <summary>
        /// Obtiene el estado efectivo (considera el vencimiento)
        /// </summary>
        public int ObtenerEstadoEfectivo()
        {
            if (EstaVencido())
                return 5; // Vencido

            return Estado;
        }

        // Cálculos
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

        // ==================== VALIDACIONES DE NEGOCIO ====================

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
    /// Entidad de dominio PresupuestoDetalle - Representa una línea del presupuesto
    /// </summary>
    public class PresupuestoDetalleDM
    {
        public Guid Id { get; private set; }
        public string Numero { get; private set; } // Número del presupuesto (desnormalizado)
        public Guid? IdPresupuesto { get; private set; }
        public Guid? IdProducto { get; private set; }
        public decimal Cantidad { get; private set; }
        public decimal Precio { get; private set; }
        public decimal Descuento { get; private set; } // Porcentaje de descuento
        public int Renglon { get; private set; }
        public decimal PorcentajeIVA { get; private set; } // IVA del producto

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
        }

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
            decimal porcentajeIVA)
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
        }

        // Métodos internos para la entidad padre
        internal void EstablecerPresupuesto(Guid idPresupuesto)
        {
            IdPresupuesto = idPresupuesto;
        }

        internal void EstablecerRenglon(int renglon)
        {
            if (renglon <= 0)
                throw new ArgumentException("El renglón debe ser mayor a cero.", nameof(renglon));

            Renglon = renglon;
        }

        internal void EstablecerNumero(string numero)
        {
            Numero = numero;
        }

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
        }

        // Cálculos
        public decimal CalcularSubtotal()
        {
            return Cantidad * Precio;
        }

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

        public decimal CalcularTotalConIva()
        {
            return CalcularTotal() + CalcularIva();
        }

        // ==================== VALIDACIONES DE NEGOCIO ====================

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
