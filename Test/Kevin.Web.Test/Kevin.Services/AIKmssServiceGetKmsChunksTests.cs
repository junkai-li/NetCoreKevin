using FakeItEasy;
using kevin.Application.Services.AI;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories;
using kevin.Domain.Share.Dtos.AI;
using Kevin.RAG.Dto;
using Kevin.RAG.Interfaces;
using Kevin.RAG.Ollama;
using Kevin.Unit.Infrastructure;
using Medallion.Threading;
using Microsoft.AspNetCore.Http;
using Web.Global.Exceptions;
using Web.Global.User;

namespace Kevin.Unit.Kevin.Services
{
    /// <summary>
    /// AIKmssService.GetKmsChunks（TASK-04 知识库文件碎片预览）单元测试：
    /// 覆盖 ① 归属校验失败抛 UserFriendlyException（不存在/已删除，防越权）；② 分页参数钳制
    /// （pageNum&lt;1 归 1、pageSize 钳 1..100）且以正确的 limit/offset 传给 RAG 层；③ RAG 乱序返回时
    /// 按 ChunkIndex 升序映射 DTO；④ Qdrant 异常归一为空结果（total=0/list=[]，覆盖 CR-005 CreatedAt
    /// 脏数据在 RAG 层抛异常后的服务侧行为）；⑤ pageNum 极大时巨大 offset 不溢出服务层、RAG 异常
    /// 后归一空（CR-003 服务侧链路）；⑥ 空结果不抛异常；⑦ 只读不获取分布式锁。
    /// 仓储与 RAG 存储服务全部用 FakeItEasy 桩替 + 内存 IQueryable（TestAsyncEnumerable），
    /// 不触库、不触真实 Qdrant。RAG 层 Scroll/OrderBy 兼容性属 QdrantClient 内部（依赖真实
    /// Qdrant.Server），纳入联测范围，见 test-report.md。
    /// </summary>
    public class AIKmssServiceGetKmsChunksTests
    {
        private const long KmssId = 8001;
        private const int MyTenantId = 1000;
        private const string SourceFile = "contract.pdf";
        private const string CollectionName = "AIKmss-8001"; // GetCollectionName: "AIKmss-" + 8001

        private readonly IAIKmssRp _rp;
        private readonly IRAGStorageService _rag;
        private readonly ICurrentUser _user;
        private readonly IDistributedLockProvider _distLock;
        private readonly List<TAIKmss> _data;
        private readonly AIKmssService _service;

        public AIKmssServiceGetKmsChunksTests()
        {
            _rp = A.Fake<IAIKmssRp>();
            _rag = A.Fake<IRAGStorageService>();
            _user = A.Fake<ICurrentUser>();
            _distLock = A.Fake<IDistributedLockProvider>();
            A.CallTo(() => _user.TenantId).Returns(MyTenantId);
            _data = new List<TAIKmss>();
            A.CallTo(() => _rp.Query(A<bool>._, A<bool>._))
                .ReturnsLazily((bool _, bool _) => new TestAsyncEnumerable<TAIKmss>(_data));

            var httpAccessor = A.Fake<IHttpContextAccessor>();
            A.CallTo(() => httpAccessor.HttpContext).Returns((HttpContext?)null);
            _service = new AIKmssService(
                httpAccessor,
                _rp,
                A.Fake<IAIKmsDetailsRp>(),
                A.Fake<IFileRp>(),
                A.Fake<kevin.FileStorage.IFileStorage>(),
                _rag,
                A.Fake<kevin.Domain.Interfaces.IServices.IFileService>(),
                A.Fake<IOllamaApiService>(),
                A.Fake<IAIModelsRp>(),
                _distLock)
            {
                CurrentUser = _user,
                HttpContextAccessor = httpAccessor,
                SnowflakeIdService = new global::Kevin.SnowflakeId.Service.SnowflakeIdService()
            };
        }

        private static TAIKmss K(long id, int tenantId = MyTenantId, bool isDelete = false)
            => new() { Id = id, TenantId = tenantId, IsDelete = isDelete, Name = "测试知识库" };

        private static DocumentChunkDto Chunk(long id, int chunkIndex, string content = "内容")
            => new()
            {
                Id = id,
                ChunkIndex = chunkIndex,
                Content = content,
                SourceFile = SourceFile,
                Title = "contract",
                Category = "测试知识库",
                CreatedAt = new DateTimeOffset(2026, 11, 20, 15, 30, 0, TimeSpan.FromHours(8))
            };

        private void StubRag(ulong total, List<DocumentChunkDto> chunks)
            => A.CallTo(() => _rag.GetChunksBySourceFile(A<string>._, A<string>._, A<ulong>._, A<ulong>._))
                .Returns(Task.FromResult((total, chunks)));

        private static Exception GetException(Func<Task> action)
        {
            try
            {
                action().GetAwaiter().GetResult();
            }
            catch (Exception e) when (e is not InvalidOperationException)
            {
                return e;
            }
            return null!;
        }

        // ── 归属校验 ────────────────────────────────────────────────

        [Fact]
        public void GetKmsChunks_NotExists_ThrowsUserFriendlyException()
        {
            _data.Add(K(KmssId)); // 查询 kmsId 不在数据里
            var ex = GetException(() => _service.GetKmsChunks(KmssId + 1, SourceFile));
            Assert.IsType<UserFriendlyException>(ex);
            Assert.Equal("不存在或已删除", ex.Message);
            A.CallTo(() => _rag.GetChunksBySourceFile(A<string>._, A<string>._, A<ulong>._, A<ulong>._)).MustNotHaveHappened();
        }

        [Fact]
        public void GetKmsChunks_Deleted_ThrowsUserFriendlyException()
        {
            _data.Add(K(KmssId, isDelete: true));
            var ex = GetException(() => _service.GetKmsChunks(KmssId, SourceFile));
            Assert.IsType<UserFriendlyException>(ex);
            Assert.Equal("不存在或已删除", ex.Message);
        }

        [Fact]
        public void GetKmsChunks_OtherTenant_ThrowsUserFriendlyException()
        {
            _data.Add(K(KmssId, tenantId: MyTenantId + 1));
            var ex = GetException(() => _service.GetKmsChunks(KmssId, SourceFile));
            Assert.IsType<UserFriendlyException>(ex);
            Assert.Equal("不存在或已删除", ex.Message);
        }

        // ── 正常路径与 DTO 映射 ─────────────────────────────────────

        [Fact]
        public async Task GetKmsChunks_Normal_MapsDtoAndSortsByChunkIndex()
        {
            _data.Add(K(KmssId));
            // RAG 层已做内存排序兜底，但服务层再排一次：这里故意乱序喂入，验证服务侧仍稳定升序
            var chunks = new List<DocumentChunkDto>
            {
                Chunk(3, 2, "第三块"),
                Chunk(1, 0, "第一块"),
                Chunk(2, 1, "第二块")
            };
            StubRag(3, chunks);

            var result = await _service.GetKmsChunks(KmssId, SourceFile, 1, 10);

            Assert.Equal(3, result.Total);
            Assert.Equal(3, result.List.Count);
            Assert.Equal(new[] { 0, 1, 2 }, result.List.Select(c => c.ChunkIndex));
            Assert.Equal(new[] { 1L, 2L, 3L }, result.List.Select(c => c.Id));
            Assert.Equal(new[] { "第一块", "第二块", "第三块" }, result.List.Select(c => c.Content));
            Assert.All(result.List, c => Assert.Equal("测试知识库", c.Category));
            Assert.All(result.List, c => Assert.Equal(new DateTimeOffset(2026, 11, 20, 15, 30, 0, TimeSpan.FromHours(8)), c.CreatedAt));
        }

        [Fact]
        public async Task GetKmsChunks_Normal_PassesCollectionNameAndSourceFile()
        {
            _data.Add(K(KmssId));
            StubRag(0, new List<DocumentChunkDto>());

            await _service.GetKmsChunks(KmssId, SourceFile);

            A.CallTo(() => _rag.GetChunksBySourceFile(CollectionName, SourceFile, 10UL, 0UL))
                .MustHaveHappenedOnceExactly();
        }

        // ── 分页参数钳制与下传（CR-002/CR-003 服务侧口径）────────────

        [Theory]
        [InlineData(0, 1)]      // pageSize<1 → 钳为 1
        [InlineData(-5, 1)]     // 负数 → 钳为 1
        [InlineData(10, 10)]    // 正常值透传
        [InlineData(100, 100)]  // 上边界
        [InlineData(1000, 100)] // 超上限 → 钳为 100
        public async Task GetKmsChunks_PageSize_ClampedTo1To100(int inputPageSize, ulong expectedLimit)
        {
            _data.Add(K(KmssId));
            StubRag(0, new List<DocumentChunkDto>());

            await _service.GetKmsChunks(KmssId, SourceFile, 1, inputPageSize);

            A.CallTo(() => _rag.GetChunksBySourceFile(CollectionName, SourceFile, expectedLimit, 0UL))
                .MustHaveHappenedOnceExactly(); // 第 1 页 offset 恒为 0
        }

        [Theory]
        [InlineData(-3, 0)]            // pageNum<1 → 归 1，offset=0
        [InlineData(0, 0)]             // pageNum=0 → 归 1，offset=0
        [InlineData(1, 0)]             // 第 1 页 offset=0
        [InlineData(3, 40)]            // offset=(3-1)*20
        [InlineData(11, 200)]          // offset=(11-1)*20
        public async Task GetKmsChunks_PageNum_ClampedAndOffsetComputed(int inputPageNum, ulong expectedOffset)
        {
            _data.Add(K(KmssId));
            StubRag(0, new List<DocumentChunkDto>());

            await _service.GetKmsChunks(KmssId, SourceFile, inputPageNum, 20);

            A.CallTo(() => _rag.GetChunksBySourceFile(CollectionName, SourceFile, 20UL, expectedOffset))
                .MustHaveHappenedOnceExactly();
        }

        // ── pageNum 极大：服务层不溢出，RAG 异常后归一空（CR-003 服务侧链路）──

        [Fact]
        public async Task GetKmsChunks_HugePageNum_RagFailureNormalizedToEmpty()
        {
            _data.Add(K(KmssId));
            // 对巨大 offset 的调用让 RAG 桩替抛异常（模拟 Qdrant Server 拒绝超量 Scroll）
            A.CallTo(() => _rag.GetChunksBySourceFile(A<string>._, A<string>._, A<ulong>._, A<ulong>.That.IsGreaterThan(uint.MaxValue)))
                .Throws(new Exception("模拟 Qdrant Server 不接受超大预取 limit"));

            var result = await _service.GetKmsChunks(KmssId, SourceFile, int.MaxValue, 10);

            // 服务层 offset 计算 (ulong)((int.MaxValue-1)*10) 无 OverflowException；RAG 抛异常后归一空
            Assert.Equal(0, result.Total);
            Assert.Empty(result.List);
        }

        // ── 异常归一（CR-005 CreatedAt 脏数据在 RAG 层抛异常 → 服务侧整页归零）──

        [Fact]
        public async Task GetKmsChunks_RagThrows_ReturnsEmptyDtoNotThrow()
        {
            _data.Add(K(KmssId));
            A.CallTo(() => _rag.GetChunksBySourceFile(A<string>._, A<string>._, A<ulong>._, A<ulong>._))
                .Throws(new FormatException("模拟 RAG 层 CreatedAt 解析失败/连接失败"));

            var result = await _service.GetKmsChunks(KmssId, SourceFile);

            Assert.Equal(0, result.Total);
            Assert.NotNull(result.List);
            Assert.Empty(result.List);
        }

        // ── 空结果 ─────────────────────────────────────────────────

        [Fact]
        public async Task GetKmsChunks_NoChunks_ReturnsTotalOnly()
        {
            _data.Add(K(KmssId));
            StubRag(0, new List<DocumentChunkDto>());

            var result = await _service.GetKmsChunks(KmssId, SourceFile);

            Assert.Equal(0, result.Total);
            Assert.Empty(result.List);
        }

        [Fact]
        public async Task GetKmsChunks_RagReturnsEmptyListWithTotal_ReturnsTotalAndEmptyList()
        {
            _data.Add(K(KmssId));
            StubRag(7, new List<DocumentChunkDto>()); // 集合存在但该文件无块

            var result = await _service.GetKmsChunks(KmssId, SourceFile);

            Assert.Equal(7, result.Total);
            Assert.Empty(result.List);
        }

        // ── 只读不加锁 ─────────────────────────────────────────────

        [Fact]
        public async Task GetKmsChunks_ReadOnly_DoesNotAcquireDistributedLock()
        {
            _data.Add(K(KmssId));
            StubRag(0, new List<DocumentChunkDto>());

            await _service.GetKmsChunks(KmssId, SourceFile, 2, 10);

            A.CallTo(_distLock).MustNotHaveHappened();
        }
    }
}
