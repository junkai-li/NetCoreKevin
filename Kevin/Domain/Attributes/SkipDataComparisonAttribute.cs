using System;
using System.Collections.Generic;
using System.Text;

namespace kevin.Domain.Attributes
{
    /// <summary>
    /// 用于跳过数据比对
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class SkipDataComparisonAttribute : Attribute
    {

    }
}
