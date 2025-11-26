using kevin.FileStorage;
using kevin.Share.Dtos;
using Kevin.Common.App;
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

        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="tableid"></param>
        /// <param name="table"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<FileDto>> GetFileDtos(List<Guid> tableid, string table)
        {
            return (await fileRp.Query().Where(t => t.IsDelete == false && tableid.Contains(t.TableId) && t.Table == table).ToListAsync()).MapToList<TFile, FileDto>().ToList();
        }

        public Task<bool> DeleteFile(Guid id, CancellationToken cancellationToken)
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
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        public Task<(FileStream?, string?, string?)> GetFile(Guid fileid, CancellationToken cancellationToken)
        {
            var file = fileRp.Query().Where(t => t.Id == fileid).FirstOrDefault();
            if (file != default)
            {
                string path = Kevin.Common.App.IO.Path.ContentRootPath() + file.Path;

                //读取文件入流
                var stream = System.IO.File.OpenRead(path);

                //获取文件后缀
                string fileExt = Path.GetExtension(path);

                //获取系统常规全部mime类型
                var provider = new FileExtensionContentTypeProvider();

                //通过文件后缀寻找对呀的mime类型
                var memi = provider.Mappings.ContainsKey(fileExt) ? provider.Mappings[fileExt] : provider.Mappings[".zip"];

                return Task.FromResult<(FileStream?, string?, string?)>((stream, memi, file.Name));
            }
            throw new UserFriendlyException("通过指定的文件ID未找到任何文件");
        }

        public async Task<string> GetFilePath(Guid fileid, CancellationToken cancellationToken)
        {
            var file = fileRp.Query().Where(t => t.Id == fileid).FirstOrDefault();

            if (file != null)
            {
                string domain = "";
                if (fileStorage != default)
                {
                    domain = await fileStorage.GetUrl();
                }
                else
                {
                    //本地
                    domain = HttpContextAccessor.GetBaseUrl();
                } 
                string fileUrl = domain + file.Path?.Replace("\\", "/"); 
                return fileUrl;
            }
            else
            {
                throw new UserFriendlyException("通过指定的文件ID未找到任何文件");
            }
        }

        public async Task<Guid> RemoteUploadFile(string table, Guid tableid, string sign, dtoKeyValue fileInfo, CancellationToken cancellationToken)
        {
            string remoteFileUrl = fileInfo.Key?.ToString() ?? "";
            var fileExtension = Path.GetExtension(fileInfo.Value?.ToString() ?? "").ToLower();
            var fileName = Guid.NewGuid().ToString() + fileExtension;
            string basepath = "Files/" + DateTime.Now.ToString("yyyy/MM/dd");
            var filePath = Kevin.Common.App.IO.Path.ContentRootPath() + "/" + basepath + "/";
            //下载文件
            var dlPath = Common.IO.IOHelper.DownloadFile(remoteFileUrl, filePath, fileName);
            if (dlPath == null)
            {
                throw new UserFriendlyException("远程文件下载失败！");
            }
            filePath = dlPath.Replace(Kevin.Common.App.IO.Path.ContentRootPath(), "");
            if (dlPath != null)
            {
                var isSuccess = true;
                if (fileStorage != default)
                {
                    var upload = fileStorage.FileUpload(dlPath, basepath, (fileInfo.Value ?? "").ToString());
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
                    f.Name = (fileInfo.Value ?? "").ToString();
                    f.Path = filePath;
                    f.Table = table;
                    f.TableId = tableid;
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

        public async Task<Guid> UploadFile(string table, Guid tableid, string sign, IFormFile file, CancellationToken cancellationToken, int MB = 50)
        {
            if (file == null || file.Length == 0)
            {
                throw new UserFriendlyException($"文件为空");
            }
            if (file.Length > MB * 1024 * 1024)
            {
                throw new UserFriendlyException($"文件不能大于{MB}MB");
            }
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
                if (fileStorage != default)
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
                if (file != default)
                {
                    TFile f = new();
                    f.Id = fileName;
                    f.IsDelete = false;
                    f.Name = file.FileName ?? Guid.NewGuid().ToString();
                    f.Path = path;
                    f.Table = table;
                    f.TableId = tableid;
                    f.Sign = sign;
                    f.CreateUserId = CurrentUser.UserId;
                    f.CreateTime = DateTime.Now;
                    fileRp.Add(f);
                    await fileRp.SaveChangesAsync();
                }
                return fileName;
            }
            else
            {
                throw new UserFriendlyException("文件上传失败！");
            }
        }

    }
}
