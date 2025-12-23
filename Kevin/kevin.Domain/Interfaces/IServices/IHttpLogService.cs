using kevin.Domain.Share.Dtos.System;

namespace kevin.Domain.Interfaces.IServices
{
    public interface IHttpLogService: IBaseService
    {

        /// <summary>
        /// 获取操作日志
        /// </summary>
        /// <param name="dtoPage"></param>
        /// <returns></returns>
        Task<dtoPageData<HttpLogDto>> GetPageData(dtoPageData<HttpLogDto> dtoPage);
        /// <summary>
        /// 请求日志
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="operateRemark"></param>
        /// <returns></returns>
        Task<bool> Add(string operateType, string operateRemark);
    }
}
