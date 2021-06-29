using Cms.Libraries.Start;

namespace Cms.Libraries.IO
{
    public class Path
    {


        /// <summary>
        /// 获取 wwwroot 路径
        /// </summary>
        /// <returns></returns>
        public static string WebRootPath()
        {
            return StartHostingEnvironment.webHostEnvironment.WebRootPath.Replace("\\", "/");
        }



        /// <summary>
        /// 获取 项目运行 路径
        /// </summary>
        /// <returns></returns>
        public static string ContentRootPath()
        {
            return StartHostingEnvironment.webHostEnvironment.ContentRootPath.Replace("\\", "/");
        }

    }
}
