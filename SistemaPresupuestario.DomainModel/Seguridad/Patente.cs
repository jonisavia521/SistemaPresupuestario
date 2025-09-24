using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SistemaPresupuestario.DomainModel.Seguridad.Base;

namespace SistemaPresupuestario.DomainModel.Seguridad
{
    /// <summary>
    /// Represents a specific permission (patent) in the system
    /// Implements Composite pattern as leaf node
    /// </summary>
    public class Patente : EntityBase, IComponentePermiso
    {
        /// <summary>
        /// Name of the permission
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        /// <summary>
        /// Associated view or form name for this permission
        /// </summary>
        [MaxLength(200)]
        public string Vista { get; set; }

        /// <summary>
        /// Description of what this permission allows
        /// </summary>
        [MaxLength(500)]
        public string Descripcion { get; set; }

        /// <summary>
        /// Users who have this patent directly assigned
        /// </summary>
        public virtual ICollection<Usuario> Usuarios { get; set; }

        /// <summary>
        /// Families that contain this patent
        /// </summary>
        public virtual ICollection<Familia> Familias { get; set; }

        public Patente()
        {
            Usuarios = new HashSet<Usuario>();
            Familias = new HashSet<Familia>();
        }

        public Patente(string nombre, string vista = null, string descripcion = null) : this()
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Vista = vista;
            Descripcion = descripcion;
        }

        #region IComponentePermiso Implementation

        /// <summary>
        /// Returns itself as it's a leaf node in the composite pattern
        /// </summary>
        public IEnumerable<Patente> ObtenerPatentesDirectas()
        {
            yield return this;
        }

        /// <summary>
        /// Returns itself as it has no children
        /// </summary>
        public IEnumerable<Patente> ObtenerPatentesEfectivas()
        {
            yield return this;
        }

        /// <summary>
        /// Not supported for Patente (leaf node)
        /// </summary>
        public void AgregarComponente(IComponentePermiso componente)
        {
            throw new InvalidOperationException("Cannot add components to a Patente (leaf node)");
        }

        /// <summary>
        /// Not supported for Patente (leaf node)
        /// </summary>
        public void EliminarComponente(IComponentePermiso componente)
        {
            throw new InvalidOperationException("Cannot remove components from a Patente (leaf node)");
        }

        /// <summary>
        /// Returns empty collection as Patente has no children
        /// </summary>
        public IEnumerable<IComponentePermiso> ObtenerHijos()
        {
            return Enumerable.Empty<IComponentePermiso>();
        }

        #endregion

        public override string ToString()
        {
            return $"Patente: {Nombre}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Patente other)
                return Id == other.Id;
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}