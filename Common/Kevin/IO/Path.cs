
using System.IO;

namespace Kevin.Common.Kevin.IO
{
    public class Path
    {  

        /// <summary>
        /// 获取 项目运行 路径
        /// </summary>
        /// <returns></returns>
        public static string ContentRootPath()
        {
            return Directory.GetCurrentDirectory();
        }

    }
}
