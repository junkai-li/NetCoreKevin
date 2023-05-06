using App.RepositorieRps.Repositories.v1;
using AppServices.Services.v1._;
using DotNetCore.CAP;
using kevin.Domain.Kevin;
using Kevin.Web.Attributes.IocAttrributes.IocAttrributes;
using Microsoft.AspNetCore.Http;
using Repository.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Services.v1
{ 
    public class TestService : BaseService, ITestService
    {
        //public ICapPublisher capPublisher { get; set; } 

        public ILogRp logRp { get; set; }
        public TestService(IHttpContextAccessor _httpContextAccessor) : base(_httpContextAccessor)
        {
             
        }

        public async Task<string> SendSubsMsg(string msg)
        {
            //var _transaction = db.Database.BeginTransaction(capPublisher);
            //try
            //{
            //    msg = msg + msg;
            //   await capPublisher.PublishAsync("ShowMessage", msg);
            //    await capPublisher.PublishAsync("AppSubscribeShowMessage", msg);
            //    await _transaction.CommitAsync();
            //}
            //catch (Exception ex)
            //{ 
            //    _transaction.Rollback();
            //    return ex.Message;
            //}   
            return "Ok";
        }
        public async Task<string> SendTLogEvent(string msg)
        { 
            var log = new TLog("测试", "Type", msg);
            log.CreateTime = DateTime.Now;
            logRp.Add(log);
            logRp.SaveChanges();
            return "Ok";
        }
    }
}
