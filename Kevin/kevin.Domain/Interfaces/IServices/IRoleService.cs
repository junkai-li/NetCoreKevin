using kevin.Share.Dtos.System;

namespace kevin.Domain.Interfaces.IServices
{
    public interface IRoleService : IBaseService
    {
        Task<dtoPageData<dtoRole>> GetRolePage(dtoPageData<dtoRole> dtoPage);
        Task<bool> AddEidt(dtoRole dtoRole);
        Task<bool> DeleteRole(long roleId);
        Task<dtoRole> GetRoleById(long roleId);
        Task<List<dtoRole>> GetAllRoles();
    }
}
