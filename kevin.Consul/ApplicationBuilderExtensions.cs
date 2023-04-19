using Consul;
using kevin.Consul.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Consul
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseKevinConsul(this IApplicationBuilder app, ConsulSetting setting, IHostApplicationLifetime lifetime)
        {
            var consulClient = new ConsulClient(c =>
            {
                //consul地址
                c.Address = new Uri(setting.ConsulAddress);
            });

            var registration = new AgentServiceRegistration()
            {
                ID = Guid.NewGuid().ToString(),//服务实例唯一标识
                Name = setting.ServiceName,//服务名
                Address = setting.ServiceIP, //服务IP
                Port = int.Parse(setting.ServicePort),//服务端口 因为要运行多个实例，端口不能在appsettings.json里配置，在docker容器运行时传入
                Check = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
                    Interval = TimeSpan.FromSeconds(10),//健康检查时间间隔
                    HTTP = $"http://{setting.ServiceIP}:{setting.ServicePort}{setting.ServiceHealthCheck}",//健康检查地址
                    Timeout = TimeSpan.FromSeconds(5)//超时时间
                }
            }; 
            //服务注册
            consulClient.Agent.ServiceRegister(registration).Wait(); 
            //应用程序终止时，取消注册
            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });

        }
    }
}
