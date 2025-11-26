using System;
using System.Collections.Generic;

namespace DomainModel.Domain
{
    /// <summary>
    /// Entidad de dominio que representa un producto o servicio del catálogo del sistema.
    /// 
    /// Esta clase encapsula la lógica de negocio asociada con productos, incluyendo
    /// validaciones de códigos, descripciones y tasas impositivas. Los productos son
    /// utilizados como líneas de detalle en presupuestos y facturas.
    /// 
    /// Responsabilidades:
    /// - Validar códigos únicos y descripciones
    /// - Validar porcentajes de IVA según opciones permitidas
    /// - Mantener auditoría de creación
    /// - Gestionar estado de habilitación/inhabilitación
    /// 
    /// Invariantes:
    /// - El código es obligatorio y no puede exceder 50 caracteres
    /// - El porcentaje de IVA debe ser 0 (Exento), 10.50 o 21.00
    /// - La fecha de alta no puede ser futura
    /// 
    /// Estados:
    /// - Habilitado: Producto disponible para usar en presupuestos
    /// - Inhabilitado: Producto no disponible, no aparece en listas de selección
    /// 
    /// Ejemplo de uso:
    /// <code>
    /// var producto = new ProductoDM
    /// {
    ///     Codigo = "PRD001",
    ///     Descripcion = "Laptop Dell Inspiron 15",
    ///     PorcentajeIVA = 21.00m,
    ///     Inhabilitado = false,
    ///     FechaAlta = DateTime.Now
    /// };
    /// var errores = producto.ValidarNegocio();
    /// </code>
    /// </summary>
    public class ProductoDM
    {
        /// <summary>Identificador único del producto en el sistema</summary>
        public Guid ID { get; set; }
        
        /// <summary>Código alfanumérico único para referencia del producto. Máximo 50 caracteres.</summary>
        public string Codigo { get; set; }
        
        /// <summary>Descripción detallada del producto. Máximo 50 caracteres.</summary>
        public string Descripcion { get; set; }
        
        /// <summary>Indica si el producto está inhabilitado. Los productos inhabilitados no aparecen en listas de selección.</summary>
        public bool Inhabilitado { get; set; }
        
        /// <summary>Fecha y hora en que se registró el producto en el sistema</summary>
        public DateTime FechaAlta { get; set; }
        
        /// <summary>Identificador del usuario que registró el producto</summary>
        public int UsuarioAlta { get; set; }
        
        /// <summary>Porcentaje de IVA a aplicar: 0.00 (Exento), 10.50 o 21.00 (General)</summary>
        public decimal PorcentajeIVA { get; set; }

        /// <summary>
        /// Valida todas las propiedades del producto contra las reglas de negocio.
        /// 
        /// Este método realiza validaciones comprehensivas de los datos del producto:
        /// - Código: obligatorio, máximo 50 caracteres
        /// - Descripción: máximo 50 caracteres si está completa
        /// - Fecha de alta: debe ser válida y no futura
        /// - IVA: debe ser exactamente 0.00, 10.50 o 21.00
        /// 
        /// Retorna una lista de mensajes de error descriptivos. Si la lista está vacía,
        /// el producto cumple todas las validaciones.
        /// </summary>
        /// <returns>
        /// Lista de mensajes de error encontrados durante la validación.
        /// Si la lista está vacía, todas las validaciones pasaron.
        /// </returns>
        /// <remarks>
        /// Este método se utiliza típicamente en la capa BLL antes de persistir
        /// cambios. Retorna lista de errores en lugar de lanzar excepciones para
        /// permitir recopilación múltiple de problemas.
        /// </remarks>
        public List<string> ValidarNegocio()
        {
            var errores = new List<string>();

            // Validación de código (requerido)
            if (string.IsNullOrWhiteSpace(Codigo))
            {
                errores.Add("El código del producto es obligatorio.");
            }
            else if (Codigo.Length > 50)
            {
                errores.Add("El código del producto no puede exceder los 50 caracteres.");
            }

            // Validación de descripción (opcional pero con límite)
            if (!string.IsNullOrWhiteSpace(Descripcion) && Descripcion.Length > 50)
            {
                errores.Add("La descripción del producto no puede exceder los 50 caracteres.");
            }

            // Validación de fecha de alta
            if (FechaAlta == DateTime.MinValue)
            {
                errores.Add("La fecha de alta es inválida.");
            }

            if (FechaAlta > DateTime.Now)
            {
                errores.Add("La fecha de alta no puede ser futura.");
            }

            // Validación de porcentaje de IVA (solo valores predefinidos)
            if (PorcentajeIVA != 0.00m && PorcentajeIVA != 10.50m && PorcentajeIVA != 21.00m)
            {
                errores.Add("El porcentaje de IVA debe ser 0.00 (Exento), 10.50 o 21.00.");
            }

            return errores;
        }
    }
}
