using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace TaskService
{
    class Program
    {


        static void Main(string[] args)
        {
            Common.EnvironmentHelper.InitTestServer();

            //为各数据库注入连接字符串
            Repository.Database.dbContext.ConnectionString = Common.IO.Config.Get().GetConnectionString("dbConnection");


            //注入依赖服务
            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);


            //获取所有服务
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetService<Tasks.DemoTask>().Run();


            Console.WriteLine("启动成功，输入 exit 回车后停止！");
            bool end = true;
            do
            {
                var read = Console.ReadLine();

                if (read == "exit")
                {
                    end = false;
                }
            } while (end);

        }




        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<Repository.Database.dbContext>(options => { }, 100);

            //注册要执行的Task
            services.AddSingleton<Tasks.DemoTask>();


        }

    }
}
