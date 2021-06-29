using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Cms.Libraries.Ueditor
{

    /// <summary>
    /// Config 的摘要说明
    /// </summary>
    public class ConfigHandler : Handler
    {
        public ConfigHandler(HttpContext context) : base() { }

        public override string Process()
        {
          return  WriteJson(Config.Items);
        }
    }

}