using Common;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using kevin.Domain.Share.Enums;
using Kevin.Common.Extension;
using Kevin.log4Net;
using Kevin.RAG.Interfaces;
using Kevin.RAG.Ollama;
using Microsoft.Extensions.AI;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// <see cref="IAKnowledgeSearchToolService"/> 默认实现：把知识库向量检索（RAG + 可选 Rerank）
    /// 封装为模型可调用的 <c>SearchKnowledge</c> AIFunction。
    /// <para>
    /// 与 <see cref="kevin.AI.AgentFramework.ImageGeneration.ImageGenToolFactory"/> 同一套路：检索所需的
    /// collection/模型配置通过闭包捕获，模型只看到 query 一个业务参数；
    /// 工具内部按知识库配置自建向量化客户端（不替换容器里的默认 <see cref="IOllamaApiService"/>，
    /// 避免旧 KmsRag 实现那种改写 scoped 字段串知识库的隐患）。
    /// </para>
    /// </summary>
    public class KnowledgeSearchToolService : BaseService, IAKnowledgeSearchToolService
    {
        private readonly IAIKmssService _aIKmssService;
        private readonly IAIModelsService _aIModelsService;
        private readonly IRAGService _rAGService;
        private readonly IOllamaApiService _ollamaApiService;

        public KnowledgeSearchToolService(IHttpContextAccessor _httpContextAccessor,
            IAIKmssService aIKmssService, IAIModelsService aIModelsService,
            IRAGService rAGService, IOllamaApiService ollamaApiService) : base(_httpContextAccessor)
        {
            this._aIKmssService = aIKmssService;
            this._aIModelsService = aIModelsService;
            this._rAGService = rAGService;
            this._ollamaApiService = ollamaApiService;
        }

        /// <inheritdoc />
        public async Task<AITool?> BuildSearchFunction(AIAppsDto aiapp)
        {
            if (aiapp.KmsId == default) return null;
            var kmss = await _aIKmssService.GetDetails(aiapp.KmsId.GetValueOrDefault());
            if (kmss == default) return null;

            // 向量化模型：知识库指定了 Embedding 模型则用它，否则回落全局 OllamaApi 配置
            IOllamaApiService embedder = _ollamaApiService;
            if (kmss.aIModelsId != default)
            {
                var embedModel = await _aIModelsService.GetNoPerDetails(kmss.aIModelsId.GetValueOrDefault());
                if (embedModel?.AIModelType == AIModelType.Embedding)
                {
                    embedder = new OllamaApiService(embedModel.EndPoint, embedModel.ModelName, embedModel.ModelKey);
                }
            }

            // 重排模型：配置了且类型是 Rerank 才走 Rerank 分支，否则纯向量检索
            AIModelsDto? rerankModel = null;
            if (kmss.aIRerankModelsId != default)
            {
                var candidate = await _aIModelsService.GetNoPerDetails(kmss.aIRerankModelsId.ToTryInt64());
                if (candidate?.AIModelType == AIModelType.Rerank)
                {
                    rerankModel = candidate;
                }
            }

            var tool = new KnowledgeSearchTool(embedder, _rAGService, kmss.Id,
                aiapp.MaxMatchesCount, aiapp.Relevance / 100, rerankModel, aiapp.ContentLengthLimit);
            return AIFunctionFactory.Create(tool.SearchKnowledge, new AIFunctionFactoryOptions
            {
                Name = "SearchKnowledge",
                Description = $"搜索知识库「{kmss.Name}」。当用户问题涉及知识库中的资料、规章制度、产品或专业知识时调用；query 用精炼的检索词或一句完整的问题表述。返回相关文档片段列表，返回 ❌ 开头表示无结果或失败，此时禁止编造知识库内容。"
            });
        }

        /// <summary>
        /// 单次工具调用的承载类：捕获知识库与智能体检索配置，暴露符合 AIFunction 反射规范的实例方法。
        /// </summary>
        private sealed class KnowledgeSearchTool
        {
            private readonly IOllamaApiService _embedder;
            private readonly IRAGService _ragService;
            private readonly string _collectionName;
            private readonly int _topK;
            private readonly double _score;
            private readonly AIModelsDto? _rerankModel;
            private readonly int _contentLengthLimit;

            public KnowledgeSearchTool(IOllamaApiService embedder, IRAGService ragService, long kmsId,
                int topK, double score, AIModelsDto? rerankModel, int contentLengthLimit)
            {
                _embedder = embedder;
                _ragService = ragService;
                _collectionName = "AIKmss-" + kmsId.ToString();
                _topK = topK;
                _score = score;
                _rerankModel = rerankModel;
                _contentLengthLimit = contentLengthLimit;
            }

            /// <summary>
            /// AIFunction 入口方法：向量检索（可选 Rerank）后返回文档片段列表；
            /// 无结果/异常时返回 <c>❌ ...</c> 前缀，与项目其他工具的错误约定一致，模型能理解并如实告知用户。
            /// </summary>
            [Description("搜索知识库，返回相关文档片段列表。")]
            public async Task<string> SearchKnowledge(
                [Description("检索内容：针对知识库的主题词或完整问题表述")][Required] string query)
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return "❌ 搜索失败: query 不能为空";
                }
                try
                {
                    var embedding = await _embedder.GetEmbedding(query);
                    var result = _rerankModel == null
                        ? await _ragService.GetRAGSystemPrompt(_collectionName, embedding, query, false, _topK, _score)
                        : await _ragService.GetRAGAliReankSystemPrompt(_collectionName, embedding, query, _topK, _score,
                            _rerankModel.EndPoint, _rerankModel.ModelKey, _rerankModel.ModelName);
                    if (!result.Item1 || result.Item3.Count == 0)
                    {
                        return "❌ 知识库中未找到与该检索内容相关的文档，可换个检索词重试或如实告知用户知识库暂无相关资料";
                    }
                    return StringHelper.SubstringText($"知识库「{_collectionName}」检索到 {result.Item3.Count} 个相关文档：{result.Item2}", _contentLengthLimit);
                }
                catch (Exception ex)
                {
                    LogHelper.logger.Error("知识库搜索工具调用失败:", ex);
                    return $"❌ 搜索失败: {ex.Message}";
                }
            }
        }
    }
}
