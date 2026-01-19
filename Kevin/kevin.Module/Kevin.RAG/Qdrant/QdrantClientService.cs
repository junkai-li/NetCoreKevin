using Kevin.RAG.Dto;
using Kevin.RAG.Ollama;
using Kevin.RAG.Qdrant.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OllamaSharp;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace Kevin.RAG.Qdrant
{
    public class QdrantClientService : IQdrantClientService
    {
        private readonly string Url;
        private readonly string ApiKey;
        private readonly string CertificateThumbprint; 
        private readonly QdrantClient QdrantClient; 
        public QdrantClientService(IOptionsMonitor<QdrantClientSetting> config)
        {
            try
            {
                Url = config.CurrentValue.Url;
                ApiKey = config.CurrentValue.ApiKey;
                CertificateThumbprint = config.CurrentValue.CertificateThumbprint;  
                if (!string.IsNullOrEmpty(Url))
                {
                    if (!string.IsNullOrEmpty(ApiKey))
                    {
                        var channel = QdrantChannel.ForAddress(Url, new ClientConfiguration
                        {
                            ApiKey = ApiKey,
                            CertificateThumbprint = CertificateThumbprint
                        });
                        var grpcClient = new QdrantGrpcClient(channel);
                        QdrantClient = new QdrantClient(grpcClient);
                    }
                    else
                    {
                        QdrantClient = new QdrantClient(Url);
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Kevin.RAG请检查Qdrant配置是否正确");
            }

        }
         
        public async Task<bool> AddData(string collectionName, List<DocumentChunkDto> data, ulong embeddingValueSize=2048)
        {
            if (QdrantClient == default)
            {
                throw new ArgumentException($"请检查QdrantClient配置是否正确");
            }
            if (QdrantClient != null)
            { 
                var points = data.Select(i => new PointStruct
                {
                    Id = (ulong)i.Id,
                    Vectors = i.ContentVector.Vector.ToArray(),
                    /*# 与 collection 的 vector size 对应*/
                    Payload =
                     {
                       ["Content"]   = i.Content,
                       ["SourceFile"] = i.SourceFile,
                       ["Title"] = i.Title,
                       ["Category"] = i.Category,
                       ["ChunkIndex"] = i.ChunkIndex,
                       ["CreatedAt"]=i.CreatedAt.ToString()
                     }
                }).ToList();
                if (!(await IsValidateCollectionExists(collectionName)))
                {
                    await QdrantClient.CreateCollectionAsync(collectionName, new VectorParams { Size = embeddingValueSize, Distance = Distance.Cosine });
                }
                var result = await QdrantClient.UpsertAsync(collectionName, points);
                return true;
            }
            return false;
        }

        // 验证集合是否存在
        public async Task<bool> IsValidateCollectionExists(string collectionName)
        {
            if (QdrantClient == default)
            {
                throw new ArgumentException($"请检查OllamaApiQdrantClientQdrantClient配置是否正确");
            }
            try
            {
                var collectionNameInfo = await QdrantClient.GetCollectionInfoAsync(collectionName);
            }
            catch (Exception)
            {

                return false;
            }
            return true;

        }

        public async Task<List<DocumentChunkDto>> Search(string collectionName,
            Embedding<float> query, ulong limit = 10, double? Score = null)
        {
            if (QdrantClient == default)
            {
                throw new ArgumentException($"请检查QdrantClient配置是否正确");
            }
            var data = await QdrantClient.SearchAsync(collectionName, query.Vector, limit: limit);
            var relust = data.Select(i =>
            {
                var payload = i.Payload;
                return new DocumentChunkDto
                {
                    Id = long.Parse(i.Id.Num.ToString()),
                    Content = payload["Content"].StringValue,
                    SourceFile = payload["SourceFile"].StringValue,
                    Title = payload["Title"].StringValue,
                    Category = payload["Category"].StringValue,
                    ChunkIndex = Convert.ToInt32(payload["ChunkIndex"].IntegerValue),
                    CreatedAt = DateTimeOffset.Parse(payload["CreatedAt"].StringValue ?? string.Empty),
                    Score = i.Score
                };
            }).ToList();
            if (Score != default)
            {
                relust = relust.Where(i => i.Score >= Score).ToList();
            }
            return relust;
        }

        public void Dispose()
        {
            QdrantClient.Dispose();
        }
    }
}
