using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

    /// <summary>
    /// 环境变量操作Helper方法
    /// </summary>
    public static class EnvironmentHelper
    {



        /// <summary>
        /// 初始化TestServer
        /// </summary>
        public static void InitTestServer()
        {
            var testIpList = new List<string>();

            testIpList.Add("x.x.x.x");

            if (testIpList.Count() != 0)
            {
                var localIpList = SystemHelper.GetAllIpAddress();

                foreach (var item in localIpList)
                {
                    if (testIpList.Contains(item))
                    {
                        Environment.SetEnvironmentVariable("NETCORE_ENVIRONMENT", "Test");
                        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
                        break;
                    }
                }
            }
        }



    }
}
