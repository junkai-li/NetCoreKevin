using Kevin.RAG.Dto;
using Kevin.RAG.Interfaces;
using Kevin.RAG.Qdrant.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace Kevin.RAG.Qdrant
{
    public class QdrantClientService : IRAGStorageService
    {
        private readonly string? Url;
        private readonly string? ApiKey;
        private readonly string? CertificateThumbprint;
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

        public async Task<bool> AddData(string collectionName, List<DocumentChunkDto> data, ulong embeddingValueSize = 2048)
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
            return relust.OrderByDescending(t => t.Score).ToList();
        }

        /// <summary>
        /// 按来源文件删除向量点：多文件知识库重导/移除单个文件时只清该文件的块。
        /// 集合不存在时直接返回成功，重复调用不会报错。
        /// </summary>
        public async Task<bool> DeleteData(string collectionName, string sourceFile)
        {
            if (QdrantClient == default)
            {
                throw new ArgumentException($"请检查QdrantClient配置是否正确");
            }
            if (string.IsNullOrWhiteSpace(collectionName) || string.IsNullOrWhiteSpace(sourceFile))
            {
                return false;
            }
            if (!await IsValidateCollectionExists(collectionName))
            {
                return true;
            }
            // 按 payload 的 SourceFile 精确匹配（写入时已落在 payload 上，无需额外索引）
            var filter = new Filter { Must = { Conditions.MatchKeyword("SourceFile", sourceFile) } };
            await QdrantClient.DeleteAsync(collectionName, filter);
            return true;
        }

        /// <summary>
        /// 删除整个集合：知识库删除时调用，避免集合永久残留（否则重导会把旧块/旧维度点一起检出来）。
        /// 集合不存在时直接返回成功，重复删除不会报错。
        /// </summary>
        public async Task<bool> DeleteCollection(string collectionName)
        {
            if (QdrantClient == default)
            {
                throw new ArgumentException($"请检查QdrantClient配置是否正确");
            }
            if (string.IsNullOrWhiteSpace(collectionName))
            {
                return false;
            }
            if (!await IsValidateCollectionExists(collectionName))
            {
                return true;
            }
            await QdrantClient.DeleteCollectionAsync(collectionName);
            return true;
        }

        public void Dispose()
        {
            QdrantClient.Dispose();
        }
    }
}
