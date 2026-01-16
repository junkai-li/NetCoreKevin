using Kevin.RAG.Dto;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.RAG.Qdrant
{
    public interface IQdrantClientService: IDisposable
    { 

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> AddData(string collectionName, List<DocumentChunkDto> data);

        /// <summary>
        ///  搜索向量数据 向量搜索：根据查询向量找到最相关的文档
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="query"></param>
        /// <param name="limit">返回多少数据</param>
        /// <param name="Score">相似度</param> 
        /// <returns></returns>
        Task<List<DocumentChunkDto>> Search(string collectionName,
            String query, ulong limit = 10, double? Score = null); 

    }
}
