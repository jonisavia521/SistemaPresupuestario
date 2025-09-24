using System;
using System.Collections.Generic;

namespace SistemaPresupuestario.DomainModel.Seguridad
{
    /// <summary>
    /// Interface for implementing Composite pattern for permissions hierarchy
    /// Allows both Familia and Patente to be treated uniformly in permission trees
    /// </summary>
    public interface IComponentePermiso
    {
        /// <summary>
        /// Unique identifier of the permission component
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Display name of the permission component
        /// </summary>
        string Nombre { get; set; }

        /// <summary>
        /// Gets all direct patent permissions associated with this component
        /// For Patente: returns itself
        /// For Familia: returns all direct patents assigned to this family
        /// </summary>
        /// <returns>Collection of patent permissions</returns>
        IEnumerable<Patente> ObtenerPatentesDirectas();

        /// <summary>
        /// Gets all effective patent permissions including inherited ones
        /// For Patente: returns itself
        /// For Familia: returns all patents from this family and all child families recursively
        /// </summary>
        /// <returns>Collection of all effective patent permissions</returns>
        IEnumerable<Patente> ObtenerPatentesEfectivas();

        /// <summary>
        /// Adds a child component to this component (for Familia only)
        /// </summary>
        /// <param name="componente">Component to add</param>
        void AgregarComponente(IComponentePermiso componente);

        /// <summary>
        /// Removes a child component from this component (for Familia only)
        /// </summary>
        /// <param name="componente">Component to remove</param>
        void EliminarComponente(IComponentePermiso componente);

        /// <summary>
        /// Gets all direct child components (for Familia only)
        /// </summary>
        /// <returns>Collection of child components</returns>
        IEnumerable<IComponentePermiso> ObtenerHijos();
    }
}