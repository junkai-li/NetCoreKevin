using Aop.Api.Domain;
using kevin.Share.Dtos;
using kevin.Share.Dtos.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Interfaces.IServices
{
    public interface IRoleService : IBaseService
    {
        Task<dtoPageData<dtoRole>> GetRolePage(dtoPageData<dtoRole> dtoPage);
        Task<bool> AddEidt(dtoRole dtoRole);
        Task<bool> DeleteRole(Guid roleId);
        Task<dtoRole> GetRoleById(Guid roleId);
        Task<List<dtoRole>> GetAllRoles();
    }
}
