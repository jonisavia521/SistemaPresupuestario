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
        public decimal PorcentajeIVA { get; set; }

        /// <summary>
        /// Valida las reglas de negocio del producto.
        /// Esta lógica debe estar en el dominio, no en la DAL ni en la UI.
        /// </summary>
        public List<string> ValidarNegocio()
        {
            var errores = new List<string>();

            if (string.IsNullOrWhiteSpace(Codigo))
            {
                errores.Add("El código del producto es obligatorio.");
            }
            else if (Codigo.Length > 50)
            {
                errores.Add("El código del producto no puede exceder los 50 caracteres.");
            }

            if (!string.IsNullOrWhiteSpace(Descripcion) && Descripcion.Length > 50)
            {
                errores.Add("La descripción del producto no puede exceder los 50 caracteres.");
            }

            if (FechaAlta == DateTime.MinValue)
            {
                errores.Add("La fecha de alta es inválida.");
            }

            if (FechaAlta > DateTime.Now)
            {
                errores.Add("La fecha de alta no puede ser futura.");
            }

            if (PorcentajeIVA != 0.00m && PorcentajeIVA != 10.50m && PorcentajeIVA != 21.00m)
            {
                errores.Add("El porcentaje de IVA debe ser 0.00 (Exento), 10.50 o 21.00.");
            }

            return errores;
        }
    }
}
