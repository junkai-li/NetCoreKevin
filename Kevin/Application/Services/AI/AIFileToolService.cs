using Common;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.FileStorage;
using System.ComponentModel;
using System.Text;
using TencentCloud.Teo.V20220901.Models;

namespace kevin.Application.Services.AI
{
    public class AIFileToolService : IAIFileToolService
    {
        public readonly IFileStorage _fileStorage;
        public AIFileToolService(IHttpContextAccessor _httpContextAccessor, IFileStorage fileStorage)
        {
            this._fileStorage = fileStorage;
        }
        public string SaveFileContent([Description("文件名称如（xx.html,xx.txt）支持各种文件类型")] string fileName, [Description("本地文件地址")] string? basePath, [Description("文件数据内容")] string? content)
        {
            Console.WriteLine();
            Console.WriteLine($"🔧 正在调用 SaveFileContent 上传远程文件 ");
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    return "❌ 保存失败: fileName 不能为空。";

                if (string.IsNullOrWhiteSpace(basePath) && string.IsNullOrWhiteSpace(content))
                    return "❌ 保存失败:basePath和content不能同时为空。";

                if (!string.IsNullOrEmpty(basePath))
                {
                    if (!File.Exists(basePath))
                    {
                        return $"❌ 保存失败: 文件不存在 {basePath}";
                    }
                    var upload = _fileStorage.FileUpload(basePath, "/Files/" + DateTime.Now.ToString("yyyy/MM/dd"), fileName);
                    if (upload.Item1)
                    {
                        Common.IO.IOHelper.DeleteFile(basePath);
                        return upload.Item2;
                    }
                    else
                    {
                        return "❌ 文件上传保存失败";
                    }
                }
                else
                {
                    var encoding = new UTF8Encoding(false); // 默认 UTF-8 无 BOM 
                    string safeName = StringHelper.MakeSafeFileName(fileName);
                    string basepath = "/Files/" + DateTime.Now.ToString("yyyy/MM/dd");
                    string filepath = Kevin.Common.App.IO.Path.ContentRootPath() + basepath;
                    Directory.CreateDirectory(filepath);
                    fileName = Guid.NewGuid().ToString("N") + "-" + fileName;
                    string fullPath = Path.Combine(filepath, fileName);
                    File.WriteAllText(fullPath, content ?? string.Empty, encoding);
                    var upload = _fileStorage.FileUpload(fullPath, basepath, fileName);
                    if (upload.Item1)
                    {
                        Common.IO.IOHelper.DeleteFile(fullPath);
                        return upload.Item2;
                    }
                    else
                    {
                        return "❌ 文件上传保存失败";
                    }
                }

            }
            catch (Exception ex)
            {

                return "❌ 保存失败: 发生异常。" + ex.Message;
            }

        }
    }
}
