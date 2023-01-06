using System;

namespace Kevin.Web.Filters.TransactionScope.Attribute
{
    /// <summary>
    /// 不启动
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class NotTransactionalAttribute : System.Attribute
    {
    }
}
