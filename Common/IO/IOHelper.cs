using Common.IO.Tar;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace Common.IO
{
    public class IOHelper
    {

        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public static bool DeleteFile(string path)
        {
            try
            {
                var file = new FileInfo(path);
                if (file.Exists)
                {
                    //将文件属性设置为普通,如：只读文件设置为普通
                    file.Attributes = FileAttributes.Normal;

                    file.Delete();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }





        /// <summary>
        /// 删除指定文件夹
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <returns></returns>
        public static bool DeleteDirectory(string path)
        {
            try
            {
                var directory = new DirectoryInfo(path);
                if (directory.Exists)
                {
                    //将文件夹属性设置为普通,如：只读文件夹设置为普通
                    directory.Attributes = FileAttributes.Normal;

                    directory.Delete(true);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }



        /// <summary>
        /// 下载远程文件保存到本地
        /// </summary>
        /// <param name="url">文件URL</param>
        /// <param name="filePath">保存路径，以 / 结束，否则将取最后一个 / 之前的路径, / 之后的当作自定义文件名前缀</param>
        /// <param name="fileName">保存文件名称,不传则自动通过 url 获取名称</param>
        /// <returns></returns>
        public static string DownloadFile(string url, string filePath, string fileName = null)
        {
            try
            {
                if (fileName == null)
                {
                    fileName = System.Web.HttpUtility.UrlDecode(Path.GetFileName(url));
                }

                //检查目标路径文件夹是否存在不存在则创建
                if (!Directory.Exists(filePath))
                {
                    //如果路径结尾不是 / 则说明尾端可能是自定义的文件名前缀

                    var lastindex = filePath.Length - filePath.LastIndexOf("/");

                    if (lastindex == 1)
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    else
                    {
                        string temp = filePath.Substring(0, filePath.LastIndexOf("/"));
                        Directory.CreateDirectory(temp);

                    }
                }

                WebClient webClient = new WebClient();

                //添加来源属性，解决部分资源防盗链伪验证
                webClient.Headers.Add(HttpRequestHeader.Referer, "");

                string fullpath = filePath + fileName;

                //下载文件
                webClient.DownloadFile(url, fullpath);

                return fullpath;
            }
            catch
            {
                return null;
            }
        }



        /// <summary>
        /// 获取指定文件的大小
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileSize(string path)
        {
            var fileInfo = new FileInfo(path);

            string m_strSize = "";

            long FactSize = fileInfo.Length;

            if (FactSize < 1024.00)
            {
                m_strSize = FactSize.ToString("F2") + " Byte";
            }
            else if (FactSize >= 1024.00 && FactSize < 1048576)
            {
                m_strSize = (FactSize / 1024.00).ToString("F2") + " K";
            }
            else if (FactSize >= 1048576 && FactSize < 1073741824)
            {
                m_strSize = (FactSize / 1024.00 / 1024.00).ToString("F2") + " M";
            }
            else if (FactSize >= 1073741824)
            {
                m_strSize = (FactSize / 1024.00 / 1024.00 / 1024.00).ToString("F2") + " G";
            }

            return m_strSize;
        }



        /// <summary>
        /// 文件Length值转String
        /// </summary>
        /// <param name="fileLength"></param>
        /// <returns></returns>
        public static string FileLengthToString(long fileLength)
        {

            string m_strSize = "";

            if (fileLength < 1024.00)
            {
                m_strSize = fileLength.ToString("F2") + " Byte";
            }
            else if (fileLength >= 1024.00 && fileLength < 1048576)
            {
                m_strSize = (fileLength / 1024.00).ToString("F2") + " K";
            }
            else if (fileLength >= 1048576 && fileLength < 1073741824)
            {
                m_strSize = (fileLength / 1024.00 / 1024.00).ToString("F2") + " M";
            }
            else if (fileLength >= 1073741824)
            {
                m_strSize = (fileLength / 1024.00 / 1024.00 / 1024.00).ToString("F2") + " G";
            }

            return m_strSize;
        }



        /// <summary>
        /// 获取文件夹下所有文件
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        public static List<string> GetFolderAllFiles(string folderPath)
        {
            var list = new List<string>();

            var directoryInfo = new DirectoryInfo(folderPath);
            foreach (FileInfo info in directoryInfo.GetFiles())
            {
                list.Add(info.FullName);
            }
            foreach (DirectoryInfo info in directoryInfo.GetDirectories())
            {
                list.AddRange(GetFolderAllFiles(info.FullName));
            }


            for (int i = 0; i < list.Count; i++)
            {
                list[i] = list[i].Replace(@"\", "/");
            }

            return list;
        }



        /// <summary>
        /// 将指定目录下的文件压缩为Tar文件
        /// </summary>
        /// <param name="folderPath">文件夹地址 D:/1/ </param>
        /// <param name="filePath">文件地址 D:/1.tar </param>
        /// <remarks>依赖于SharpZipLib，如果缺失请独立安装</remarks>
        public static void CompressTarFile(string folderPath, string filePath)
        {
            var outStream = new FileStream(filePath, FileMode.OpenOrCreate);
            var archive = TarArchive.CreateOutputTarArchive(outStream);
            var files = GetFolderAllFiles(folderPath);

            foreach (var file in files)
            {

                TarEntry entry = TarEntry.CreateEntryFromFile(file);
                entry.Name = file.Replace(folderPath, "");
                entry.TarHeader.Mode = 511;
                archive.WriteEntry(entry, true);
            }
            if (archive != null)
            {
                archive.Close();
            }
        }



        /// <summary>
        /// 解压Tar文件到指定目录
        /// </summary>
        /// <param name="filePath">文件地址 D:/1.tar</param>
        /// <param name="folderPath">文件夹地址 D:/1/</param>
        /// <remarks>依赖于SharpZipLib，如果缺失请独立安装</remarks>
        public static void DecompressTarFile(string filePath, string folderPath)
        {

            using (var s = new TarInputStream(File.OpenRead(filePath), Encoding.Default))
            {

                TarEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {

                    theEntry.Name = folderPath + theEntry.Name;

                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    if (fileName != string.Empty)
                    {
                        using (FileStream streamWriter = System.IO.File.Create(theEntry.Name))
                        {

                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }





        /// <summary>
        /// 将指定目录下的文件压缩为Zip文件
        /// </summary>
        /// <param name="folderPath">文件夹地址 D:/1/ </param>
        /// <param name="filePath">文件地址 D:/1.zip </param>
        public static void CompressZipFile(string folderPath, string filePath)
        {
            ZipFile.CreateFromDirectory(folderPath, filePath, CompressionLevel.Optimal, false);
        }



        /// <summary>
        /// 解压Zip文件到指定目录
        /// </summary>
        /// <param name="filePath">文件地址 D:/1.zip</param>
        /// <param name="folderPath">文件夹地址 D:/1/</param>
        public static void DecompressZipFile(string filePath, string folderPath)
        {
            ZipFile.ExtractToDirectory(filePath, folderPath);
        }

    }
}
