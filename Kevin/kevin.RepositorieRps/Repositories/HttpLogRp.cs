using kevin.Domain.Entities;
using kevin.Domain.Interfaces.IRepositories;
using kevin.Domain.Kevin;
using Kevin.Common.App;
using Kevin.EntityFrameworkCore._.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.RepositorieRps.Repositories
{
    public class HttpLogRp : Repository<THttpLog, Guid>, IHttpLogRp
    {
        public HttpLogRp(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }
        /// <summary>
        /// 请求日志
        /// </summary>
        /// <param name="operateType"></param>
        /// <param name="operateRemark"></param>
        /// <returns></returns>
        public Task<bool> Add(string operateType, string operateRemark)
        {
            var log = new THttpLog();
            log.Id = Guid.NewGuid();
            log.CreateTime = DateTime.Now;
            log.IsDelete = false;
            log.CreateUserId = log.Id;
            log.user_name = "未知";
            log.TenantId = CurrentUser.TenantId;
            if (CurrentUser != default)
            {
                log.CreateUserId = Guid.Parse(CurrentUser.UserId.ToString() ?? log.Id.ToString());
                log.user_name = CurrentUser.UserName ?? "未知";
            }
            log.operate_remark = operateRemark;
            log.operate_type = operateType;
            log.ip = ServiceProvider.GetService<IHttpContextAccessor>().GetIpAddress();
            log.http_body = ServiceProvider.GetService<IHttpContextAccessor>().GetRequestBody();
            log.http_action = ServiceProvider.GetService<IHttpContextAccessor>().GetUrlAction();
            log.http_url = ServiceProvider.GetService<IHttpContextAccessor>().GetUrl();
            log.device = ServiceProvider.GetService<IHttpContextAccessor>().GetDevice();
            log.http_method = ServiceProvider.GetService<IHttpContextAccessor>().Current().Request.Method;
            DbSet.Add(log);
            SaveChangesAsync();
            return Task.FromResult(true);
        }
    }
}
