using kevin.Domain.Entities;
using kevin.Domain.Interfaces.IRepositories;
using kevin.Domain.Kevin;
using Kevin.Common.App;
using Kevin.EntityFrameworkCore._.Data;
using Kevin.SnowflakeId.Service;
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
    public class HttpLogRp : Repository<THttpLog, long>, IHttpLogRp
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
            log.Id = SnowflakeIdService.GetNextId();
            log.CreateTime = DateTime.Now;
            log.IsDelete = false;
            log.CreateUserId = log.Id;
            log.UserName = "未知";
            log.TenantId = CurrentUser.TenantId;
            if (CurrentUser != default)
            {
                log.CreateUserId = CurrentUser.UserId.ToTryInt64() > 0 ? CurrentUser.UserId.ToTryInt64() : log.Id.ToTryInt64();
                log.UserName = CurrentUser.UserName ?? "未知";
            }
            log.OperateRemark = operateRemark;
            log.OperateType = operateType;
            log.IP = ServiceProvider.GetService<IHttpContextAccessor>().GetIpAddress();
            log.HttpBody = ServiceProvider.GetService<IHttpContextAccessor>().GetRequestBody();
            log.HttpAction = ServiceProvider.GetService<IHttpContextAccessor>().GetUrlAction();
            log.HttpUrl = ServiceProvider.GetService<IHttpContextAccessor>().GetUrl();
            log.Device = ServiceProvider.GetService<IHttpContextAccessor>().GetDevice();
            log.HttpMethod = ServiceProvider.GetService<IHttpContextAccessor>().Current().Request.Method;
            DbSet.Add(log);
            SaveChangesAsync();
            return Task.FromResult(true);
        }
    }
}
