using Microsoft.AspNetCore.Mvc;
using Repository.Database;
using Models.Extension;
using Web.Global.User;
using Service.Services.v1;
using Medallion.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Global;
using Web.Libraries;
using Kevin.Web.Attributes.IocAttrributes.IocAttrributes;

namespace WebApi.Controllers.Bases
{
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        [IocProperty]
        public dbContext db { get; set; } 
        [IocProperty]
        public ICurrentUser CurrentUser { get; set; }
        [IocProperty]
        public IConfiguration Configuration { get; set; }
        [IocProperty]
        public IDistributedLockProvider distLock { get; set; }
        [IocProperty]
        public IDistributedSemaphoreProvider distSemaphoreLock { get; set; }
        [IocProperty]
        public IDistributedUpgradeableReaderWriterLockProvider distUpgradeableLock { get; set; }
    }
}
