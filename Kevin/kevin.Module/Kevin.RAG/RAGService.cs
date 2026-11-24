using Kevin.RAG.Dto;
using Kevin.RAG.Interfaces;
using Kevin.RAG.Rerank;
using Microsoft.Extensions.AI;
using TencentCloud.Lke.V20231130.Models;

namespace Kevin.RAG
{
    public class RAGService : IRAGService
    {
        private IRAGStorageService RAGStorageService { get; set; }

        private IRerankService RerankService { get; set; }
        public RAGService(IRAGStorageService rAGStorageService, IRerankService rerankService)
        {
            RAGStorageService = rAGStorageService;
            RerankService = rerankService;
        }
        public async Task<(bool, string, List<DocumentChunkDto>)> GetRAGSystemPrompt(string collectionName, Embedding<float> question, string questionText = "", bool isRerankModel = false, int topK = 3, double? Score = null)
        {
            if (isRerankModel)
            {
                return await GetPriRAGSystemPrompt(collectionName, question, questionText, topK, Score, RerankService);
            }
            else
            {
                return await GetPriRAGSystemPrompt(collectionName, question, questionText, topK, Score);
            }
        }

        public async Task<(bool, string, List<DocumentChunkDto>)> GetRAGAliReankSystemPrompt(string collectionName, Embedding<float> question, string questionText = "", int topK = 3, double? Score = null, string RerankUrl = "", string RerankKey = "", string RerankModel = "")
        {
            if (!string.IsNullOrEmpty(RerankUrl) && !string.IsNullOrEmpty(RerankKey) && !string.IsNullOrEmpty(RerankModel))
            {
                var rerankService = new AliRerankService(RerankUrl, RerankModel, RerankKey);
                return await GetPriRAGSystemPrompt(collectionName, question, questionText, topK, Score, rerankService);
            }
            else
            {
                return await GetPriRAGSystemPrompt(collectionName, question, questionText, topK, Score);
            }
        }

        private async Task<(bool, string, List<DocumentChunkDto>)> GetPriRAGSystemPrompt(string collectionName, Embedding<float> question, string questionText = "", int topK = 3, double? Score = null, IRerankService? rerankService = default)
        {
            var documents = new List<DocumentChunkDto>();
            if (rerankService != default)
            {
                documents = await RAGStorageService.Search(collectionName, question, (ulong)(topK * 10));
                if (documents.Count > 0)
                {
                    // 重排模型对「单条文档」和「单次请求总量」都有 token 上限（部分模型单次仅 3 万～5 万），
                    // 超限直接 400（如 document index:N is invalid）。这里先做体积收敛，
                    // 避免历史脏数据（整张表格一个块）把预算打穿；块本身有界时这一步不会裁剪任何内容。
                    var (rerankInputs, trimmed) = BuildRerankInputs(documents);
                    if (trimmed)
                    {
                        documents = documents.Take(rerankInputs.Count).ToList();
                        Console.WriteLine($"Kevin.RAG重排输入超出模型预算，已收敛为 {rerankInputs.Count} 条候选");
                    }
                    try
                    {
                        var rerankData = await rerankService.RerankAsync(questionText, rerankInputs, topK);
                        for (int i = 0; i < documents.Count; i++)
                        {
                            documents[i].Score = rerankData.results.FirstOrDefault(t => t.index == i)?.relevance_score ?? documents[i].Score;
                        }
                    }
                    catch (Exception ex)
                    {
                        // 重排失败不能让整次知识库检索失败：退回向量相似度排序
                        Console.WriteLine($"Kevin.RAG重排失败，已退回向量排序：{ex.Message}");
                    }
                    if (Score != default)
                    {
                        documents = documents.Where(i => i.Score >= Score).ToList();
                    }
                    documents = documents.OrderByDescending(t => t.Score).Take(documents.Count <= topK ? documents.Count : topK).ToList();
                }
            }
            else
            {
                documents = await RAGStorageService.Search(collectionName, question, (ulong)topK, Score);
            }
            if (documents.Count == 0)
            {
                return (false, "\n" + "文档信息列表：无相关信息", new List<DocumentChunkDto>());
            }
            var context = string.Join("\n\n---\n\n", documents.Select((doc, index) =>
                $"文档 {doc.Title + "-" + index + 1}（来源：{doc.SourceFile}）（相似度：{doc.Score}）：\n{doc.Content}"));
            var userPrompt = $@"文档信息列表： 
                                文档内容：
                                {context} ";
            return (true, "\n" + userPrompt, documents);
        }

        /// <summary>送排单条文本上限（字符）：按中文约 1 字符≈1 token 保守估算，低于主流重排模型的单条上限</summary>
        private const int RerankMaxCharsPerDocument = 1500;

        /// <summary>送排候选总量上限（字符）：对应「单次请求最大 Token」的最小口径（如 gte-rerank-v2 的 30000）</summary>
        private const int RerankMaxTotalChars = 20000;

        /// <summary>送排候选条数上限</summary>
        private const int RerankMaxDocuments = 100;

        /// <summary>
        /// 构造送排文本：逐条截断超长内容，并在总预算用尽时停止追加。
        /// 返回的列表按传入顺序排列，重排结果的 index 可直接对应 documents 的下标。
        /// </summary>
        private static (List<string> Inputs, bool Trimmed) BuildRerankInputs(List<DocumentChunkDto> documents)
        {
            var candidates = documents.Take(RerankMaxDocuments).ToList();
            var inputs = new List<string>(candidates.Count);
            var totalChars = 0;
            foreach (var document in candidates)
            {
                var content = document.Content ?? string.Empty;
                if (content.Length > RerankMaxCharsPerDocument)
                {
                    content = content.Substring(0, RerankMaxCharsPerDocument);
                }
                if (inputs.Count > 0 && totalChars + content.Length > RerankMaxTotalChars)
                {
                    break;
                }
                totalChars += content.Length;
                inputs.Add(content);
            }

            return (inputs, inputs.Count < documents.Count);
        }
    }
}
