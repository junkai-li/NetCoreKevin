using AppServices.Services.v1._;
using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Services.v1
{
    public class AppSubscribeService : BaseService,IAppSubscribeService
    {
        [CapSubscribe("AppSubscribeShowMessage")]
        public void AppSubscribeShowMessage(string message)
        {
            var inst = int.Parse(message);
            Task.Run(() =>
            {
                Console.WriteLine("AppSubscribeShowMessage" + DateTime.Now + inst.ToString());
            });

        }
    }
}
