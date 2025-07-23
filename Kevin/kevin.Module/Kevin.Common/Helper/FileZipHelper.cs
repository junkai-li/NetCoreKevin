using ICSharpCode.SharpZipLib.Checksum;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetCore.Util
{
    /// <summary>
    /// 文件压缩帮助类
    /// </summary>
    public class FileZipHelper
    {
        /// <summary>
        /// 压缩一个文件
        /// </summary>
        /// <param name="file">文件信息</param>
        /// <returns></returns>
        public static byte[] ZipFile(FileEntry file)
        {
            return ZipFile(new List<FileEntry> { file });
        }

        /// <summary>
        /// 压缩多个文件
        /// </summary>
        /// <param name="files">文件信息列表</param>
        /// <returns></returns>
        public static byte[] ZipFile(List<FileEntry> files)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (ZipOutputStream zipStream = new ZipOutputStream(ms))
                {
                    files.ForEach(aFile =>
                    {
                        byte[] fileBytes = aFile.FileBytes;
                        ZipEntry entry = new ZipEntry(aFile.FileName)
                        {
                            DateTime = DateTime.Now,
                            IsUnicodeText = true
                        };
                        zipStream.PutNextEntry(entry);
                        zipStream.Write(fileBytes, 0, fileBytes.Length);
                        zipStream.CloseEntry();
                    });

                    zipStream.IsStreamOwner = false;
                    zipStream.Finish();
                    zipStream.Close();
                    ms.Position = 0;

                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationZipFilePath"></param>
        public static void CreateZip(string sourceFilePath, string destinationZipFilePath)
        {
            if (sourceFilePath[sourceFilePath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                sourceFilePath += System.IO.Path.DirectorySeparatorChar;
            using FileStream fsWrite = new FileStream(destinationZipFilePath, FileMode.Create);
            ZipOutputStream zipStream = new ZipOutputStream(fsWrite);
            zipStream.SetLevel(6);  // 压缩级别 0-9
            CreateZipFiles(sourceFilePath, zipStream, sourceFilePath);

            zipStream.Finish();
            zipStream.Close();
        }
        /// <summary>
        /// 递归压缩文件
        /// </summary>
        /// <param name="sourceFilePath">待压缩的文件或文件夹路径</param>
        /// <param name="zipStream">
        /// <param name="staticFile"></param>
        private static void CreateZipFiles(string sourceFilePath, ZipOutputStream zipStream, string staticFile)
        {
            Crc32 crc = new Crc32();
            string[] filesArray = Directory.GetFileSystemEntries(sourceFilePath);
            foreach (string file in filesArray)
            {
                if (Directory.Exists(file))                     //如果当前是文件夹，递归
                {
                    CreateZipFiles(file, zipStream, staticFile);
                }

                else                                            //如果是文件，开始压缩
                {
                    FileStream fileStream = File.OpenRead(file);

                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, buffer.Length);
                    string tempFile = file.Substring(staticFile.LastIndexOf("\\") + 1);
                    ZipEntry entry = new ZipEntry(tempFile);

                    entry.DateTime = DateTime.Now;
                    entry.Size = fileStream.Length;
                    fileStream.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    zipStream.PutNextEntry(entry);

                    zipStream.Write(buffer, 0, buffer.Length);
                }
            }
        }
    }

    /// <summary>
    /// 文件信息
    /// </summary>
    public struct FileEntry
    {
        public FileEntry(string fileName, byte[] fileBytes)
        {
            FileName = fileName;
            FileBytes = fileBytes;
        }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件字节
        /// </summary>
        public byte[] FileBytes { get; set; }
    }
}
