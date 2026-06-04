using Common;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.FileStorage;
using System.ComponentModel;
using System.Text;

namespace kevin.Application.Services.AI
{
    public class AIFileToolService : BaseService, IAIFileToolService
    {
        public readonly IFileStorage _fileStorage;
        public AIFileToolService(IHttpContextAccessor _httpContextAccessor, IFileStorage fileStorage) : base(_httpContextAccessor)
        {
            this._fileStorage = fileStorage;
        }
        public string SaveFileContent([Description("文件名称如（xx.html,xx.txt）支持各种文件类型")] string fileName, [Description("内容")] string content)
        {
            Console.WriteLine();
            Console.WriteLine($"🔧 正在调用 SaveFileContent 上传远程文件 ");
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    return "❌ 保存失败: fileName 不能为空。";

                if (string.IsNullOrWhiteSpace(content))
                    return  "❌ 保存失败: content 不能为空。";

                var encoding = new UTF8Encoding(false); // 默认 UTF-8 无 BOM 
                string safeName = StringHelper.MakeSafeFileName(fileName);
                string basepath = "/Files/" + DateTime.Now.ToString("yyyy/MM/dd");
                string filepath = Kevin.Common.App.IO.Path.ContentRootPath() + basepath;
                Directory.CreateDirectory(filepath);
                fileName = SnowflakeIdService.GetNextId() + "-" + fileName;
                string fullPath = Path.Combine(filepath, fileName);
                File.WriteAllText(fullPath, content ?? string.Empty, encoding);
                var upload = _fileStorage.FileUpload(fullPath, basepath, fileName);
                if (upload.Item1)
                {
                    Common.IO.IOHelper.DeleteFile(fullPath);
                    return  upload.Item2;
                }
                else
                {
                    return "❌ 文件上传保存失败";
                } 
            }
            catch (Exception ex)
            {

                return "❌ 保存失败: 发生异常。" + ex.Message;
            }

        }
    }
}
