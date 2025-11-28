using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.Organizational;
using kevin.Share.Dtos;

namespace kevin.Domain.Interfaces.IServices.Organizational
{
    public interface IDepartmentService : IBaseService
    {
        Task<dtoPageData<DepartmentDto>> GetPageData(dtoPagePar<string> par);
        Task<DepartmentDto> GetDepartmentTree();

        Task<List<DepartmentDto>> GetALLList();

        List<long> GetChildIdList(long id);

        List<long> GetChildUserIds(long id);
        Task<bool> AddEdit(DepartmentDto data);

        Task<bool> Delete(long id);
    }
}
