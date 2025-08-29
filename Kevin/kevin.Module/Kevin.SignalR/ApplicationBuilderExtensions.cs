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
        public static void UseKevinSignalR(this IApplicationBuilder app, Action<SignalrSetting> action)
        {
            var signalrSetting = new SignalrSetting();
            action.Invoke(signalrSetting);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<MySignalRHub>(signalrSetting.url);
            });

        }
    }
}
