using kevin.Domain.Share.Dtos.System;
using kevin.Share.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Interfaces.IServices
{
    public interface IOSLogService : IBaseService
    {
        /// <summary>
        /// 获取OS日志
        /// </summary>
        /// <param name="dtoPage"></param>
        /// <returns></returns>
        Task<dtoPageData<OSLogDto>> GetPageData(dtoPageData<OSLogDto> dtoPage);
    }
}
