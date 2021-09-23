using Microsoft.AspNetCore.Mvc;
using Repository.Database;  
using Models.Extension;
using Web.Extension.Autofac;
using Web.Global.User;
using Service.Services.v1;
using Medallion.Threading;

namespace WebApi.Controllers.Bases
{
    public class PubilcControllerBase: ControllerBase
    {
        [Autowired]
        public dbContext db { get; set; }

        [Autowired]
        public ICurrentUser CurrentUser{ get; set; }

        [Autowired]
        public   IDistributedLockProvider distLock { get; set; }

        [Autowired]
        public   IDistributedSemaphoreProvider distSemaphoreLock { get; set; }

        [Autowired]
        public   IDistributedUpgradeableReaderWriterLockProvider distUpgradeableLock { get; set; }
    }
}
