using kevin.Domain.Share.Dtos.System;

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
