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
    public interface IAIChatsService
    {
        Task<dtoPageData<AIChatsDto>> GetMyPageData(dtoPagePar<string> dtoPage);
        Task<bool> Add(AIChatsDto par);
        Task<bool> Delete(long id);
    }
}
