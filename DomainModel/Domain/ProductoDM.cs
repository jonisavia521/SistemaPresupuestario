using System;
using System.Collections.Generic;

namespace DomainModel.Domain
{
    /// <summary>
    /// Entidad de dominio que representa un Producto en el sistema.
    /// Contiene la lógica de negocio pura sin dependencias de infraestructura.
    /// </summary>
    public class ProductoDM
    {
        public Guid ID { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public bool Inhabilitado { get; set; }
        public DateTime FechaAlta { get; set; }
        public int UsuarioAlta { get; set; }

        /// <summary>
        /// Valida las reglas de negocio del producto.
        /// Esta lógica NO debe estar en la DAL ni en la UI.
        /// </summary>
        public List<string> ValidarNegocio()
        {
            var errores = new List<string>();

            // Validación de negocio: El código es obligatorio
            if (string.IsNullOrWhiteSpace(Codigo))
            {
                errores.Add("El código del producto es obligatorio.");
            }
            else if (Codigo.Length > 50)
            {
                errores.Add("El código del producto no puede exceder los 50 caracteres.");
            }

            // Validación de negocio: La descripción no puede exceder el límite
            if (!string.IsNullOrWhiteSpace(Descripcion) && Descripcion.Length > 50)
            {
                errores.Add("La descripción del producto no puede exceder los 50 caracteres.");
            }

            // Validación de negocio: La fecha de alta debe ser válida
            if (FechaAlta == DateTime.MinValue)
            {
                errores.Add("La fecha de alta es inválida.");
            }

            // Validación de negocio: No se puede modificar un producto con fecha futura
            if (FechaAlta > DateTime.Now)
            {
                errores.Add("La fecha de alta no puede ser futura.");
            }

            return errores;
        }
    }
}
