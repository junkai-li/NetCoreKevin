using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiService.Permisson.Attributes
{
    /// <summary>
    /// 标记Controller或Action调试模式，只有调试模式才允许访问
    /// Mark Controller or action with debug, only debug mode can be access.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class DebugOnlyAttribute : Attribute
    {
    }
}
