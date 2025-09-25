using System;
using System.Collections.Generic;

namespace DomainModel.Domain.Seguridad
{
    /// <summary>
    /// Clase base abstracta para el patrón Composite de permisos
    /// Implementa la interfaz común para Familias y Patentes
    /// </summary>
    public abstract class PermisoComponentDomain
    {
        /// <summary>
        /// Identificador único del componente
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Nombre descriptivo del componente
        /// </summary>
        public string Nombre { get; protected set; }

        /// <summary>
        /// Versión para control de concurrencia
        /// </summary>
        public byte[] VersionConcurrencia { get; protected set; }

        /// <summary>
        /// Constructor protegido para clases derivadas
        /// </summary>
        protected PermisoComponentDomain(Guid id, string nombre)
        {
            Id = id;
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
        }

        /// <summary>
        /// Constructor protegido para clases derivadas con versión
        /// </summary>
        protected PermisoComponentDomain(Guid id, string nombre, byte[] versionConcurrencia)
            : this(id, nombre)
        {
            VersionConcurrencia = versionConcurrencia;
        }

        /// <summary>
        /// Enumera todas las patentes contenidas en este componente
        /// PATRÓN COMPOSITE: Implementación diferente para Familia (recursiva) y Patente (simple)
        /// </summary>
        /// <returns>Lista de patentes únicas</returns>
        public abstract IEnumerable<PatenteDomain> EnumerarPatentes();

        /// <summary>
        /// Cuenta las patentes únicas en este componente
        /// </summary>
        /// <returns>Cantidad de patentes</returns>
        public virtual int ContarPatentes()
        {
            var patentes = new HashSet<Guid>();
            foreach (var patente in EnumerarPatentes())
            {
                patentes.Add(patente.Id);
            }
            return patentes.Count;
        }

        /// <summary>
        /// Indica si este componente contiene una patente específica
        /// </summary>
        /// <param name="patenteId">ID de la patente a buscar</param>
        /// <returns>True si contiene la patente</returns>
        public virtual bool ContienePatente(Guid patenteId)
        {
            foreach (var patente in EnumerarPatentes())
            {
                if (patente.Id == patenteId)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Obtiene información resumida del componente
        /// </summary>
        /// <returns>String descriptivo</returns>
        public override string ToString()
        {
            return $"{GetType().Name}: {Nombre} (ID: {Id})";
        }
    }
}