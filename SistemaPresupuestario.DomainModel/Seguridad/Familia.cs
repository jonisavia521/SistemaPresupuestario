using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SistemaPresupuestario.DomainModel.Seguridad.Base;

namespace SistemaPresupuestario.DomainModel.Seguridad
{
    /// <summary>
    /// Represents a family of permissions that can contain other families and patents
    /// Implements Composite pattern as composite node
    /// </summary>
    public class Familia : EntityBase, IComponentePermiso
    {
        /// <summary>
        /// Name of the family
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        /// <summary>
        /// Description of the family's purpose
        /// </summary>
        [MaxLength(500)]
        public string Descripcion { get; set; }

        /// <summary>
        /// Parent family (null for root families)
        /// </summary>
        public virtual Familia FamiliaPadre { get; set; }

        /// <summary>
        /// Foreign key to parent family
        /// </summary>
        public Guid? FamiliaPadreId { get; set; }

        /// <summary>
        /// Child families
        /// </summary>
        public virtual ICollection<Familia> FamiliasHijas { get; set; }

        /// <summary>
        /// Patents directly assigned to this family
        /// </summary>
        public virtual ICollection<Patente> Patentes { get; set; }

        /// <summary>
        /// Users who have this family assigned
        /// </summary>
        public virtual ICollection<Usuario> Usuarios { get; set; }

        public Familia()
        {
            FamiliasHijas = new HashSet<Familia>();
            Patentes = new HashSet<Patente>();
            Usuarios = new HashSet<Usuario>();
        }

        public Familia(string nombre, string descripcion = null) : this()
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Descripcion = descripcion;
        }

        #region IComponentePermiso Implementation

        /// <summary>
        /// Gets all patents directly assigned to this family
        /// </summary>
        public IEnumerable<Patente> ObtenerPatentesDirectas()
        {
            return Patentes?.ToList() ?? Enumerable.Empty<Patente>();
        }

        /// <summary>
        /// Gets all effective patents including those from child families recursively
        /// Uses DFS to avoid cycles and ensure all permissions are included
        /// </summary>
        public IEnumerable<Patente> ObtenerPatentesEfectivas()
        {
            var patentesEfectivas = new HashSet<Patente>();
            var familiasVisitadas = new HashSet<Guid>();
            
            ObtenerPatentesEfectivasRecursivo(patentesEfectivas, familiasVisitadas);
            
            return patentesEfectivas;
        }

        /// <summary>
        /// Recursive helper method to calculate effective patents avoiding cycles
        /// </summary>
        private void ObtenerPatentesEfectivasRecursivo(HashSet<Patente> patentesEfectivas, HashSet<Guid> familiasVisitadas)
        {
            // Avoid cycles
            if (familiasVisitadas.Contains(Id))
                return;

            familiasVisitadas.Add(Id);

            // Add direct patents from this family
            if (Patentes != null)
            {
                foreach (var patente in Patentes)
                {
                    patentesEfectivas.Add(patente);
                }
            }

            // Recursively add patents from child families
            if (FamiliasHijas != null)
            {
                foreach (var familiaHija in FamiliasHijas)
                {
                    familiaHija.ObtenerPatentesEfectivasRecursivo(patentesEfectivas, familiasVisitadas);
                }
            }
        }

        /// <summary>
        /// Adds a component (Familia or Patente) to this family
        /// </summary>
        public void AgregarComponente(IComponentePermiso componente)
        {
            if (componente == null)
                throw new ArgumentNullException(nameof(componente));

            if (componente is Familia familia)
            {
                // Prevent cycles
                if (EsAncestro(familia))
                    throw new InvalidOperationException("Cannot add family: would create a cycle");

                if (!FamiliasHijas.Contains(familia))
                {
                    FamiliasHijas.Add(familia);
                    familia.FamiliaPadre = this;
                    familia.FamiliaPadreId = Id;
                }
            }
            else if (componente is Patente patente)
            {
                if (!Patentes.Contains(patente))
                {
                    Patentes.Add(patente);
                }
            }
        }

        /// <summary>
        /// Removes a component from this family
        /// </summary>
        public void EliminarComponente(IComponentePermiso componente)
        {
            if (componente == null)
                return;

            if (componente is Familia familia)
            {
                if (FamiliasHijas.Contains(familia))
                {
                    FamiliasHijas.Remove(familia);
                    familia.FamiliaPadre = null;
                    familia.FamiliaPadreId = null;
                }
            }
            else if (componente is Patente patente)
            {
                Patentes.Remove(patente);
            }
        }

        /// <summary>
        /// Gets all direct child components (families and patents)
        /// </summary>
        public IEnumerable<IComponentePermiso> ObtenerHijos()
        {
            var hijos = new List<IComponentePermiso>();
            
            if (FamiliasHijas != null)
                hijos.AddRange(FamiliasHijas.Cast<IComponentePermiso>());
            
            if (Patentes != null)
                hijos.AddRange(Patentes.Cast<IComponentePermiso>());
            
            return hijos;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Checks if the given family is an ancestor of this family (to prevent cycles)
        /// </summary>
        private bool EsAncestro(Familia familia)
        {
            var actual = FamiliaPadre;
            var visitadas = new HashSet<Guid> { Id };

            while (actual != null && !visitadas.Contains(actual.Id))
            {
                if (actual.Id == familia.Id)
                    return true;

                visitadas.Add(actual.Id);
                actual = actual.FamiliaPadre;
            }

            return false;
        }

        /// <summary>
        /// Gets the root family of this hierarchy
        /// </summary>
        public Familia ObtenerRaiz()
        {
            var actual = this;
            var visitadas = new HashSet<Guid>();

            while (actual.FamiliaPadre != null && !visitadas.Contains(actual.Id))
            {
                visitadas.Add(actual.Id);
                actual = actual.FamiliaPadre;
            }

            return actual;
        }

        /// <summary>
        /// Gets the depth level of this family in the hierarchy (0 = root)
        /// </summary>
        public int ObtenerNivel()
        {
            int nivel = 0;
            var actual = FamiliaPadre;
            var visitadas = new HashSet<Guid> { Id };

            while (actual != null && !visitadas.Contains(actual.Id))
            {
                nivel++;
                visitadas.Add(actual.Id);
                actual = actual.FamiliaPadre;
            }

            return nivel;
        }

        #endregion

        public override string ToString()
        {
            return $"Familia: {Nombre}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Familia other)
                return Id == other.Id;
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}