using kevin.Domain.Share.Dtos.AI;

namespace kevin.Domain.Interfaces.IServices.AI
{
    public interface IAIChatsService
    {
        Task<dtoPageData<AIChatsDto>> GetMyPageData(dtoPagePar<string> dtoPage);
        Task<AIChatsDto> GetDetails(long id);
        Task<AIChatHistorysDto> Add(AIChatsDto par);
        Task<bool> Delete(long id);

        Task<bool> UpdateNameAndMsg(long Id, string Name, string LastMessage);
    }
}
