using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainModel.Domain.Seguridad
{
    /// <summary>
    /// Entidad de dominio para Familias de permisos
    /// PATRÓN COMPOSITE: Representa un nodo compuesto que puede contener patentes y otras familias
    /// </summary>
    public class FamiliaDomain : PermisoComponentDomain
    {
        private readonly List<FamiliaDomain> _familiasHijas;
        private readonly List<PatenteDomain> _patentesDirectas;

        /// <summary>
        /// Familias hijas (solo lectura desde el exterior)
        /// </summary>
        public IReadOnlyCollection<FamiliaDomain> FamiliasHijas => _familiasHijas.AsReadOnly();

        /// <summary>
        /// Patentes asignadas directamente a esta familia
        /// </summary>
        public IReadOnlyCollection<PatenteDomain> PatentesDirectas => _patentesDirectas.AsReadOnly();

        /// <summary>
        /// Constructor para nueva familia
        /// </summary>
        /// <param name="nombre">Nombre de la familia</param>
        public FamiliaDomain(string nombre) : base(Guid.NewGuid(), nombre)
        {
            _familiasHijas = new List<FamiliaDomain>();
            _patentesDirectas = new List<PatenteDomain>();
        }

        /// <summary>
        /// Constructor para cargar familia existente
        /// </summary>
        /// <param name="id">ID de la familia</param>
        /// <param name="nombre">Nombre de la familia</param>
        /// <param name="versionConcurrencia">Versión para control de concurrencia</param>
        public FamiliaDomain(Guid id, string nombre, byte[] versionConcurrencia) 
            : base(id, nombre, versionConcurrencia)
        {
            _familiasHijas = new List<FamiliaDomain>();
            _patentesDirectas = new List<PatenteDomain>();
        }

        /// <summary>
        /// Agrega una familia hija
        /// </summary>
        /// <param name="familiaHija">Familia a agregar como hija</param>
        public void AgregarFamiliaHija(FamiliaDomain familiaHija)
        {
            if (familiaHija == null)
                throw new ArgumentNullException(nameof(familiaHija));

            if (familiaHija.Id == this.Id)
                throw new ArgumentException("Una familia no puede ser hija de sí misma");

            if (!_familiasHijas.Any(f => f.Id == familiaHija.Id))
            {
                _familiasHijas.Add(familiaHija);
            }
        }

        /// <summary>
        /// Remueve una familia hija
        /// </summary>
        /// <param name="familiaId">ID de la familia a remover</param>
        public void RemoverFamiliaHija(Guid familiaId)
        {
            _familiasHijas.RemoveAll(f => f.Id == familiaId);
        }

        /// <summary>
        /// Agrega una patente directa
        /// </summary>
        /// <param name="patente">Patente a agregar</param>
        public void AgregarPatenteDirecta(PatenteDomain patente)
        {
            if (patente == null)
                throw new ArgumentNullException(nameof(patente));

            if (!_patentesDirectas.Any(p => p.Id == patente.Id))
            {
                _patentesDirectas.Add(patente);
            }
        }

        /// <summary>
        /// Remueve una patente directa
        /// </summary>
        /// <param name="patenteId">ID de la patente a remover</param>
        public void RemoverPatenteDirecta(Guid patenteId)
        {
            _patentesDirectas.RemoveAll(p => p.Id == patenteId);
        }

        /// <summary>
        /// Establece las familias hijas (reemplaza las existentes)
        /// </summary>
        /// <param name="familiasHijas">Lista de familias hijas</param>
        public void EstablecerFamiliasHijas(IEnumerable<FamiliaDomain> familiasHijas)
        {
            _familiasHijas.Clear();
            if (familiasHijas != null)
            {
                foreach (var familia in familiasHijas)
                {
                    AgregarFamiliaHija(familia);
                }
            }
        }

        /// <summary>
        /// Establece las patentes directas (reemplaza las existentes)
        /// </summary>
        /// <param name="patentesDirectas">Lista de patentes directas</param>
        public void EstablecerPatentesDirectas(IEnumerable<PatenteDomain> patentesDirectas)
        {
            _patentesDirectas.Clear();
            if (patentesDirectas != null)
            {
                foreach (var patente in patentesDirectas)
                {
                    AgregarPatenteDirecta(patente);
                }
            }
        }

        /// <summary>
        /// IMPLEMENTACIÓN PATRÓN COMPOSITE:
        /// Enumera recursivamente todas las patentes de esta familia
        /// (directas + de todas las familias hijas)
        /// </summary>
        /// <returns>Enumeración de patentes únicas</returns>
        public override IEnumerable<PatenteDomain> EnumerarPatentes()
        {
            var patentesUnicas = new Dictionary<Guid, PatenteDomain>();

            // Agregar patentes directas
            foreach (var patente in _patentesDirectas)
            {
                patentesUnicas[patente.Id] = patente;
            }

            // Agregar patentes de familias hijas recursivamente
            foreach (var familiaHija in _familiasHijas)
            {
                foreach (var patente in familiaHija.EnumerarPatentes())
                {
                    patentesUnicas[patente.Id] = patente;
                }
            }

            return patentesUnicas.Values;
        }

        /// <summary>
        /// Detecta ciclos en la jerarquía de familias usando DFS
        /// </summary>
        /// <param name="visitadas">Set de familias ya visitadas</param>
        /// <param name="enProceso">Set de familias en el stack de procesamiento actual</param>
        /// <returns>True si se detecta un ciclo</returns>
        public bool TieneCiclos(HashSet<Guid> visitadas = null, HashSet<Guid> enProceso = null)
        {
            visitadas = visitadas ?? new HashSet<Guid>();
            enProceso = enProceso ?? new HashSet<Guid>();

            if (enProceso.Contains(this.Id))
                return true; // Ciclo detectado

            if (visitadas.Contains(this.Id))
                return false; // Ya procesada sin ciclos

            visitadas.Add(this.Id);
            enProceso.Add(this.Id);

            foreach (var familiaHija in _familiasHijas)
            {
                if (familiaHija.TieneCiclos(visitadas, enProceso))
                    return true;
            }

            enProceso.Remove(this.Id);
            return false;
        }

        /// <summary>
        /// Actualizar nombre
        /// </summary>
        /// <param name="nuevoNombre">Nuevo nombre</param>
        public void ActualizarNombre(string nuevoNombre)
        {
            if (string.IsNullOrWhiteSpace(nuevoNombre))
                throw new ArgumentException("El nombre no puede estar vacío", nameof(nuevoNombre));

            Nombre = nuevoNombre.Trim();
        }

        /// <summary>
        /// Actualizar versión de concurrencia
        /// </summary>
        /// <param name="nuevaVersion">Nueva versión</param>
        public void ActualizarVersionConcurrencia(byte[] nuevaVersion)
        {
            VersionConcurrencia = nuevaVersion;
        }
    }
}