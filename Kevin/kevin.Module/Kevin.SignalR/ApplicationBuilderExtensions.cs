using Kevin.SignalR.Models;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.SignalR
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseKevinRedisSignalR(this IApplicationBuilder app, Action<SignalrRdisSetting> action)
        {
            var signalrSetting = new SignalrRdisSetting();
            action.Invoke(signalrSetting);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MySignalRHub>(signalrSetting.url);
            });

        }
    }
}
