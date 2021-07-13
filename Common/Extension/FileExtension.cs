using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Castle.Core.Internal;

namespace Common.Extension
{
    public static class FileExtension
    {
        private static void RecursingFolder(List<string> visitedPath,string rd, Action<string, string[]> directoryAndFiles)
        {
            if (!Directory.Exists(rd))
            {
                return;
            }
            if (visitedPath.Any(r => r == rd))
            {
                return;
            }
            var files = Directory.GetFiles(rd);
            //当前文件夹内容及所有文件内容
            directoryAndFiles(rd, files);
            visitedPath.Add(rd);
            foreach (var s in Directory.GetDirectories(rd))
            {
                DirPath_Recurse(s, directoryAndFiles);
            }
        }
        /// <summary>
        /// 对文件夹入口文件夹 进行递归遍历所有文件
        /// </summary>
        /// <param name="rootDictory">入口路径</param>
        /// <param name="directoryAndFiles">文件夹和文件的路径</param>
        public static void DirPath_Recurse(this string rootDictory, Action<string, string[]> directoryAndFiles)
        {
            List<string> visitedPath = new List<string>();
            RecursingFolder(visitedPath, rootDictory, directoryAndFiles);
        }
        /// <summary>
        /// 获取路径名称
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string DirPath_GetName(this string fullPath)
        {
            string str;
            if (fullPath.Length > 3)
            {
                string path = fullPath;
                int length = fullPath.Length;
                var isWithSeparatorChar =  length != 0 && (int)fullPath[length - 1] == (int)Path.DirectorySeparatorChar;
                if (isWithSeparatorChar)
                    path = fullPath.Substring(0, fullPath.Length - 1);
                str = Path.GetFileName(path);
            }
            else
                str = fullPath;
            return str;
        }

        /// <summary>
        /// Read File text by path， if not exist then  return string.Empty
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string FilePath_ReadText(this string path)
        {
            //从开发目录中寻找是否存在
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            //如果均不存在则抛出异常信息 提醒界面无法找到表单文件
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 文件名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string FilePath_GetFileName(this string path)
        {
            return Path.GetFileName(path);
        }

        /// <summary>
        /// 获取扩展名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string FilePath_GetExtensionName(this string path)
        {
            return Path.GetExtension(path);
        }


        /// <summary>
        /// 获取C#文件的命名空间
        /// </summary>
        /// <returns></returns>
        public static string FileString_GetCsharpNameSpace(this string fileString)
        {
            var spaceRegex = new Regex(@"\s?namespace\s+([a-zA-Z0-9_\-\.]+)\s+\{");
            var matched = spaceRegex.Match(fileString);
            if (matched.Groups.Count < 2)
                return null;
            return matched.Groups[1].Value;
        }

        /// <summary>
        /// 获取C#文件的类名
        /// </summary>
        /// <param name="fileString"></param>
        /// <returns></returns>
        public static string FileString_GetCsharpClassName(this string fileString)
        {
            var spaceRegex = new Regex(@"public\s+class\s+([a-zA-Z0-9\-,<>]+)\s*(:|\{)");
            var matched = spaceRegex.Match(fileString);
            if (matched.Groups.Count < 2)
                return null;
            return matched.Groups[1].Value;
        }

    }
}
