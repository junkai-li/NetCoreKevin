using System;

namespace Kevin.Web.Filters.TransactionScope.Attribute
{
    /// <summary>
    ///启动事务
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TransactionalAttribute : System.Attribute
    {
    }
}
