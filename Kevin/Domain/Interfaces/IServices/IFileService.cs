using kevin.Domain.Share.Dtos.System;
using Microsoft.AspNetCore.Http;

namespace kevin.Domain.Interfaces.IServices
{
    public interface IFileService : IBaseService
    {

        /// <summary>
        /// 获取相关文件
        /// </summary>
        /// <param name="tableid"></param>
        /// <param name="table"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<FileDto>> GetFileDtos(List<string> tableid, string table);


        /// <summary>
        /// 远程单文件上传接口
        /// </summary>
        /// <param name="business">业务领域</param>
        /// <param name="key">记录值</param>
        /// <param name="sign">自定义标记</param>
        /// <param name="fileInfo">Key为文件URL,Value为文件名称</param>
        /// <returns>文件ID</returns>
        Task<long> RemoteUploadFile(string business, string key, string sign, dtoKeyValue fileInfo, CancellationToken cancellationToken);
        /// <summary>
        /// 单文件上传接口
        /// </summary>
        /// <param name="business">业务领域</param>
        /// <param name="key">记录值</param>
        /// <param name="sign">自定义标记</param>
        /// <param name="file">file</param>
        /// <returns>文件ID</returns>
        Task<long> UploadFile(string business, string key, string sign, IFormFile file, CancellationToken cancellationToken);

        /// <summary>
        /// 通过文件ID获取文件
        /// </summary>
        /// <param name="fileid">文件ID</param>
        /// <returns></returns>
        Task<(FileStream, string, string)> GetFile(long fileid, CancellationToken? cancellationToken=default);


        /// <summary>
        /// 通过文件ID获取文件静态访问路径
        /// </summary>
        /// <param name="fileid">文件ID</param>
        /// <returns></returns> 
        Task<string> GetFilePath(long fileid, CancellationToken cancellationToken);

        /// <summary>
        /// 通过文件ID删除文件方法
        /// </summary>
        /// <param name="id">文件ID</param>
        /// <returns></returns>
        Task<bool> DeleteFile(long id, CancellationToken cancellationToken);  
    }
}
