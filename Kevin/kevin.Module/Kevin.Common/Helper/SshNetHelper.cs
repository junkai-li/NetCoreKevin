using Renci.SshNet;
using System;
using System.Collections;
using System.Globalization;
using System.IO;

namespace Kevin.Common.Helper
{
    /// <summary>
    /// SSHNET Helper
    /// </summary>
    public class SshNetHelper : IDisposable
    {
        private readonly SftpClient sftp;

        public bool Connected => sftp.IsConnected;

        public string ip = "";
        public string port = "";
        public string user = "";
        public string pwd = "";

        public SshNetHelper(string ip, string port, string user, string pwd)
        {
            this.ip = ip;
            this.port = port;
            this.user = user;
            this.pwd = pwd;
            sftp = new SftpClient(ip, int.Parse(port, CultureInfo.CurrentCulture), user, pwd);
        }

        public void Connect()
        {

            try
            {
                if (!Connected)
                {
                    sftp.Connect();
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"Disconnect SFTP fail，Error Message：{ex.Message}");
            }
        }
        public void Disconnect()
        {
            try
            {
                if (sftp != null && Connected)
                {
                    sftp.Disconnect();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Disconnect SFTP fail，Error Message：{ex.Message}");
            }
        }
        /// <summary>
        /// SFTP Upload File
        /// </summary>
        /// <param name="localPath">Local file path</param>
        /// <param name="remotePath">Remote file path</param>
        public void Put(string localPath, string remotePath)
        {
            try
            {
                using (var file = File.OpenRead(localPath))
                {
                    Connect();
                    sftp.UploadFile(file, remotePath);
                    Disconnect();
                }
            }
            catch (Exception ex)
            {

                throw new Exception($"SFTP Upload File fail，Error Message：{ex.Message}");
            }
        }

        /// <summary>
        /// SFTP Upload File
        /// </summary>
        /// <param name="file">Local file path</param>
        /// <param name="remotePath">Remote file path</param>
        public void FliePut(Stream file, string remotePath)
        {
            try
            {
                Connect();
                sftp.UploadFile(file, remotePath);
                Disconnect();
            }
            catch (Exception ex)
            {

                throw new Exception($"SFTP Upload File fail，Error Message：{ex.Message}");
            }
        }

        /// <summary>
        /// SFTP Batch Upload File
        /// </summary>
        /// <param name="localPath">Local file path(without file name)</param>
        /// <param name="remotePath">Remote file path(without file name)</param>
        public void BatchPut(string localPath, string remotePath)
        {
            string uploadFullName = "";
            string remoteFullName = "";
            try
            {
                Connect();
                DirectoryInfo fdir = new DirectoryInfo(localPath);
                FileInfo[] file = fdir.GetFiles();
                if (file.Length != 0)
                {
                    foreach (FileInfo f in file) //show all files under the path
                    {
                        using (var upLoadFile = File.OpenRead(f.FullName))
                        {
                            uploadFullName = f.FullName;
                            remoteFullName = remotePath + "/" + f.Name;

                            sftp.UploadFile(upLoadFile, remotePath + "/" + f.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"SFTP Batch Upload File Fail，Error Message：{ex.Message}");
            }
        }

        /// <summary>
        /// SFTP Get File
        /// </summary>
        /// <param name="remotePath">Remote file path</param>
        /// <param name="localPath">Local file path</param>
        public void Get(string remotePath, string localPath)
        {
            try
            {
                Connect();
                var byt = sftp.ReadAllBytes(remotePath);
                Disconnect();
                System.IO.File.WriteAllBytes(localPath, byt);
            }
            catch (Exception ex)
            {
                throw new Exception($"SFTP Get File Fail，Error Message：{ex.Message}");
            }
        }

        /// <summary>
        /// Delete SFTP File
        /// </summary>
        /// <param name="remoteFile">Remote File path</param>
        public void Delete(string remoteFile)
        {
            try
            {
                Connect();
                sftp.Delete(remoteFile);
                Disconnect();
            }
            catch (Exception ex)
            {
                throw new Exception($"Remote File Path Fail，Error Message：{ex.Message}");
            }
        }


        /// <summary>
        /// Get SFTP File List
        /// </summary>
        /// <param name="remotePath">Local path</param>
        /// <param name="fileSuffix">file suffix</param>
        /// <returns></returns>
        public ArrayList GetFileList(string remotePath, string fileSuffix)
        {
            try
            {
                Connect();
                var files = sftp.ListDirectory(remotePath);
                Disconnect();
                var objList = new ArrayList();
                foreach (var file in files)
                {
                    string name = file.Name;
                    if (name.Length > (fileSuffix.Length + 1) && fileSuffix == name.Substring(name.Length - fileSuffix.Length))
                    {
                        objList.Add(name);
                    }
                }
                return objList;
            }
            catch (Exception ex)
            {
                throw new Exception($"Get SFTP File List Fail，Error Message：{ex.Message}");
            }
        }
        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="ptah">Local path</param> 
        /// <returns></returns>
        public byte[] GetFile(string ptah)
        {
            try
            {
                Connect();
                using var stream = sftp.OpenRead(ptah);
                var lie = new byte[stream.Length];
                using (BinaryReader br = new BinaryReader(stream))
                {
                    var i = (int)(stream.Length);
                    lie = br.ReadBytes(i);
                    br.Close();
                }
                Disconnect();
                return lie;
            }
            catch (Exception ex)
            {
                throw new Exception($"Get SFTP File List Fail，Error Message：{ex.Message}");
            }
        }

        /// <summary>
        /// Move SFTP File
        /// </summary>
        /// <param name="oldRemotePath">Old Remote Path</param>
        /// <param name="newRemotePath">New Remote Path</param>
        public void Move(string oldRemotePath, string newRemotePath)
        {
            try
            {
                Connect();
                sftp.RenameFile(oldRemotePath, newRemotePath);
                Disconnect();
            }
            catch (Exception ex)
            {
                throw new Exception($"Move SFTP File Faile，Error Message：{ex.Message}");
            }
        }

        public (bool, string) Cmd(string cmd)
        {
            try
            {
                var connectionInfo = new PasswordConnectionInfo(ip, user, pwd);
                using (var sshClient = new SshClient(connectionInfo))
                {
                    sshClient.Connect();
                    if (sshClient.IsConnected)
                    {
                        var result = sshClient.RunCommand(cmd);
                        if (result != null && !string.IsNullOrEmpty(result.Error))
                        {
                            return (false, result.Error);
                        }
                        return (true, result.Result);
                    }
                    sshClient.Disconnect();
                }
                return default;
            }
            catch (Exception ex)
            {
                throw new Exception($"Get SFTP File List Fail，Error Message：{ex.Message}");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            // dispose managed resources
            sftp.Dispose();
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
