using Microsoft.Extensions.Hosting;

namespace Kevin.Common.App.IO
{
    public static class Path
    {
        public static IHostEnvironment _hostingEnvironment { get; set; }
         
        /// <summary>
        /// 获取获取应用程序根目录路径
        /// </summary>
        /// <returns></returns>
        public static string ContentRootPath()
        {
            return _hostingEnvironment.ContentRootPath;
        }

        /// <summary>
        /// 获取 项目运行 路径
        /// </summary>
        /// <returns></returns>
        public static string WebRootPath()
        {
            return _hostingEnvironment.ContentRootPath;
        }

        /// <summary>
        /// 获取 项目运行 路径
        /// </summary>
        /// <returns></returns>
        public static string EnvironmentName()
        {
            return _hostingEnvironment.EnvironmentName;
        }
    }
}
