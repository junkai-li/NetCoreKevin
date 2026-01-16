using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.RAG
{
    public interface IRAGService
    {
        /// <summary>
        /// 获取向量搜索服务的系统提示词
        /// </summary>
        /// <param name="question"></param>
        /// <param name="topK"></param>
        /// <returns></returns>
        Task<(bool, string)> GetSystemPrompt(string collectionName, string question, int topK = 3, double? Score = null);
    }
}
