using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;


namespace Repository.Interceptors
{
    public class DeBugInterceptor : DbCommandInterceptor
    {

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            return result;
        }





        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {

            var runtime = eventData.Duration.TotalSeconds;

            //如果执行时间超过 5秒 则记录日志
            if (runtime > 5)
            {
                System.Diagnostics.Debug.WriteLine($"[慢查询警告] 执行耗时 {runtime:F2}秒, SQL: {command.CommandText}");
                Console.WriteLine($"[慢查询警告] 执行耗时 {runtime:F2}秒, SQL: {command.CommandText}");
            }

            return result;
        }

    }
}
