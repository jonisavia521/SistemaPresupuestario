namespace BLL.Enums
{
    /// <summary>
    /// Define los modos de acceso al formulario de presupuestos
    /// según la acción del usuario desde el menú principal
    /// </summary>
    public enum ModoPresupuesto
    {
        /// <summary>
        /// Modo para gestionar todos los presupuestos
        /// Muestra: Borrado (1), Emitido (2), Aprobado (3), Rechazado (4), Vencido (5), Facturado (6)
        /// Permite: Crear nuevos, Editar Emitidos, Eliminar (borrado lógico) Emitidos, Ver el resto, Copiar cualquiera, Facturar Aprobados
        /// </summary>
        Gestionar = 0,

        /// <summary>
        /// Modo para aprobar presupuestos emitidos
        /// Solo muestra presupuestos en estado Emitido (2)
        /// Permite: Aprobar o Rechazar
        /// </summary>
        Aprobar = 1
    }
}
