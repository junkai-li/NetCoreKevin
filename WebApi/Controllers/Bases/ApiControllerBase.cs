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
namespace WebApi.Controllers.Bases
{
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        public readonly dbContext db = GlobalServices.Get<dbContext>() ?? Web.Libraries.Http.HttpContext.Current().RequestServices.GetService<dbContext>();

        public readonly ICurrentUser CurrentUser = GlobalServices.Get<ICurrentUser>() ?? Web.Libraries.Http.HttpContext.Current().RequestServices.GetService<ICurrentUser>();

        public readonly IConfiguration Configuration = GlobalServices.Get<IConfiguration>() ?? Web.Libraries.Http.HttpContext.Current().RequestServices.GetService<IConfiguration>();

        public readonly IDistributedLockProvider distLock = GlobalServices.Get<IDistributedLockProvider>() ?? Web.Libraries.Http.HttpContext.Current().RequestServices.GetService<IDistributedLockProvider>();

        public readonly IDistributedSemaphoreProvider distSemaphoreLock = GlobalServices.Get<IDistributedSemaphoreProvider>() ?? Web.Libraries.Http.HttpContext.Current().RequestServices.GetService<IDistributedSemaphoreProvider>();

        public readonly IDistributedUpgradeableReaderWriterLockProvider distUpgradeableLock = GlobalServices.Get<IDistributedUpgradeableReaderWriterLockProvider>() ?? Web.Libraries.Http.HttpContext.Current().RequestServices.GetService<IDistributedUpgradeableReaderWriterLockProvider>();
    }
}
