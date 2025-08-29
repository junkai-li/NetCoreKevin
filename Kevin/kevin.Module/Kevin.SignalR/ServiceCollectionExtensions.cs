using Kevin.SignalR.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
namespace Kevin.SignalR
{
    public static class ServiceCollectionExtensions
    {
        public static void AddKevinSignalR(this IServiceCollection services, Action<SignalrSetting> action)
        {

            services.Configure(action);
            var signalrSetting = new SignalrSetting();
            action.Invoke(signalrSetting);
            services.AddSignalR(options =>
            {
                //客户端发保持连接请求到服务端最长间隔，默认30秒，改成4分钟，网页需跟着设置connection.keepAliveIntervalInMilliseconds = 12e4;即2分钟
                options.ClientTimeoutInterval = TimeSpan.FromMinutes(4);
                //服务端发保持连接请求到客户端间隔，默认15秒，改成2分钟，网页需跟着设置connection.serverTimeoutInMilliseconds = 24e4;即4分钟
                options.KeepAliveInterval = TimeSpan.FromMinutes(2);
            }).AddStackExchangeRedis(op =>
            {
                op.Configuration.ConfigurationChannel = signalrSetting.configurationChannel;
                op.Configuration.DefaultDatabase = 12;
                op.Configuration.AbortOnConnectFail = false;
                op.Configuration.Password = signalrSetting.password;
                op.Configuration.EndPoints.Add(signalrSetting.hostname, int.Parse(signalrSetting.port));
                op.Configuration.ConnectRetry = 3;
                op.Configuration.SyncTimeout = 3000;
            });
        }
    }
}
