using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Base._;

namespace kevin.Domain.Interfaces.IServices
{
    public interface IHttpLogService: IBaseService
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
