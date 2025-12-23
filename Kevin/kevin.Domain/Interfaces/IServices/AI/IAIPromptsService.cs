using kevin.Domain.Share.Dtos.AI;

namespace kevin.Domain.Interfaces.IServices.AI
{
    public interface IAIPromptsService : IBaseService
    {
        Task<dtoPageData<AIPromptsDto>> GetPageData(dtoPagePar<string> dtoPage);
        Task<bool> AddEdit(AIPromptsDto par);
        Task<bool> Delete(long id);
        Task<AIPromptsDto> GetDetails(long id);
        Task<List<AIPromptsDto>> GetALLList();
    }
}
