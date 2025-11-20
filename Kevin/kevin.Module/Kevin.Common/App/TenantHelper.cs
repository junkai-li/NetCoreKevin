using Kevin.Common.Helper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.Common.App
{
    public class TenantHelper
    {
        /// <summary>
        /// GetSettingsTenantId
        /// </summary>
        /// <returns></returns>
        public static string GetSettingsTenantId()
        {
            try
            {  
                return ConfigHelper.GetValue("TenantId");
            }
            catch (Exception)
            {
                return "1000";
            }
        }
    }
}
