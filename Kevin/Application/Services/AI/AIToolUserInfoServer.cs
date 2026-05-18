using kevin.AI.AgentFramework.Interfaces;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.RepositorieRps.Repositories.AI;
using Kevin.Common.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Application.Services.AI
{
    public class AIToolUserInfoServer : BaseService, IAIToolUserInfoServer
    {
        public AIToolUserInfoServer(IHttpContextAccessor _httpContextAccessor) : base(_httpContextAccessor) { }
        public Task<string> GetUserIdAsync()
        {
            return Task.FromResult(CurrentUser.UserId.ToString());
        }

        public Task<string> GetUserInfo()
        {
            return Task.FromResult($"""
                用户Id(字段名为：UserId)：{CurrentUser.UserId} ,
                用户名(字段名为：UserName)：{CurrentUser.UserName},
                当前登录用户租户code(字段名为：TenantId)：{CurrentUser.TenantId}
                超级管理员(字段名为：IsSuperAdmin)：{CurrentUser.IsSuperAdmin}
                其他信息(字段名为：UserInfo)：{CurrentUser.UserInfo.ToJson()}
                """);
        }
    }
}
