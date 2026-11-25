using System.Reflection;
using FakeItEasy;
using kevin.Application;
using kevin.Application.Services.AI.Tools;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using Kevin.RAG.Interfaces;
using Kevin.RAG.Ollama;
using Kevin.SnowflakeId.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.AI;
using Web.Global.User;

namespace Kevin.Unit.Kevin.Services
{
    /// <summary>
    /// BuildUniqueToolName 工具名唯一化单元测试（TASK-04 / FIX-01 回归）：
    /// 覆盖多知识库合并挂载时的工具名生成 —— 合法名直拼、非法字符净化、中文兜底 kms_{kmsId}、
    /// 超长截断限 40、同一 scoped 实例内重名 _2 序号去重、跨实例不共享去重集合。
    /// BuildUniqueToolName 为 private 纯逻辑方法，通过反射调用（不触库、不触向量检索）。
    /// </summary>
    public class KnowledgeSearchToolServiceToolNameTests
    {
        private static MethodInfo _buildNameMethod = null!;

        public KnowledgeSearchToolServiceToolNameTests()
        {
            _buildNameMethod = typeof(KnowledgeSearchToolService).GetMethod(
                "BuildUniqueToolName", BindingFlags.Instance | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException("未找到 BuildUniqueToolName 私有方法（FIX-01 实现被改动？）");
        }

        private static KnowledgeSearchToolService CreateService()
        {
            var httpAccessor = A.Fake<IHttpContextAccessor>();
            A.CallTo(() => httpAccessor.HttpContext).Returns((HttpContext?)null);
            return new KnowledgeSearchToolService(
                httpAccessor,
                A.Fake<IAIKmssService>(),
                A.Fake<IAIModelsService>(),
                A.Fake<IRAGService>(),
                A.Fake<IOllamaApiService>())
            {
                HttpContextAccessor = httpAccessor,
                CurrentUser = new CurrentUser(httpAccessor, null),
                SnowflakeIdService = new SnowflakeIdService()
            };
        }

        private static string InvokeName(KnowledgeSearchToolService service, string kmsName, long kmsId)
        {
            return (string)_buildNameMethod.Invoke(service, new object[] { kmsName, kmsId })!;
        }

        [Fact]
        public void ToolName_WithAsciiName_ShouldUseSanitizedSuffix()
        {
            // 空格与特殊字符均被过滤，仅保留字母/数字/下划线
            var service = CreateService();
            Assert.Equal("SearchKnowledge_mykms", InvokeName(service, "my kms!@#", 1));
            Assert.Equal("SearchKnowledge_my_kms", InvokeName(service, "my_kms", 1));
        }

        [Fact]
        public void ToolName_WithChineseOnlyName_ShouldFallbackToKmsId()
        {
            var service = CreateService();
            Assert.Equal("SearchKnowledge_kms_7", InvokeName(service, "产品知识库", 7));
        }

        [Fact]
        public void ToolName_WithNullOrEmptyName_ShouldFallbackToKmsId()
        {
            var service = CreateService();
            Assert.Equal("SearchKnowledge_kms_8", InvokeName(service, "", 8));
            Assert.Equal("SearchKnowledge_kms_9", InvokeName(service, null!, 9));
        }

        [Fact]
        public void ToolName_WithOverlongName_ShouldTruncateTo40()
        {
            var longName = new string('a', 41);
            var service = CreateService();
            Assert.Equal("SearchKnowledge_" + new string('a', 40), InvokeName(service, longName, 1));
        }

        [Fact]
        public void ToolName_WithinSameInstance_ShouldDeduplicateWithSeqSuffix()
        {
            // 同一 scoped 实例（同一次请求）挂载多个同名知识库：function name 唯一（_2/_3 序号）
            var service = CreateService();
            Assert.Equal("SearchKnowledge_docs", InvokeName(service, "docs", 1));
            Assert.Equal("SearchKnowledge_docs_2", InvokeName(service, "docs", 2));
            Assert.Equal("SearchKnowledge_docs_3", InvokeName(service, "docs", 3));
        }

        [Fact]
        public void ToolName_AcrossInstances_ShouldNotShareDedupSet()
        {
            // scoped 语义：去重集合按实例隔离，不同请求互不影响
            var s1 = CreateService();
            var s2 = CreateService();
            Assert.Equal("SearchKnowledge_docs", InvokeName(s1, "docs", 1));
            Assert.Equal("SearchKnowledge_docs", InvokeName(s2, "docs", 1));
        }

        #region BuildSearchFunction 终态验证：多库挂载后 function name 在工具集合内唯一

        /// <summary>桩定 GetDetails：两个同名知识库（净化后后缀碰撞）</summary>
        private static KnowledgeSearchToolService CreateServiceWithKmss()
        {
            var kmssService = A.Fake<IAIKmssService>();
            A.CallTo(() => kmssService.GetDetails(1)).Returns(new AIKmssDto { Id = 1, Name = "docs" });
            A.CallTo(() => kmssService.GetDetails(2)).Returns(new AIKmssDto { Id = 2, Name = "docs" });
            var httpAccessor = A.Fake<IHttpContextAccessor>();
            A.CallTo(() => httpAccessor.HttpContext).Returns((HttpContext?)null);
            return new KnowledgeSearchToolService(
                httpAccessor,
                kmssService,
                A.Fake<IAIModelsService>(),
                A.Fake<IRAGService>(),
                A.Fake<IOllamaApiService>())
            {
                HttpContextAccessor = httpAccessor,
                CurrentUser = new CurrentUser(httpAccessor, null),
                SnowflakeIdService = new SnowflakeIdService()
            };
        }

        private static AIAppsDto CreateAiapp()
        {
            // 挂载参数走默认空值即可：BuildSearchFunction 构造期不触向量检索
            return new AIAppsDto();
        }

        [Fact]
        public async Task BuildSearchFunction_TwoSameNamedKmss_ShouldProduceUniqueFunctionNames()
        {
            // Arrange：模拟多库合并挂载（同名知识库），同一 scoped 实例连续构建两个工具
            var service = CreateServiceWithKmss();
            var aiapp = CreateAiapp();

            // Act
            var t1 = await service.BuildSearchFunction(aiapp, 1);
            var t2 = await service.BuildSearchFunction(aiapp, 2);

            // Assert：ChatOptions.Tools 中不得出现重名 function（否则模型网关 400 或错路由）
            Assert.NotNull(t1);
            Assert.NotNull(t2);
            Assert.NotEqual(t1!.Name, t2!.Name);
            Assert.Equal("SearchKnowledge_docs", t1.Name);
            Assert.Equal("SearchKnowledge_docs_2", t2.Name);
        }

        [Fact]
        public async Task BuildSearchFunction_SingleDbPath_ShouldDelegateToSameNamingLogic()
        {
            // Arrange：单库路径（原单参方法 ≡ BuildSearchFunction(aiapp, aiapp.KmsId)）
            var service = CreateServiceWithKmss();
            var aiapp = new AIAppsDto { KmsId = 1 };

            // Act：单库路径（单参）与多库路径（双参）构建同一知识库
            var single = await service.BuildSearchFunction(aiapp);
            var multi = await service.BuildSearchFunction(aiapp, 1);

            // Assert：同一实例上同名同 Id → 单库/多库命名统一，多库路径追加序号保持唯一
            Assert.NotNull(single);
            Assert.NotNull(multi);
            Assert.Equal("SearchKnowledge_docs", single!.Name);
            Assert.Equal("SearchKnowledge_docs_2", multi!.Name);
        }

        [Fact]
        public async Task BuildSearchFunction_MissingKms_ShouldReturnNull()
        {
            // Arrange：知识库不存在/无数据权限（isDataPer 查不到）→ 调用方跳过 + Warn
            var kmssService = A.Fake<IAIKmssService>();
            // 显式桩定为 null（FakeItEasy 默认会隐式生成假对象，掩盖"查不到"分支）
            A.CallTo(() => kmssService.GetDetails(A<long>._)).Returns((AIKmssDto?)null);
            var httpAccessor = A.Fake<IHttpContextAccessor>();
            A.CallTo(() => httpAccessor.HttpContext).Returns((HttpContext?)null);
            var service = new KnowledgeSearchToolService(
                httpAccessor,
                kmssService,
                A.Fake<IAIModelsService>(),
                A.Fake<IRAGService>(),
                A.Fake<IOllamaApiService>())
            {
                HttpContextAccessor = httpAccessor,
                CurrentUser = new CurrentUser(httpAccessor, null),
                SnowflakeIdService = new SnowflakeIdService()
            };

            // Act & Assert：不抛错返回 null
            Assert.Null(await service.BuildSearchFunction(CreateAiapp(), 999));
        }

        #endregion
    }
}
