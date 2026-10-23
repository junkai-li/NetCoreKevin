using Kevin.log4Net;
using Kevin.Web.Filters.TransactionScope.Attribute;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Repository.Database;
using System.Data;
using System.Reflection;

namespace Kevin.Web.Filters.TransactionScope
{
    /// <summary>
    /// 自动启用事务的筛选器
    /// </summary>
    /// <remarks>
    /// 用 EF 的数据库本地事务（开在请求作用域的 <see cref="KevinDbContext"/> 上）替代 System.Transactions 的环境事务：
    /// 环境事务一旦在请求里出现第二条连接（项目里有多处 new KevinDbContext()），MySqlConnector 会把它升级成
    /// XA 分布式事务；提交阶段 XA PREPARE 失败会抛 TransactionAbortedException，并可能在 MySQL 服务端留下
    /// 未回滚的 XA 分支持续持锁，表现为后续写操作 Lock wait timeout exceeded。
    /// 隔离级别取 ReadCommitted：普通 SELECT 走 MVCC 不加锁、也没有间隙锁，锁只落在真正被写的行上。
    /// </remarks>
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
            if (!hasTransactionalAttribute)
            {
                await next();
                return;
            }

            //事务必须开在仓储共用的那个 scoped DbContext 上，否则 SaveChanges 不在事务内
            var db = context.HttpContext.RequestServices.GetService<KevinDbContext>();
            if (db == default || db.Database.CurrentTransaction != default)
            {
                //未注册领域库，或调用链上已经开过事务：不重复开
                await next();
                return;
            }

            //BeginTransaction 会占用这条连接直到提交/回滚；释放时自动回滚，不会把未提交的行锁带出请求
            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                var result = await next();
                if (result.Exception == default && !context.HttpContext.RequestAborted.IsCancellationRequested)
                {
                    //传 None：客户端断开不应阻止已经成功的写落库
                    await transaction.CommitAsync(CancellationToken.None);
                }
                else
                {
                    await RollbackAsync(transaction);
                }
            }
            catch
            {
                await RollbackAsync(transaction);
                throw;
            }
        }

        /// <summary>
        /// 回滚。连接已被服务端断开时回滚会再抛异常，这里只记日志，避免覆盖真正的业务异常
        /// </summary>
        private static async Task RollbackAsync(IDbContextTransaction transaction)
        {
            try
            {
                await transaction.RollbackAsync(CancellationToken.None);
            }
            catch (Exception ex)
            {
                LogHelper<TransactionScopeFilter>.logger.Error("事务回滚失败", ex);
            }
        }
    }
}
