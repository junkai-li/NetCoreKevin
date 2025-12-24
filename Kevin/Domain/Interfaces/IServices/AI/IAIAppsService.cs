using kevin.Domain.Share.Dtos.AI;

namespace kevin.Domain.Interfaces.IServices.AI
{ 
    public interface IAIAppsService : IBaseService
    { 
        Task<dtoPageData<AIAppsDto>> GetPageData(dtoPagePar<string> dtoPage);
        Task<AIAppsDto> GetDetails(long id);
        Task<List<AIAppsDto>> GetALLList();
        Task<bool> AddEdit(AIAppsDto par);
        Task<bool> Delete(long id);
    }
}
