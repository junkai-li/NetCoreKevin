using Aop.Api.Domain;
using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Kevin;
using kevin.Permission.Permisson.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using NPOI.SS.Formula.Functions;
using SkiaSharp;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Controllers
{

    /// <summary>
    /// 文件上传控制器
    /// </summary>
    [ApiVersionNeutral]
    [Authorize]
    [Route("api/[controller]")]
    [SkipAuthority]
    public class FileController : ApiControllerBase
    {
        [IocProperty]
        public IFileService _fileService { get; set; }

        /// <summary>
        /// 远程单文件上传接口
        /// </summary>
        /// <param name="business">业务领域</param>
        /// <param name="key">记录值</param>
        /// <param name="sign">自定义标记</param>
        /// <param name="fileInfo">Key为文件URL,Value为文件名称</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>文件ID</returns>
        [HttpPost("RemoteUploadFile")]
        public async Task<Guid> RemoteUploadFile([FromQuery][Required] string business, [FromQuery][Required] Guid key, [FromQuery][Required] string sign, [Required][FromBody] dtoKeyValue fileInfo, CancellationToken cancellationToken)
        {
            return await _fileService.RemoteUploadFile(business, key, sign, fileInfo, cancellationToken);
        }

        /// <summary>
        /// 单文件上传接口
        /// </summary>
        /// <param name="business">业务领域</param>
        /// <param name="key">记录值</param>
        /// <param name="sign">自定义标记</param>
        /// <param name="file">file</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>文件ID</returns>
        [DisableRequestSizeLimit]
        [HttpPost("UploadFile")]
        public async Task<Guid> UploadFile([FromQuery][Required] string business, [FromQuery][Required] Guid key, [FromQuery][Required] string sign, [Required] IFormFile file, CancellationToken cancellationToken)
        {

            return await _fileService.UploadFile(business, key, sign, file, cancellationToken);
        }



        /// <summary>
        /// 多文件上传接口
        /// </summary>
        /// <param name="business">业务领域</param>
        /// <param name="key">记录值</param>
        /// <param name="sign">标记</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns></returns>
        /// <remarks>swagger 暂不支持多文件接口测试，请使用 postman</remarks>
        [DisableRequestSizeLimit]
        [HttpPost("BatchUploadFile")]
        public async Task<List<Guid>> BatchUploadFile([FromQuery][Required] string business, [FromQuery][Required] Guid key, [FromQuery][Required] string sign, CancellationToken cancellationToken)
        {
            var fileIds = new List<Guid>();
            var ReqFiles = Request.Form.Files;
            List<IFormFile> Attachments = new();
            for (int i = 0; i < ReqFiles.Count; i++)
            {
                Attachments.Add(ReqFiles[i]);
            }
            foreach (var file in Attachments)
            {
                var fileId = await UploadFile(business, key, sign, file, cancellationToken);

                fileIds.Add(fileId);
            }

            return fileIds;
        }

        /// <summary>
        /// 通过文件ID获取文件
        /// </summary>
        /// <param name="fileid">文件ID</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetFile")]
        public async Task<FileResult> GetFile([Required] Guid fileid, CancellationToken cancellationToken)
        {
            var data = await _fileService.GetFile(fileid, cancellationToken);
            return File(data.Item1, data.Item2, data.Item3); 
        }




        /// <summary>
        /// 通过图片文件ID获取图片
        /// </summary>
        /// <param name="fileId">图片ID</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns></returns>
        /// <remarks>不指定宽高参数,返回原图</remarks>
        [AllowAnonymous]
        [HttpGet("GetImage")]
        public FileResult GetImage([Required] Guid fileId, int width, int height, CancellationToken cancellationToken)
        {

            var file = db.Set<TFile>().Where(t => t.Id == fileId).FirstOrDefault();
            var path = Kevin.Common.App.IO.Path.ContentRootPath() + file.Path;

            var stream = System.IO.File.OpenRead(path);

            string fileExt = Path.GetExtension(path);

            var provider = new FileExtensionContentTypeProvider();

            var memi = provider.Mappings[fileExt];

            if (width == 0 && height == 0)
            {
                return File(stream, memi, file.Name);
            }
            else
            {
                using var original = SKBitmap.Decode(path);
                if (original.Width < width || original.Height < height)
                {
                    return File(stream, memi, file.Name);
                }
                else
                {

                    if (width != 0 && height == 0)
                    {
                        var percent = ((float)width / (float)original.Width);

                        width = (int)(original.Width * percent);
                        height = (int)(original.Height * percent);
                    }

                    if (width == 0 && height != 0)
                    {
                        var percent = ((float)height / (float)original.Height);

                        width = (int)(original.Width * percent);
                        height = (int)(original.Height * percent);
                    }
                    var SKSamplingOptions = new SKSamplingOptions();  
                    using var resizeBitmap = original.Resize(new SKImageInfo(width, height), SKSamplingOptions);
                    using var image = SKImage.FromBitmap(resizeBitmap);
                    using var imageData = image.Encode(SKEncodedImageFormat.Png, 100);
                    return File(imageData.ToArray(), "image/png");
                }

            }
        }
         
        /// <summary>
        /// 通过文件ID获取文件静态访问路径
        /// </summary>
        /// <param name="fileid">文件ID</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns></returns>
        [HttpGet("GetFilePath")]
        public async Task<string> GetFilePath([Required] Guid fileid, CancellationToken cancellationToken)
        {
            return  await _fileService.GetFilePath(fileid, cancellationToken); 
        }
        /// <summary>
        /// 多文件切片上传，获取初始化文件ID
        /// </summary>
        /// <param name="business">业务领域</param>
        /// <param name="key">记录值</param>
        /// <param name="sign">自定义标记</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="slicing">总切片数</param>
        /// <param name="unique">文件校验值</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns></returns>
        [HttpGet("CreateGroupFileId")]
        public async Task<Guid> CreateGroupFileId([Required] string business, [Required] Guid key, [Required] string sign, [Required] string fileName, [Required] int slicing, [Required] string unique, CancellationToken cancellationToken)
        { 
           return await _fileService.CreateGroupFileId(business, key, sign, fileName, slicing, unique, cancellationToken);
        }



        /// <summary>
        /// 文件切片上传接口
        /// </summary>
        /// <param name="fileId">文件组ID</param>
        /// <param name="index">切片索引</param>
        /// <param name="file">file</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>文件ID</returns>
        [HttpPost("UploadGroupFile")]
        public async Task<bool> UploadGroupFile([Required][FromForm] Guid fileId, [Required][FromForm] int index, [Required] IFormFile file, CancellationToken cancellationToken)
        { 
               return await _fileService.UploadGroupFile(fileId, index, file, cancellationToken);
        }



        /// <summary>
        /// 通过文件ID删除文件方法
        /// </summary>
        /// <param name="id">文件ID</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns></returns>
        [HttpDelete("DeleteFile")]
        public async Task<bool> DeleteFile(Guid id, CancellationToken cancellationToken)
        {
            return  await _fileService.DeleteFile(id, cancellationToken);
        }


    }
}