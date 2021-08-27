using Web.Permisson.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Permisson.Extensions
{
    public static class ActionDescriptionExtension
    {
        public static string GetDescription(this ActionDescriptionAttribute self, ControllerBase controller)
        {
            return self.GetDescription(controller.GetType());
        }
        /// <summary>
        /// 内置模块
        /// </summary>
        /// <param name="self"></param>
        /// <param name="controllertype"></param>
        /// <returns></returns>
        public static string GetDescription(this ActionDescriptionAttribute self, Type controllertype)
        {
            string rv = "";
            if (string.IsNullOrEmpty(self.Description) == false)
            {
                // TODO: 内置模块多语言
                rv = self.Description;
            }
            return rv;

        }

    }
}
