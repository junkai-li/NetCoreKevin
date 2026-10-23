namespace Kevin.Web.Filters.TransactionScope.Attribute
{
    /// <summary>
    /// 启动事务（ReadCommitted）：action 正常返回后统一提交，异常回滚。
    /// 方法体内必须 await 所有返回 Task 的调用，也不要 new KevinDbContext()，否则写不进事务。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TransactionalAttribute : System.Attribute
    {
    }
}
