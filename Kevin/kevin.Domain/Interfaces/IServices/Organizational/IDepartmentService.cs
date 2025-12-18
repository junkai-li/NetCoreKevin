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

        Task<List<DepartmentDto>> GetALLList(List<long> Ids);
        /// <summary>
        /// 获取某个部门下面的所有部门ids
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<long> GetChildIdList(long id);

        /// <summary>
        /// 获取某个部门下面的所有用户ids
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<long> GetDepartmentChildUserIds(long id);
        Task<bool> AddEdit(DepartmentDto data);

        Task<bool> Delete(long id);
        /// <summary>
        /// 获取某个部门的用户ids
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<long>> GetDepartmentUserIds(long id);
    }
}
