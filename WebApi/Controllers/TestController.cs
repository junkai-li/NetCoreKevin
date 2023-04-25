using AppServices.Services.v1._;
using kevin.Cache.Service;
using Kevin.Web.Attributes.IocAttrributes.IocAttrributes;
using Medallion.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services.v1._;
using System;
using System.Threading.Tasks;
using WebApi.Controllers.Bases;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TestController : ApiControllerBase
    {
        [IocProperty]
        public IUserService _userService { get; set; }

        [IocProperty]
        public ICacheService _cacheService { get; set; }
        [IocProperty]
        public ITestService _TestService { get; set; }

        [HttpGet("TestUserService")]
        public object TestUserService()
        {
            _userService.GetUser(new Guid());
            return default;
        }
        [HttpGet("TestCacheServiceOk")]
        public string TestCacheServiceOk()
        {
            _cacheService.SetString("1", "ok");
            return _cacheService.GetString("1");
        }

        [HttpGet("TsetDynamicExpresso")]
        public bool TsetDynamicExpresso(string str)
        {
            var interpreter = new DynamicExpresso.Interpreter();
            return interpreter.Eval<bool>(str);
        }
        /// <summary>
        /// 健康检查接口
        /// </summary>
        /// <returns></returns>
        [HttpGet("HealthCheckGet")]
        public IActionResult HealthCheckGet()
        {
            return Ok();
        }

        /// <summary>
        /// 分布式锁demo
        /// </summary>
        /// <returns></returns>
        [HttpGet("DistLock")]
        public bool DistLock()
        {

            //互斥锁
            using (distLock.AcquireLock(""))
            {
            }

            //互斥锁，可配置内部代码最多同时运行数
            using (distSemaphoreLock.AcquireSemaphore("", 5))
            {
            }

            //读锁，多人同读，与写锁互斥
            using (distUpgradeableLock.AcquireReadLock(""))
            {
            }


            //写锁，互斥
            using (distUpgradeableLock.AcquireWriteLock(""))
            {
            }

            //可升级读锁，初始状态为读锁，可手动升级为写锁
            using (var handle = distUpgradeableLock.AcquireUpgradeableReadLock(""))
            {

                //升级写锁
                handle.UpgradeToWriteLock();
            }

            return true;
        }

        /// <summary>
        /// 分布式锁demo
        /// </summary>
        /// <returns></returns>
        [HttpGet("AcquireLock")]
        public bool AcquireLock()
        {

            //互斥锁
            try
            {
                var datalock = distLock.AcquireLock("1", TimeSpan.FromSeconds(60)); 
                // 锁创建成功
                Console.WriteLine("OK to acquire lock.");
                return true;
            }
            catch (Exception)
            { 
                // 锁创建失败
                Console.WriteLine("Failed to acquire lock.");
                return false;
            }
        }



        /// <summary>
        ///测试提醒异常
        /// </summary>
        /// <param name="str"></param>
        /// <exception cref="UserFriendlyException"></exception>
        [HttpGet("TestErrMsg")]
        public void TestErrMsg(string str)
        {
            throw new UserFriendlyException(str);
        }

        [HttpGet("SendSubsMsg")]
        public Task<string> SendSubsMsg(string msg)
        {
            return _TestService.SendSubsMsg(msg);
        }
    }
}
