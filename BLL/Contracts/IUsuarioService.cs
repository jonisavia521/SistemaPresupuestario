using BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Contracts
{
    public interface IUsuarioService
    {
        void Add(UsuarioDTO obj);
        void Update(UsuarioDTO obj);
        void Delete(UsuarioDTO obj);
        Task<IEnumerable<UsuarioDTO>> GetAllAsync();
        Task<UsuarioDTO> GetByIdAsync(Guid id);

        // Métodos extendidos para ABM completo
        Task<UserEditDto> GetUserForEditAsync(Guid id);
        Task<bool> ExistsUserNameAsync(string userName, Guid? excludeId = null);
        Task<Guid> CreateUserWithRelationsAsync(UserEditDto userDto);
        Task UpdateUserWithRelationsAsync(UserEditDto userDto);
        Task DeleteUserAsync(Guid id, byte[] timestamp);
        Task<PermisoEfectivoDto> GetEffectivePermissionsAsync(Guid userId);
        Task<PagedResult<UserEditDto>> GetPagedUsersAsync(string filter, int page, int pageSize);
        
        // Métodos para obtener datos de referencia
        Task<IEnumerable<FamiliaDto>> GetAllFamiliasAsync();
        Task<IEnumerable<PatenteDto>> GetAllPatentesAsync();
        Task<List<FamiliaDto>> GetFamiliasJerarquicasAsync();
    }
}