using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.AI;
using kevin.Share.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Interfaces.IServices.AI
{
    public interface IAIChatHistorysService
    {
        Task<dtoPageData<AIChatHistorysDto>> GetPageData(dtoPagePar<string> dtoPage);
        Task<bool> Add(AIChatHistorysDto par);
        Task<bool> Delete(long id);
    }
}
