using Kevin.Common.Helper;
using System;

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
