using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;


namespace Repository.Interceptors
{
    public class DeBugInterceptor : DbCommandInterceptor
    {

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {

            //执行的Sql
            _ = command.CommandText;

            return result;
        }





        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {

            var runtime = eventData.Duration.TotalSeconds;

            //如果执行时间超过 5秒 则记录日志
            if (runtime > 5)
            {

            }

            return result;
        }

    }
}
