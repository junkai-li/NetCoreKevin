using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.Organizational;
using kevin.Share.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Interfaces.IServices.Organizational
{
    public interface IPositionService : IBaseService
    {

        Task<PositionDto> GetPositionTree();
        Task<dtoPageData<PositionDto>> GetPageData(dtoPagePar<string> par);

        Task<List<PositionDto>> GetALLList();

        List<long> GetChildIdList(long id);

        Task<bool> AddEdit(PositionDto data);

        Task<bool> Delete(long id);

        Task<bool> AddEditUserBindPosition(long userId, List<long> positionIds);
    }
}
