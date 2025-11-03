namespace BLL.Enums
{
    /// <summary>
    /// Define los modos de acceso al formulario de presupuestos
    /// según la acción del usuario desde el menú principal
    /// </summary>
    public enum ModoPresupuesto
    {
        /// <summary>
        /// Modo para crear nuevos presupuestos (estado Borrador)
        /// Solo permite crear nuevos presupuestos en estado Borrador
        /// </summary>
        Generar = 0,

        /// <summary>
        /// Modo para gestionar presupuestos existentes
        /// Muestra: Borrador, Aprobado, Rechazado y Vencido
        /// Permite: Editar/Eliminar Borradores, Ver el resto, Copiar cualquiera
        /// </summary>
        Gestionar = 1,

        /// <summary>
        /// Modo para aprobar presupuestos emitidos
        /// Solo muestra presupuestos en estado Emitido
        /// Permite: Aprobar o Rechazar
        /// </summary>
        Aprobar = 2
    }
}
