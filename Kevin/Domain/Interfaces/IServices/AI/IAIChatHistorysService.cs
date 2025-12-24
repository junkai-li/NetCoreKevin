using kevin.Domain.Share.Dtos.AI;

namespace kevin.Domain.Interfaces.IServices.AI
{
    public interface IAIChatHistorysService
    {
        Task<dtoPageData<AIChatHistorysDto>> GetPageData(dtoPagePar<string> dtoPage);
        Task<AIChatHistorysDto> Add(AIChatHistorysDto par);
        Task<bool> Delete(long id);
    }
}
