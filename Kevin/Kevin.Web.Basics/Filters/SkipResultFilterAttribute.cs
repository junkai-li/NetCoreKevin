namespace Web.Filters
{
    /// <summary>
    /// 标记控制器或方法跳过 ResultFilter 的全局包装，
    /// 适用于需要原样返回响应体的场景（如接口转发代理）
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class SkipResultFilterAttribute : Attribute
    {
    }
}
