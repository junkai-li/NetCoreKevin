using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiService.Permisson.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MyAreaAttribute : Attribute
    {
        public string AreaName { get; set; }
        public string Area { get; set; }

        public MyAreaAttribute(string areaName, string area)
        {
            this.Area = area;
            this.AreaName = areaName;
        }
    }
}
