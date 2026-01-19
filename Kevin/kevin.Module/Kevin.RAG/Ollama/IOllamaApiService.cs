using Microsoft.Extensions.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.RAG.Ollama
{
    public interface IOllamaApiService:IDisposable
    {
        /// <summary>
        /// 返回矢量数据
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        Task<Embedding<float>> GetEmbedding(string text);
    }
}
