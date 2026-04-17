using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Kevin.Hangfire.Models;
using Kevin.Hangfire.TieredServiceRegistration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;

namespace Kevin.Hangfire
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseKevinHangfire(this IApplicationBuilder app, Action<HangFireSetting> action)
        {
            var hangFireSetting = new HangFireSetting();
            action.Invoke(hangFireSetting);
            //设置本地化信息，可实现 固定 Hangfire 管理面板为中文显示
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("zh-CN"),
                SupportedCultures = new[]
                {
                    new CultureInfo("zh-CN")
                },
                SupportedUICultures = new[]
                {
                    new CultureInfo("zh-CN")
                }
            });
            if (hangFireSetting.IsUseHangfireDashboard)
            {
                //启用监控面板 
                app.UseHangfireDashboard(
                    $"/{hangFireSetting.DashboardUrl}", options: new DashboardOptions
                    {
                        DashboardTitle = hangFireSetting.DashboardTitle,
                        Authorization = new[] {new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                {
                    RequireSsl = hangFireSetting.RequireSsl,//需要SSL连接才能访问HangFire Dahsboard。
                    SslRedirect = hangFireSetting.SslRedirect,//是否将所有非SSL请求重定向到SSL URL
                    LoginCaseSensitive = hangFireSetting.LoginCaseSensitive,//区分大小写
                      Users = hangFireSetting.userSetting.Select(s => new BasicAuthAuthorizationUser {
                                Login = s.Login,
                                PasswordClear = s.Password,
                            }).ToArray()//用户
                }), },
                    });//添加hangfire仪表盘中间件, 
            }
            var jobManger = app.ApplicationServices.CreateAsyncScope().ServiceProvider.GetService<IRecurringJobManager>();
            if (jobManger != default)
            {
                app.RunModuleConfigTasks(jobManger, TaskReflectionScheduler.GetAllReferencedAssemblies());//任务调度器，执行所有模块的配置任务 
            }
        }
    }
}
