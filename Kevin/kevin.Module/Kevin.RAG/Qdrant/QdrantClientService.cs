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
        /// 按来源文件分页拉取文档碎片：CountAsync 精确计数 + ScrollAsync 按 ChunkIndex 升序取当前页
        /// （预览只读，不加分布式锁）。集合不存在或参数为空时返回空结果，不抛异常（与 DeleteData 容错一致）。
        /// <para>
        /// Qdrant.Client 1.17.0 的 ScrollAsync offset 参数是 PointId（点 ID 起点）而非数值偏移，
        /// 因此按 ChunkIndex 排序预取 offset+limit 条，再在内存中切出 [offset, offset+limit) 页；
        /// Scroll 后仍做一次内存排序兜底，块序在服务端排序不可用时也保持稳定。
        /// </para>
        /// </summary>
        public async Task<(ulong Total, List<DocumentChunkDto> Chunks)> GetChunksBySourceFile(
            string collectionName, string sourceFile, ulong limit, ulong offset)
        {
            if (QdrantClient == default)
            {
                throw new ArgumentException($"请检查QdrantClient配置是否正确");
            }
            if (string.IsNullOrWhiteSpace(collectionName) || string.IsNullOrWhiteSpace(sourceFile))
            {
                return (0, new List<DocumentChunkDto>());
            }
            if (!await IsValidateCollectionExists(collectionName))
            {
                return (0, new List<DocumentChunkDto>());
            }
            // 按 payload 的 SourceFile 精确匹配（写入时已落在 payload 上，与 DeleteData 一致）
            var filter = new Filter { Must = { Conditions.MatchKeyword("SourceFile", sourceFile) } };
            // 不使用服务端 order_by：Qdrant Server 要求 ChunkIndex 已建 range payload 索引，否则
            // Scroll 直接报错（Wrong input: No range index for `order_by` key）。集合创建时未建该索引，
            // 故拉取全部匹配点后在内存中按 ChunkIndex 排序分页（单文件碎片量级小，代价可忽略）。
            var points = (await QdrantClient.ScrollAsync(collectionName, filter,
                100000u,
                null,
                new WithPayloadSelector { Enable = true },
                new WithVectorsSelector { Enable = false },
                null,
                null,
                null)).Result;
            // 同一文件被重复导入时会产生两组 ChunkIndex 相同的点（历史数据中确实存在）：
            // 按 (ChunkIndex, Content) 去重，保留最早导入的一组；total 也按去重后计数，分页才准确
            var uniqueChunks = points.Select(p =>
                {
                    var payload = p.Payload;
                    return new DocumentChunkDto
                    {
                        Id = long.Parse(p.Id.Num.ToString()),
                        Content = payload["Content"].StringValue,
                        SourceFile = payload["SourceFile"].StringValue,
                        Title = payload["Title"].StringValue,
                        Category = payload["Category"].StringValue,
                        ChunkIndex = Convert.ToInt32(payload["ChunkIndex"].IntegerValue),
                        CreatedAt = DateTimeOffset.Parse(payload["CreatedAt"].StringValue ?? string.Empty)
                    };
                })
                .GroupBy(c => c.ChunkIndex + "|" + c.Content)
                .Select(g => g.OrderBy(c => c.CreatedAt).First())
                .ToList();
            var total = (ulong)uniqueChunks.Count;
            var chunks = uniqueChunks
            // 服务端 OrderBy 不可用（低版本 Qdrant Server）时靠内存排序兜底
            .OrderBy(c => c.ChunkIndex)
            .Skip(offset > int.MaxValue ? int.MaxValue : (int)offset)
            .Take(limit > int.MaxValue ? int.MaxValue : (int)limit)
            .ToList();
            return (total, chunks);
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
