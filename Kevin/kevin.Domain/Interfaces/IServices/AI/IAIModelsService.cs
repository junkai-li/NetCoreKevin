using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.AI;
using kevin.Share.Dtos;

namespace kevin.Domain.Interfaces.IServices.AI
{
    public interface IAIModelsService : IBaseService
    {
        Task<dtoPageData<AIModelsDto>> GetPageData(dtoPagePar<string> dtoPage);
        Task<bool> AddEdit(AIModelsDto par);
        Task<bool> Delete(long id);

        Task<AIModelsDto> GetDetails(long id);
        Task<List<AIModelsDto>> GetALLList();
    }
}
