using kevin.FileStorage;
using kevin.Share.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Repository.Database;
using Web.Global.Exceptions;

namespace kevin.Application.Services
{
    public class FileService : BaseService, IFileService
    {
        public IFileRp fileRp { get; set; }

        public IFileStorage fileStorage { get; set; }
        public FileService(IHttpContextAccessor _httpContextAccessor, IFileRp _fileRp, IFileStorage _fileStorage) : base(_httpContextAccessor)
        {
            fileRp = _fileRp;
            fileStorage = _fileStorage;
        }

        async Task<bool> IFileService.DeleteFile(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var file = fileRp.Query().Where(t => t.IsDelete == false && t.Id == id).FirstOrDefault();
                if (file != default)
                {
                    file.IsDelete = true;
                    file.DeleteTime = DateTime.Now;
                    fileRp.SaveChanges(); 
                } 
                return true;
            }
            catch
            {
                return false;
            }
        }

        async Task<(FileStream, string, string)> IFileService.GetFile(Guid fileid, CancellationToken cancellationToken)
        {
            var file = fileRp.Query().Where(t => t.Id == fileid).FirstOrDefault();
            string path = Kevin.Common.App.IO.Path.ContentRootPath() + file.Path;

            //读取文件入流
            var stream = System.IO.File.OpenRead(path);

            //获取文件后缀
            string fileExt = Path.GetExtension(path);

            //获取系统常规全部mime类型
            var provider = new FileExtensionContentTypeProvider();

            //通过文件后缀寻找对呀的mime类型
            var memi = provider.Mappings.ContainsKey(fileExt) ? provider.Mappings[fileExt] : provider.Mappings[".zip"];

            return (stream, memi, file.Name);
        }

        async Task<string> IFileService.GetFilePath(Guid fileid, CancellationToken cancellationToken)
        {
            var file = fileRp.Query().Where(t => t.Id == fileid).FirstOrDefault();

            if (file != null)
            {
                string domain = "https://file.xxxx.com";

                string fileUrl = domain + file.Path.Replace("\\", "/");

                return fileUrl;
            }
            else
            {
                throw new UserFriendlyException("通过指定的文件ID未找到任何文件");
            }
        }

        Task<FileResult> IFileService.GetImage(Guid fileId, int width, int height, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<Guid> IFileService.RemoteUploadFile(string business, Guid key, string sign, dtoKeyValue fileInfo, CancellationToken cancellationToken)
        {
            string remoteFileUrl = fileInfo.Key.ToString();

            var fileExtension = Path.GetExtension(fileInfo.Value.ToString()).ToLower();
            var fileName = Guid.NewGuid().ToString() + fileExtension;

            string basepath = "Files/" + DateTime.Now.ToString("yyyy/MM/dd");

            var filePath = Kevin.Common.App.IO.Path.ContentRootPath() + "/" + basepath + "/";

            //下载文件
            var dlPath = Common.IO.IOHelper.DownloadFile(remoteFileUrl, filePath, fileName);

            if (dlPath == null)
            {
                Thread.Sleep(5000);
                dlPath = Common.IO.IOHelper.DownloadFile(remoteFileUrl, filePath, fileName);
            }
            filePath = dlPath.Replace(Kevin.Common.App.IO.Path.ContentRootPath(), "");

            if (dlPath != null)
            {
                var isSuccess = true;
                var upRemote = false;
                if (upRemote == true)
                {
                    var upload = fileStorage.FileUpload(dlPath, basepath, fileInfo.Value.ToString());
                    if (upload)
                    {
                        Common.IO.IOHelper.DeleteFile(dlPath);
                    }
                    else
                    {
                        isSuccess = false;
                    }
                }

                if (isSuccess)
                {

                    TFile f = new();
                    f.Id = Guid.NewGuid();
                    f.Name = fileInfo.Value.ToString();
                    f.Path = filePath;
                    f.Table = business;
                    f.TableId = key;
                    f.Sign = sign;
                    f.CreateUserId = CurrentUser.UserId;
                    f.CreateTime = DateTime.Now;
                    fileRp.Add(f);
                    await fileRp.SaveChangesAsync();
                    return f.Id;
                }

            }
            throw new UserFriendlyException("文件上传失败！");
        }

        async Task<Guid> IFileService.UploadFile(string business, Guid key, string sign, IFormFile file, CancellationToken cancellationToken)
        {
            string basepath = "/Files/" + DateTime.Now.ToString("yyyy/MM/dd");
            string filepath = Kevin.Common.App.IO.Path.ContentRootPath() + basepath;
            Directory.CreateDirectory(filepath);

            var fileName = Guid.NewGuid();
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fullFileName = string.Format("{0}{1}", fileName, fileExtension);
            string path = "";
            var isSuccess = false;
            if (file != null && file.Length > 0)
            {
                path = filepath + "/" + fullFileName;
                using (FileStream fs = System.IO.File.Create(path))
                {
                    file.CopyTo(fs);
                    fs.Flush();
                }
                var upRemote = false;
                if (upRemote)
                {
                    var upload = fileStorage.FileUpload(path, "Files/" + DateTime.Now.ToString("yyyy/MM/dd"), file.FileName);
                    if (upload)
                    {
                        Common.IO.IOHelper.DeleteFile(path);
                        path = "/Files/" + DateTime.Now.ToString("yyyy/MM/dd") + "/" + fullFileName;
                        isSuccess = true;
                    }
                }
                else
                {
                    path = basepath + "/" + fullFileName;
                    isSuccess = true;
                }
            }

            if (isSuccess)
            {
                TFile f = new();
                f.Id = fileName;
                f.IsDelete = false;
                f.Name = file.FileName;
                f.Path = path;
                f.Table = business;
                f.TableId = key;
                f.Sign = sign;
                f.CreateUserId = CurrentUser.UserId;
                f.CreateTime = DateTime.Now;
                fileRp.Add(f);
                await fileRp.SaveChangesAsync();
                return fileName;
            }
            else
            {
                throw new UserFriendlyException("文件上传失败！");
            }
        }
        async Task<Guid> IFileService.CreateGroupFileId(string business, Guid key, string sign, string fileName, int slicing, string unique, CancellationToken cancellationToken)
        {
            using var db = new KevinDbContext();
            var dbfileinfo = db.Set<TFileGroup>().Where(t => t.Unique.ToLower() == unique.ToLower()).FirstOrDefault(); 
            if (dbfileinfo == null)
            {
                var fileid = Guid.NewGuid().ToString() + Path.GetExtension(fileName).ToLowerInvariant(); ;
                string basepath = "/Files/" + DateTime.Now.ToString("yyyy/MM/dd") + "/" + fileid;
                TFile f = new();
                f.Id = Guid.NewGuid();
                f.Name = fileName;
                f.Path = basepath;
                f.Table = business;
                f.TableId = key;
                f.Sign = sign;
                f.CreateTime = DateTime.Now;
                db.Set<TFile>().Add(f);
                db.SaveChanges();

                TFileGroup group = new();
                group.Id = Guid.NewGuid();
                group.FileId = f.Id;
                group.Unique = unique;
                group.Slicing = slicing;
                group.Issynthesis = false;
                group.Isfull = false;
                db.Set<TFileGroup>().Add(group);
                db.SaveChanges();
                return f.Id;
            }
            else
            {
                return dbfileinfo.FileId;
            }
        }
        async Task<bool> IFileService.UploadGroupFile(Guid fileId, int index, IFormFile file, CancellationToken cancellationToken)
        {
            try
            {
                using var db = new KevinDbContext();
                var url = string.Empty;
                var fileName = string.Empty;
                var fileExtension = string.Empty;
                var fullFileName = string.Empty;

                string basepath = "/Files/Group/" + DateTime.Now.ToString("yyyy/MM/dd") + "/" + fileId;
                string filepath = Kevin.Common.App.IO.Path.ContentRootPath() + basepath;

                Directory.CreateDirectory(filepath);

                fileName = Guid.NewGuid().ToString();
                fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                fullFileName = string.Format("{0}{1}", fileName, fileExtension);

                string path = "";

                if (file != null && file.Length > 0)
                {
                    path = filepath + "/" + fullFileName;

                    using (FileStream fs = System.IO.File.Create(path))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }

                    path = basepath + "/" + fullFileName;
                }


                var group = db.Set<TFileGroup>().Where(t => t.FileId == fileId).FirstOrDefault();

                var groupfile = new TFileGroupFile();
                groupfile.Id = Guid.NewGuid();
                groupfile.FileId = group.FileId;
                groupfile.Path = path;
                groupfile.Index = index;
                groupfile.CreateTime = DateTime.Now;

                db.Set<TFileGroupFile>().Add(groupfile);

                if (index == group.Slicing)
                {
                    group.Isfull = true;
                }

                db.SaveChanges();

                if (group.Isfull == true)
                {

                    try
                    {
                        byte[] buffer = new byte[1024 * 100];

                        var fileinfo = db.Set<TFile>().Where(t => t.Id == fileId).FirstOrDefault();

                        var fullfilepath = Kevin.Common.App.IO.Path.ContentRootPath() + fileinfo.Path;

                        using (FileStream outStream = new(fullfilepath, FileMode.Create))
                        {
                            int readedLen = 0;
                            FileStream srcStream = null;

                            var filelist = db.Set<TFileGroupFile>().Where(t => t.FileId == fileinfo.Id).OrderBy(t => t.Index).ToList();

                            foreach (var item in filelist)
                            {
                                string p = Kevin.Common.App.IO.Path.ContentRootPath() + item.Path;
                                srcStream = new FileStream(p, FileMode.Open);
                                while ((readedLen = srcStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    outStream.Write(buffer, 0, readedLen);
                                }
                                srcStream.Close();
                            }
                        }

                        group.Issynthesis = true;

                        db.SaveChanges();
                    }
                    catch
                    {

                    }

                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
