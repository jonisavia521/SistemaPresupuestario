using System;
using System.Collections.Generic;

namespace DomainModel.Domain.Seguridad
{
    /// <summary>
    /// Entidad de dominio para Patentes (permisos individuales)
    /// PATRÓN COMPOSITE: Representa una hoja (leaf) que no puede contener otros elementos
    /// </summary>
    public class PatenteDomain : PermisoComponentDomain
    {
        /// <summary>
        /// Vista o formulario asociado a esta patente
        /// </summary>
        public string Vista { get; private set; }

        /// <summary>
        /// Constructor para nueva patente
        /// </summary>
        /// <param name="nombre">Nombre de la patente</param>
        /// <param name="vista">Vista o formulario asociado</param>
        public PatenteDomain(string nombre, string vista = null) : base(Guid.NewGuid(), nombre)
        {
            Vista = vista?.Trim();
        }

        /// <summary>
        /// Constructor para cargar patente existente
        /// </summary>
        /// <param name="id">ID de la patente</param>
        /// <param name="nombre">Nombre de la patente</param>
        /// <param name="vista">Vista o formulario asociado</param>
        /// <param name="versionConcurrencia">Versión para control de concurrencia</param>
        public PatenteDomain(Guid id, string nombre, string vista, byte[] versionConcurrencia) 
            : base(id, nombre, versionConcurrencia)
        {
            Vista = vista?.Trim();
        }

        /// <summary>
        /// IMPLEMENTACIÓN PATRÓN COMPOSITE:
        /// Para una patente (hoja), simplemente se devuelve a sí misma
        /// </summary>
        /// <returns>Esta patente como única elemento</returns>
        public override IEnumerable<PatenteDomain> EnumerarPatentes()
        {
            yield return this;
        }

        /// <summary>
        /// Cuenta las patentes (siempre 1 para una patente individual)
        /// </summary>
        /// <returns>1</returns>
        public override int ContarPatentes()
        {
            return 1;
        }

        /// <summary>
        /// Verifica si contiene una patente específica (solo se compara consigo misma)
        /// </summary>
        /// <param name="patenteId">ID de la patente a buscar</param>
        /// <returns>True si el ID coincide</returns>
        public override bool ContienePatente(Guid patenteId)
        {
            return this.Id == patenteId;
        }

        /// <summary>
        /// Actualiza el nombre de la patente
        /// </summary>
        /// <param name="nuevoNombre">Nuevo nombre</param>
        public void ActualizarNombre(string nuevoNombre)
        {
            if (string.IsNullOrWhiteSpace(nuevoNombre))
                throw new ArgumentException("El nombre no puede estar vacío", nameof(nuevoNombre));

            Nombre = nuevoNombre.Trim();
        }

        /// <summary>
        /// Actualiza la vista asociada
        /// </summary>
        /// <param name="nuevaVista">Nueva vista</param>
        public void ActualizarVista(string nuevaVista)
        {
            Vista = nuevaVista?.Trim();
        }

        /// <summary>
        /// Actualiza la versión de concurrencia
        /// </summary>
        /// <param name="nuevaVersion">Nueva versión</param>
        public void ActualizarVersionConcurrencia(byte[] nuevaVersion)
        {
            VersionConcurrencia = nuevaVersion;
        }

        /// <summary>
        /// Información descriptiva extendida
        /// </summary>
        /// <returns>String descriptivo con vista</returns>
        public override string ToString()
        {
            var vista = string.IsNullOrEmpty(Vista) ? "Sin vista" : Vista;
            return $"Patente: {Nombre} (Vista: {vista}, ID: {Id})";
        }

        /// <summary>
        /// Compara dos patentes por ID
        /// </summary>
        /// <param name="obj">Objeto a comparar</param>
        /// <returns>True si son la misma patente</returns>
        public override bool Equals(object obj)
        {
            if (obj is PatenteDomain otraPatente)
            {
                return this.Id == otraPatente.Id;
            }
            return false;
        }

        /// <summary>
        /// Hash code basado en ID
        /// </summary>
        /// <returns>Hash del ID</returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}