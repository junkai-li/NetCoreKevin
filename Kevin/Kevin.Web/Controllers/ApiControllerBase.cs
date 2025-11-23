using Medallion.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Repository.Database;
using Web.Global.User;

namespace Kevin.Web.Controllers
{
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        [IocProperty]
        public KevinDbContext db { get; set; }
        [IocProperty]
        public ICurrentUser CurrentUser { get; set; }
        [IocProperty]
        public IConfiguration Configuration { get; set; }
        [IocProperty]
        public IDistributedLockProvider distLock { get; set; } 
    }
}
