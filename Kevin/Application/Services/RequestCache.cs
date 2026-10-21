using Microsoft.AspNetCore.Http;

namespace kevin.Application
{
    /// <summary>
    /// 请求级缓存：数据存放在 HttpContext.Items 中，同一个请求内的所有 scoped 服务共享，请求结束自动释放。
    /// <para>
    /// 不用 AsyncLocal 的原因：异步方法在 <c>await</c> 之后写入的 AsyncLocal 值不会回传给调用方
    /// （continuation 在独立的 ExecutionContext 中执行，写入不落回调用方上下文），
    /// 而这里需要「过滤器里异步算、Action 里同步读」，所以必须用随请求流转的 HttpContext.Items。
    /// </para>
    /// <para>
    /// 加锁原因：SignalR 同一连接的多次提问会共享握手时的 HttpContext，Items 并非线程安全字典。
    /// </para>
    /// </summary>
    internal static class RequestCache
    {
        private static readonly object _syncRoot = new object();

        public static T? Get<T>(IHttpContextAccessor? accessor, string key) where T : class
        {
            var items = accessor?.HttpContext?.Items;
            if (items == null)
            {
                return null;
            }

            lock (_syncRoot)
            {
                return items.TryGetValue(key, out var value) ? value as T : null;
            }
        }

        public static void Set(IHttpContextAccessor? accessor, string key, object value)
        {
            var items = accessor?.HttpContext?.Items;
            if (items == null)
            {
                return;
            }

            lock (_syncRoot)
            {
                items[key] = value;
            }
        }
    }
}
