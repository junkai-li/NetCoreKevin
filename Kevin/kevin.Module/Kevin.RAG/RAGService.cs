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
            var documents = await QdrantClientService.Search(collectionName, question, (ulong)topK, Score); 
            if (documents.Count == 0)
            { 
                return (false,  "\n" + "文档信息列表：无相关信息", new List<DocumentChunkDto>());
            }  
            var context = string.Join("\n\n---\n\n", documents.Select((doc, index) =>
                $"文档 {doc.Title +"-"+ index + 1}（来源：{doc.SourceFile}）（相似度：{doc.Score}）：\n{doc.Content}")); 
            var userPrompt = $@"文档信息列表： 
                                文档内容：
                                {context} "; 
            return (true,   "\n" + userPrompt, documents);
        }
    }
}
