using System;

namespace kevin.Ioc.IocAttrributes
{
    /// <summary>
    /// 属性的特性标记，主要用于标记属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IocPropertyAttribute : Attribute
    {

    } 

}
