using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kevin.Domain.Interfaces.IServices.AI
{
    public interface IAIFileToolService
    {
        [Description("保存文件到远程文件服务器并返回访问url。 用于把各种文件输出到远程文件服务器，返回完整url路径或以 ❌ 开头的错误信息。")]
        string SaveFileContent([Description("文件名称如（xx.html,xx.txt）支持各种文件类型")][Required] string fileName, [Description("内容")][Required] string content);
    }
}
