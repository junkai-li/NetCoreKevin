using Microsoft.AspNetCore.Mvc;
using Repository.Database;
using Models.Extension; 
using Web.Global.User;
using Service.Services.v1;
using Medallion.Threading; 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Controllers.Bases
{
    public class PubilcControllerBase : ControllerBase
    {
        public readonly dbContext db = Web.Libraries.Http.HttpContext.Current().RequestServices.GetService<dbContext>();

        public readonly ICurrentUser CurrentUser = Web.Libraries.Http.HttpContext.Current().RequestServices.GetService<ICurrentUser>();

        public readonly IConfiguration Configuration = Web.Libraries.Http.HttpContext.Current().RequestServices.GetService<IConfiguration>();
        public readonly IDistributedLockProvider distLock = Web.Libraries.Http.HttpContext.Current().RequestServices.GetService<IDistributedLockProvider>();

        public readonly IDistributedSemaphoreProvider distSemaphoreLock = Web.Libraries.Http.HttpContext.Current().RequestServices.GetService<IDistributedSemaphoreProvider>();

        public readonly IDistributedUpgradeableReaderWriterLockProvider distUpgradeableLock = Web.Libraries.Http.HttpContext.Current().RequestServices.GetService<IDistributedUpgradeableReaderWriterLockProvider>();
    }
}
