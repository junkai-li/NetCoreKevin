using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskAdmin.Tasks
{
    public static class Main
    {

        public static void Run()
        {
            //仅执行一次，创建后立即执行
            var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("Fire-and-forget!"));

            //仅执行一次，但是不会立即执行，再指定的时间之后执行
            jobId = BackgroundJob.Schedule(() => Console.WriteLine("Delayed!"), TimeSpan.FromDays(7));


            //按照指定的时间进行循环执行
            RecurringJob.AddOrUpdate(() => Run(), "*/5 * * * * *");


            //延续执行，在其父项作业完成后执行
            BackgroundJob.ContinueJobWith(jobId, () => Console.WriteLine("Continuation!"));
        }
    }
}
