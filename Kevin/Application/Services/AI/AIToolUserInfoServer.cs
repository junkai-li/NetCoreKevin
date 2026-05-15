using kevin.AI.AgentFramework.Interfaces;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.RepositorieRps.Repositories.AI;
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
    }
}
