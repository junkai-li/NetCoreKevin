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
                var rerankService = new AliRerankService(RerankUrl,RerankModel,RerankKey);
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
                    var rerankData = await rerankService.RerankAsync(questionText, documents.Select(t => t.Content).ToList(), topK);
                    for (int i = 0; i < documents.Count; i++)
                    {
                        documents[i].Score = rerankData.results.FirstOrDefault(t => t.index == i)?.relevance_score;
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
    }
}
