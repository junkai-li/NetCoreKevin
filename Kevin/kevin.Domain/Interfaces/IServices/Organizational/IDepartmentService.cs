using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.Organizational;
using kevin.Share.Dtos;

namespace kevin.Domain.Interfaces.IServices.Organizational
{
    public interface IDepartmentService : IBaseService
    {
        Task<dtoPageData<DepartmentDto>> GetPageData(dtoPagePar<string> par);

        Task<bool> AddEdit(DepartmentDto data);

        Task<bool> Delete(long id);
    }
}
