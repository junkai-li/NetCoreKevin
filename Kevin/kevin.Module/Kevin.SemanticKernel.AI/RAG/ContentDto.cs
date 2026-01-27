using DocumentFormat.OpenXml.EMMA;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kevin.AI.Dto
{
    /// <summary>
    /// 矢量数据库存储结构
    /// </summary>
    public class ContentDto
    {   
        public required List<NodeDto> Nodes { get; set; }

        public class NodeDto
        {
            [VectorStoreKey]
            public Guid Key { get; set; }

            [VectorStoreData]
            public string Name { get; set; }

            [VectorStoreVector(1536)]
            public Embedding<float> NameEmbedding { get; set; }

            /// <summary>
            /// 分块的内容
            /// </summary>   
            [VectorStoreData]
            public required string Description { get; set; }

            [VectorStoreVector(1024)]
            public   Embedding<float> DescriptionEmbedding { get; set; }
             
            public override string ToString()
            {
                return Description;
            }
        }

       
    }
}
