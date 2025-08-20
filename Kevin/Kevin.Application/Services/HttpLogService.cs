using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Application.Services
{
    public class HttpLogService : BaseService, IHttpLogService
    {
        public IHttpLogRp httpLogRp { get; set; }
        public HttpLogService(IHttpContextAccessor _httpContextAccessor, IHttpLogRp _httpLogRp) : base(_httpContextAccessor)
        {
            httpLogRp = _httpLogRp;
        }
        public async Task<bool> Add(string operateType, string operateRemark)
        {
           return await httpLogRp.Add(operateType, operateRemark);
        }
    }
}
