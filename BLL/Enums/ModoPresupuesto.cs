namespace BLL.Enums
{
    /// <summary>
    /// Enumeración que define los modos de operación disponibles al acceder al formulario de presupuestos.
    /// 
    /// El modo de presupuesto determina:
    /// - Qué presupuestos se muestran en la grilla
    /// - Qué operaciones están disponibles (crear, editar, eliminar, aprobar, rechazar)
    /// - Qué validaciones se aplican
    /// 
    /// Se selecciona según la acción del usuario desde el menú principal de la aplicación.
    /// </summary>
    public enum ModoPresupuesto
    {
        /// <summary>
        /// Modo de Gestión Completa de Presupuestos.
        /// 
        /// Acceso: Menú Principal > Cotizaciones > Gestión de Cotizaciones
        /// 
        /// Presupuestos visibles: Todos (estados 1-6)
        /// - 1 = Borrado
        /// - 2 = Emitido
        /// - 3 = Aprobado
        /// - 4 = Rechazado
        /// - 5 = Vencido
        /// - 6 = Facturado
        /// 
        /// Operaciones permitidas:
        /// - Crear nuevo presupuesto en estado Emitido
        /// - Editar presupuestos en estado Emitido
        /// - Eliminar (borrado lógico) presupuestos en estado Emitido (cambia a estado Borrado)
        /// - Ver detalles de cualquier presupuesto
        /// - Copiar cualquier presupuesto (crea uno nuevo en estado Emitido)
        /// - Cambiar estado: Aprobar, Rechazar, Facturar (con validaciones)
        /// - Acceder a listados de clientes, vendedores, productos, etc.
        /// 
        /// Validaciones especiales:
        /// - Solo se pueden editar presupuestos Emitidos
        /// - Solo se pueden borrar presupuestos Emitidos
        /// - Solo se pueden aprobar presupuestos Emitidos
        /// - Solo se pueden facturar presupuestos Aprobados
        /// </summary>
        Gestionar = 0,

        /// <summary>
        /// Modo de Aprobación de Presupuestos.
        /// 
        /// Acceso: Menú Principal > Cotizaciones > Aprobar Cotizaciones
        /// 
        /// Presupuestos visibles: Solo estado Emitido (2)
        /// 
        /// Operaciones permitidas:
        /// - Aprobar presupuesto (cambia a estado Aprobado)
        /// - Rechazar presupuesto (cambia a estado Rechazado)
        /// - Ver detalles del presupuesto
        /// 
        /// Restricciones:
        /// - NO se puede crear nuevos presupuestos
        /// - NO se puede editar presupuestos
        /// - NO se puede eliminar presupuestos
        /// - NO se puede copiar presupuestos
        /// - NO se puede acceder a gestión de clientes, vendedores, etc.
        /// 
        /// Propósito: Interfaz simplificada para roles que solo aprueban cotizaciones.
        /// </summary>
        Aprobar = 1
    }
}
