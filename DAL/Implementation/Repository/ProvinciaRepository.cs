using DAL.Infrastructure;
using DomainModel.Contract;
using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Implementation.Repository
{
    /// <summary>
    /// Implementación del repositorio de Provincia
    /// </summary>
    public class ProvinciaRepository : Repository<ProvinciaDM>, IProvinciaRepository
    {
        public ProvinciaRepository(EntityFramework.SistemaPresupuestario context)
            : base(context)
        {
        }

        public ProvinciaDM GetByCodigoAFIP(string codigoAFIP)
        {
            var provinciaEF = _context.Provincia
                .FirstOrDefault(p => p.CodigoAFIP == codigoAFIP);

            if (provinciaEF == null)
                return null;

            return MapearDesdeDatos(provinciaEF);
        }

        public IEnumerable<ProvinciaDM> GetAllOrdenadas()
        {
            return _context.Provincia
                .OrderBy(p => p.CodigoAFIP)
                .ToList()
                .Select(MapearDesdeDatos);
        }

        // MÉTODOS DE MAPEO PRIVADOS (no override)

        private ProvinciaDM MapearDesdeDatos(EntityFramework.Provincia provinciaEF)
        {
            if (provinciaEF == null) return null;

            return new ProvinciaDM(
                provinciaEF.ID,
                provinciaEF.CodigoAFIP,
                provinciaEF.Nombre
            );
        }

        private EntityFramework.Provincia MapearHaciaDatos(ProvinciaDM entidadDominio)
        {
            var provinciaEF = _context.Provincia.Find(entidadDominio.Id);

            if (provinciaEF == null)
            {
                provinciaEF = new EntityFramework.Provincia
                {
                    ID = entidadDominio.Id
                };
                _context.Provincia.Add(provinciaEF);
            }

            provinciaEF.CodigoAFIP = entidadDominio.CodigoAFIP;
            provinciaEF.Nombre = entidadDominio.Nombre;

            return provinciaEF;
        }
    }
}
