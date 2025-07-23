using System;

namespace kevin.Ioc.IocAttrributes
{

    /// <summary>
    /// 方法的特性标记，主要用于标记方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class IocMethodAttribute : Attribute
    {
    }
}
