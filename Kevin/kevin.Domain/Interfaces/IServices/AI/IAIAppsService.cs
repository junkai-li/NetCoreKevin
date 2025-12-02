using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.AI;
using kevin.Share.Dtos;

namespace kevin.Domain.Interfaces.IServices.AI
{ 
    public interface IAIAppsService : IBaseService
    { 
        Task<dtoPageData<AIAppsDto>> GetPageData(dtoPagePar<string> dtoPage);

        Task<List<AIAppsDto>> GetALLList();
        Task<bool> AddEdit(AIAppsDto par);
        Task<bool> Delete(long id);
    }
}
