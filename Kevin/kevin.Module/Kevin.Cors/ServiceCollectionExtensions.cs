using Kevin.Cors.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Kevin.Cors
{
    public static class ServiceCollectionExtensions
    {

        public static void AddKevinCors(this IServiceCollection services, CorsSetting WithOrigins)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            var orurls = WithOrigins.IPs?.Split(",").Where(t => !string.IsNullOrEmpty(t)).Select(t => t).ToList();
            if (orurls==default)
            {
                string hostName = System.Net.Dns.GetHostName(); 
                System.Net.IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(hostName); 
                System.Net.IPAddress[] addr = ipEntry.AddressList;
                string ip = addr[addr.Length - 1].ToString();
                orurls= new List<string> { ip }; 
            }
            services.AddCors(
                options => options.AddDefaultPolicy(builder =>
                {
                    builder.SetIsOriginAllowed(t => true);
                    builder.WithOrigins(orurls.ToArray()).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                }));
        }
    }
}
