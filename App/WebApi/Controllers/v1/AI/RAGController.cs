using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Attributes;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.AI;
using kevin.Permission.Permisson.Attributes;
using Kevin.Common.Extension;
using Kevin.RAG;
using Kevin.RAG.Dto;
using Kevin.RAG.Qdrant;
using Kevin.RAG.Tools;
using Kevin.SnowflakeId.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel.Services;
using System.Threading.Tasks;

namespace App.WebApi.Controllers.v1.AI
{
    /// <summary>
    /// AI检索增强生成（RAG）控制器    
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RAGController : ControllerBase
    {
        private ISnowflakeIdService _snowflakeIdService { get; set; }

        private IRAGService _rAGServicevice { get; set; }

        private IQdrantClientService _qdrantClientService { get; set; }
        public RAGController(IRAGService rAGServicevice, IQdrantClientService qdrantClientService, ISnowflakeIdService snowflakeIdService)
        {
            _rAGServicevice = rAGServicevice;
            _qdrantClientService = qdrantClientService;
            _snowflakeIdService = snowflakeIdService;
        }
        /// <summary>
        /// 测试
        /// </summary> 
        /// <returns></returns>
        [HttpGet("TestRag")] 
        public async Task<bool> TestRag()
        {
//            Console.WriteLine("=== RAG 系统演示 ===\n");
//            // 4. 准备示例文档
//            var sampleDocuments = new Dictionary<string, string>
//            {
//                ["产品手册.txt"] = @"
//我们的智能音箱支持多种功能：

//语音控制：
//- 唤醒词是'你好小智'
//- 可以控制音乐播放、查询天气、设置闹钟等
//- 支持中文和英文语音识别

//音乐播放：
//- 支持 QQ 音乐、网易云音乐、酷狗音乐
//- 可以通过语音控制播放、暂停、切歌
//- 支持歌单、专辑、歌手搜索

//智能家居：
//- 可以控制智能灯泡、插座、空调、窗帘
//- 支持场景模式（如回家模式、睡眠模式）
//- 可以设置定时任务",

//                ["FAQ.txt"] = @"
//常见问题：

//Q: 如何连接 WiFi？
//A: 首次使用时，打开手机 App，选择'添加设备'，按照提示连接 WiFi。

//Q: 如何重置设备？
//A: 长按设备顶部的重置按钮 10 秒，直到指示灯闪烁。

//Q: 支持哪些音乐服务？
//A: 目前支持 QQ 音乐、网易云音乐和酷狗音乐。

//Q: 如何更新固件？
//A: 设备会自动检查更新，也可以在 App 中手动检查更新。",

//                ["技术规格.txt"] = @"
//技术规格：

//硬件：
//- 处理器：四核 ARM Cortex-A53
//- 内存：1GB RAM
//- 存储：8GB Flash
//- 扬声器：5W 全频扬声器
//- 麦克风：4 麦克风阵列

//连接：
//- WiFi：2.4GHz/5GHz 双频
//- 蓝牙：5.0
//- 接口：USB-C 电源接口

//尺寸和重量：
//- 尺寸：100mm × 100mm × 50mm
//- 重量：300g
//- 电源：12V/1.5A"
//            };

//            // 5. 处理和上传文档
//            Console.WriteLine("\n正在处理文档...");
//            var allChunks = new List<DocumentChunkDto>();
//            // 2. 初始化服务
//            var documentProcessor = new DocumentProcessor(chunkSize: 500, chunkOverlap: 50);
//            foreach (var (fileName, content) in sampleDocuments)
//            {
//                // 清理文档
//                var cleanedContent = documentProcessor.CleanDocument(content);

//                // 分块
//                var chunks = documentProcessor.ChunkByParagraph(cleanedContent);

//                Console.WriteLine($"文档 '{fileName}' 分成了 {chunks.Count} 个块"); 
               

//                // 创建文档块对象
//                for (int i = 0; i < chunks.Count; i++)
//                {
//                    allChunks.Add(new DocumentChunkDto
//                    {
//                        Content = chunks[i], 
//                        SourceFile = fileName,
//                        Id= _snowflakeIdService.GetNextId(),
//                        CreatedAt=DateTime.Now,
//                        Title = fileName.Replace(".txt", ""),
//                        Category = "产品文档",
//                        ChunkIndex = i
//                    });
//                }
//            }
//            Console.WriteLine($"\n正在上传 {allChunks.Count} 个文档块到向量数据库...");
//            // 上传到向量数据库
//            await _qdrantClientService.AddData("RAG_Documents", allChunks);
            // 6. 测试 RAG 查询
            Console.WriteLine("\n=== 开始测试 RAG 查询 ===\n");
            var testQuestions = new[]
            {
                "这个音箱支持哪些音乐服务？",
                "如何连接 WiFi？",
                "音箱的重量是多少？",
                "可以控制哪些智能家居设备？"
            };
            foreach (var question in testQuestions)
            {
                var answer = await _qdrantClientService.Search("RAG_Documents", question);
                Console.WriteLine($"答案：{answer.ToJson()}");
                Console.WriteLine(new string('-', 80));
                Console.WriteLine( await _rAGServicevice.GetSystemPrompt("RAG_Documents",question));
            }
            Console.WriteLine("\n感谢使用 RAG 系统！");
            
            return true;
        }
    }
}
