using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppServices.Services.v1._
{
    public interface ITestService
    {
        Task<string> SendSubsMsg(string msg);

        Task<string> SendTLogEvent(string msg);
    }
}
