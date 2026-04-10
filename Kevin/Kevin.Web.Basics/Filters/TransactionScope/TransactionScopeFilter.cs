using Kevin.Web.Filters.TransactionScope.Attribute;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace Kevin.Web.Filters.TransactionScope
{
    /// <summary>
    /// 自动启用事务的筛选器
    /// </summary>
    public class TransactionScopeFilter : IAsyncActionFilter
    {
        private readonly int _defaultTimeoutMinutes = 10; // 默认10分钟
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            bool hasTransactionalAttribute = false;
            if (context.ActionDescriptor is ControllerActionDescriptor)
            {
                var actionDesc = (ControllerActionDescriptor)context.ActionDescriptor;
                hasTransactionalAttribute = actionDesc.MethodInfo
                    .IsDefined(typeof(TransactionalAttribute));
            }
            //有TransactionalAttribute才开启事务
            if (hasTransactionalAttribute)
            {
                using var txScope = new System.Transactions.TransactionScope(
                 TransactionScopeOption.Required,
                 TimeSpan.FromMinutes(_defaultTimeoutMinutes),// 设置超时为10分钟，可根据需要调整
                 TransactionScopeAsyncFlowOption.Enabled
             );
                var result = await next();
                if (result.Exception == null)
                {
                    txScope.Complete();
                }
                return;
            }
            await next();
            return;
        }
    }
}
