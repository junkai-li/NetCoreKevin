using kevin.FileStorage.AliCloud;
using kevin.FileStorage.KevinStaticFiles;
using kevin.FileStorage.TencentCloud;
using Microsoft.Extensions.DependencyInjection;

namespace kevin.FileStorage
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAliCloudStorage(this IServiceCollection services, Action<kevin.FileStorage.AliCloud.Models.FileStorageSetting> action)
        {
            services.Configure(action);
            services.AddTransient<IFileStorage, AliCloudStorage>();
        }

        public static void AddTencentCloudStorage(this IServiceCollection services, Action<kevin.FileStorage.TencentCloud.Models.FileStorageSetting> action)
        {
            services.Configure(action);
            services.AddTransient<IFileStorage, TencentCloudStorage>();
        }

        public static void AddKevinStaticFilesStorage(this IServiceCollection services, Action<kevin.FileStorage.KevinStaticFiles.Models.FileStorageSetting> action)
        {
            services.Configure(action);
            services.AddTransient<IFileStorage, KevinStaticFilesStorage>();
        }
    }
}
