using Kevin.Common.App.Start;
using System.IO;
using TencentCloud.Tsf.V20180326.Models;

namespace Kevin.Common.App.IO
{
    public class Path
    {

        /// <summary>
        /// 获取 项目运行 路径
        /// </summary>
        /// <returns></returns>
        public static string ContentRootPath()
        {
            return StartWebHostEnvironment.webHostEnvironment.ContentRootPath;
        }

        /// <summary>
        /// 获取 项目运行 路径
        /// </summary>
        /// <returns></returns>
        public static string WebRootPath()
        {
            return StartWebHostEnvironment.webHostEnvironment.WebRootPath;
        }

        /// <summary>
        /// 获取 项目运行 路径
        /// </summary>
        /// <returns></returns>
        public static string EnvironmentName()
        {
            return StartWebHostEnvironment.webHostEnvironment.EnvironmentName;
        }
    }
}
