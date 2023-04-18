using DotNetCore.CAP.Filter;

namespace kevin.Cap.Filter
{

    /// <summary>
    /// Cap订阅服务过滤器
    /// </summary>
    public class CapSubscribeFilter : SubscribeFilter
    {


        /// <summary>
        /// 订阅方法执行前
        /// </summary>
        /// <param name="context"></param>
        public override void OnSubscribeExecuting(ExecutingContext context)
        {

        }



        /// <summary>
        /// 订阅方法执行后
        /// </summary>
        /// <param name="context"></param>
        public override void OnSubscribeExecuted(ExecutedContext context)
        {

        }



        /// <summary>
        /// 订阅方法执行异常
        /// </summary>
        /// <param name="context"></param>
        public override void OnSubscribeException(ExceptionContext context)
        {

        }


    }
}
