using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Permission.Permission.Attributes
{
    public class MyModuleAttribute : Attribute
    {
        public string ModuleName { get; set; }
        public string Module { get; set; }
        public MyModuleAttribute(string moduleName)
        {
            this.ModuleName = moduleName;
        }
        public MyModuleAttribute(string moduleName, string module)
        {
            this.Module = module;
            this.ModuleName = moduleName;
        }
    }
}
