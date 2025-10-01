using BLL.Contracts.Seguridad;
using BLL.DTOs.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.Seguridad
{
    public class FamiliaBusinessService : IFamiliaBusinessService
    {
        public Task<IEnumerable<PatenteDto>> CalcularPatentesEfectivasAsync(Guid familiaId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FamiliaDto>> GetAllFamiliasHierarchicalAsync()
        {
            return Task.FromResult<IEnumerable<FamiliaDto>>(Enumerable.Empty<FamiliaDto>());
        }

        public Task<IEnumerable<FamiliaDto>> GetAllFamiliasWithAssignmentStatusAsync(Guid usuarioId)
        {
            throw new NotImplementedException();
        }

        public Task<FamiliaDto> GetFamiliaAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FamiliaDto>> GetFamiliasByUsuarioAsync(Guid usuarioId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateNoCyclesAsync(Guid familiaId, IEnumerable<Guid> familiasPadre)
        {
            throw new NotImplementedException();
        }
    }
}
