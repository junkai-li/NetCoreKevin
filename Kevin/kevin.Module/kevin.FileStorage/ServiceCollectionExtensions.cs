using kevin.FileStorage.AliCloud;
using kevin.FileStorage.KevinStaticFiles;
using kevin.FileStorage.QiniuCloud;
using kevin.FileStorage.TencentCloud;
using Microsoft.Extensions.DependencyInjection;

namespace kevin.FileStorage
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 阿里云
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action"></param>
        public static void AddAliCloudStorage(this IServiceCollection services, Action<kevin.FileStorage.AliCloud.Models.FileStorageSetting> action)
        {
            services.Configure(action);
            services.AddTransient<IFileStorage, AliCloudStorage>();
        }
        /// <summary>
        /// 腾讯云
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action"></param>
        public static void AddTencentCloudStorage(this IServiceCollection services, Action<kevin.FileStorage.TencentCloud.Models.FileStorageSetting> action)
        {
            services.Configure(action);
            services.AddTransient<IFileStorage, TencentCloudStorage>();
        }
        /// <summary>
        /// 本地
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action"></param>
        public static void AddKevinStaticFilesStorage(this IServiceCollection services, Action<kevin.FileStorage.KevinStaticFiles.Models.FileStorageSetting> action)
        {
            services.Configure(action);
            services.AddTransient<IFileStorage, KevinStaticFilesStorage>();
        }

        /// <summary>
        /// 七牛云
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action"></param>
        public static void AddQiniuCloudStorage(this IServiceCollection services, Action<kevin.FileStorage.QiniuCloud.Models.FileStorageSetting> action)
        {
            services.Configure(action);
            services.AddTransient<IFileStorage, QiniuCloudStorage>();
        }
    }
}
