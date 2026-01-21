using Kevin.RAG.Dto;
using Kevin.RAG.Ollama;
using Kevin.RAG.Qdrant;
using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.RAG
{
    public class RAGService : IRAGService
    {
        private IQdrantClientService QdrantClientService { get; set; }
        public RAGService(IQdrantClientService qdrantClientService)
        {
            QdrantClientService = qdrantClientService;
        }
        public async Task<(bool, string, List<DocumentChunkDto>)> GetSystemPrompt(string collectionName, Embedding<float> question, int topK = 3, double? Score = null)
        {
            Console.WriteLine($"\n问题：{question}");
            Console.WriteLine("正在检索相关文档...");
            var documents = await QdrantClientService.Search(collectionName, question, (ulong)topK, Score);
            // 4. 构建提示词
            var systemPrompt = @" 
                                重要规则：
                                1. 只使用文档中的信息来回答
                                2. 如果文档中没有相关信息，请明确告知用户
                                3. 不要编造或推测文档中没有的信息
                                4. 回答要清晰、准确、有条理
                                5. 可以引用文档来源 引用时必须选择相似度最高的文档";
            if (documents.Count == 0)
            {
                return (false, systemPrompt+"\n" +"抱歉，我没有找到相关的文档来回答您的问题。",new List<DocumentChunkDto>());
            }
            Console.WriteLine($"找到 {documents.Count} 个相关文档");
            // 3. 构建上下文
            var context = string.Join("\n\n---\n\n", documents.Select((doc, index) =>
                $"文档 {doc.Title +"-"+ index + 1}（来源：{doc.SourceFile}）（相似度：{doc.Score}）：\n{doc.Content}"));
            Console.WriteLine(context);

            var userPrompt = $@"文档内容：
                                {context} 
                                用户问题：
                                {question} 
                                请基于以上文档内容回答问题。"; 
            return (true, systemPrompt + "\n" + userPrompt, documents);
        }
    }
}
