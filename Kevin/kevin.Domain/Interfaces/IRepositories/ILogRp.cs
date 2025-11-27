using kevin.Domain.Entities;
using kevin.Domain.Interface;
using kevin.Domain.Kevin;

namespace kevin.Domain.Interfaces.IRepositories
{
    /// <summary>
    /// 仓储接口
    /// </summary>
    public interface IHttpLogRp : IRepository<THttpLog, long>
    {
        /// <summary>
        /// 请求日志
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="operateRemark"></param>
        /// <returns></returns>
        Task<bool> Add(string operateType, string operateRemark);
    }
}
