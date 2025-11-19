using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using Kevin.Web.Filters.TransactionScope.Attribute;

namespace Kevin.Web.Filters.TransactionScope
{
    /// <summary>
    /// 自动启用事务的筛选器
    /// </summary>
    public class TransactionScopeFilter : IAsyncActionFilter
    {
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
                using var txScope = new System.Transactions.TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
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
