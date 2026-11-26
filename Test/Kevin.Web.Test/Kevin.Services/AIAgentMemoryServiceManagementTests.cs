using FakeItEasy;
using kevin.AI.AgentFramework.Dto;
using kevin.AI.AgentFramework.Interfaces;
using kevin.Application;
using kevin.Application.Services.AI;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.AI;
using Kevin.SnowflakeId.Service;
using Kevin.Unit.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Web.Global.Exceptions;
using Web.Global.User;

namespace Kevin.Unit.Kevin.Services
{
    /// <summary>
    /// AIAgentMemoryService 管理端分支单元测试（TASK-04）：
    /// 覆盖 CODE-REVIEW 终审要求的风险面 —— ① 管理端 CurrentUser 归属口径
    /// （GetPageData/GetCountGroupByAIApps/AddEdit/Delete 全路径，UserId&lt;=0 抛异常防静默放大）；
    /// ② AddEdit 写入侧防护（新增归属无条件强制、编辑三维归属无条件保留原值、查不到禁止降级新增）；
    /// ③ 重要程度三档边界（4/5、7/8 归属档位）；④ 越权与不存在同提示防枚举；⑤ 超长校验。
    /// 仓储与共享信息服务全部用 FakeItEasy 桩替 + 内存 IQueryable（TestAsyncEnumerable），不触库、不触 Qdrant。
    /// </summary>
    public class AIAgentMemoryServiceManagementTests
    {
        private const long MyUserId = 10001;
        private const int MyTenantId = 1000;

        private readonly IAIAgentMemoryRp _rp;
        private readonly IAIShareInfoService _shareInfo;
        private readonly ICurrentUser _user;
        private readonly List<TAIAgentMemory> _data;
        private readonly AIAgentMemoryService _service;

        public AIAgentMemoryServiceManagementTests()
        {
            _rp = A.Fake<IAIAgentMemoryRp>();
            _shareInfo = A.Fake<IAIShareInfoService>();
            _user = A.Fake<ICurrentUser>();
            A.CallTo(() => _user.UserId).Returns(MyUserId);
            A.CallTo(() => _user.TenantId).Returns(MyTenantId);
            _data = new List<TAIAgentMemory>();
            StubQuery(_data);
            // 管理端纯 HTTP 上下文：AIShareInfoService 未初始化（空 DTO，UserId/AIChatsId/AIAppsId 均为 0）
            StubShareInfo(new AIShareInfoDto());

            var httpAccessor = A.Fake<IHttpContextAccessor>();
            A.CallTo(() => httpAccessor.HttpContext).Returns((HttpContext?)null);
            _service = new AIAgentMemoryService(
                httpAccessor,
                _rp,
                _shareInfo,
                A.Fake<IServiceScopeFactory>(),
                A.Fake<IServiceProvider>())
            {
                CurrentUser = _user,
                HttpContextAccessor = httpAccessor,
                SnowflakeIdService = new SnowflakeIdService()
            };
        }

        private void StubQuery(List<TAIAgentMemory> data)
            => A.CallTo(() => _rp.Query(A<bool>._, A<bool>._))
                .ReturnsLazily((bool _, bool _) => new TestAsyncEnumerable<TAIAgentMemory>(data));

        private void StubShareInfo(AIShareInfoDto dto)
            => A.CallTo(() => _shareInfo.GetData()).Returns(dto);

        private TAIAgentMemory? _added;

        private void CaptureAdded()
            => A.CallTo(() => _rp.Add(A<TAIAgentMemory>._))
                .Invokes((TAIAgentMemory m) => _added = m);

        private static TAIAgentMemory M(
            long id,
            long userId = MyUserId,
            int tenantId = MyTenantId,
            long aiAppsId = 1,
            int importance = 5,
            string content = "记忆内容",
            string keywords = "关键词",
            bool isDelete = false,
            DateTime? expireTime = null,
            int hitCount = 0,
            DateTime? lastHitTime = null,
            DateTime? createTime = null)
            => new()
            {
                Id = id,
                UserId = userId,
                TenantId = tenantId,
                AIAppsId = aiAppsId,
                Importance = importance,
                Content = content,
                Keywords = keywords,
                IsDelete = isDelete,
                ExpireTime = expireTime,
                HitCount = hitCount,
                LastHitTime = lastHitTime,
                CreateTime = createTime ?? DateTime.Now.AddMinutes(-10),
                MemoryType = "decision"
            };

        private static dtoAIAgentMemoryQuery Q(long aiAppsId = 0, int? min = null, int? max = null, string searchKey = "")
            => new() { AIAppsId = aiAppsId, ImportanceMin = min, ImportanceMax = max, searchKey = searchKey, pageNum = 1, pageSize = 20 };

        private static AIAgentMemoryDto D(long id = 0, long aiAppsId = 0, string? content = "新增内容", string? keywords = null, string? memoryType = null, int importance = 5, DateTime? expireTime = null)
            => new() { Id = id, AIAppsId = aiAppsId, Content = content, Keywords = keywords, MemoryType = memoryType, Importance = importance, ExpireTime = expireTime };

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
            throw new InvalidOperationException("预期抛出异常，但未抛出");
        }

        private static UserFriendlyException GetUFE(Func<Task> action)
        {
            try
            {
                action().GetAwaiter().GetResult();
            }
            catch (UserFriendlyException e)
            {
                return e;
            }
            catch (AggregateException e) when (e.InnerException is UserFriendlyException ufe)
            {
                return ufe;
            }
            throw new InvalidOperationException("预期抛出 UserFriendlyException，但未抛出");
        }

        #region GetPageData：管理端 CurrentUser 口径 + 三档筛选 + 边界

        [Fact]
        public async Task GetPageData_ManagementQuery_ShouldFilterByCurrentUserOnly()
        {
            // Arrange：本人 2 条 + 其他用户 1 条 + 软删 1 条 + 已过期 1 条
            _data.Add(M(1));
            _data.Add(M(2));
            _data.Add(M(3, userId: 99999));
            _data.Add(M(4, isDelete: true));
            _data.Add(M(5, expireTime: DateTime.Now.AddHours(-1)));

            // Act
            var result = await _service.GetPageData(Q());

            // Assert：仅本人未删除未过期记忆可见（CR-005：UserId 过滤强制叠加）
            Assert.Equal(2, result.total);
            Assert.All(result.data, t => Assert.Equal(MyUserId, t.UserId));
        }

        [Fact]
        public async Task GetPageData_ManagementQuery_ShouldFilterByAIAppsId()
        {
            _data.Add(M(1, aiAppsId: 11));
            _data.Add(M(2, aiAppsId: 22));
            _data.Add(M(3, aiAppsId: 11));

            var result = await _service.GetPageData(Q(aiAppsId: 11));

            Assert.Equal(2, result.total);
            Assert.All(result.data, t => Assert.Equal(11, t.AIAppsId));
        }

        [Fact]
        public async Task GetPageData_ImportanceBoundaries_LowTier_ShouldInclude4Exclude5()
        {
            // 边界值 4 归"低"档（0-4），5 归"中"档（5-7）
            _data.Add(M(1, importance: 3));
            _data.Add(M(2, importance: 4));
            _data.Add(M(3, importance: 5));

            var low = await _service.GetPageData(Q(min: 0, max: 4));

            Assert.Equal(2, low.total);
            Assert.All(low.data, t => Assert.True(t.Importance <= 4));
        }

        [Fact]
        public async Task GetPageData_ImportanceBoundaries_MidTier_ShouldInclude5And7Exclude4And8()
        {
            _data.Add(M(1, importance: 4));
            _data.Add(M(2, importance: 5));
            _data.Add(M(3, importance: 7));
            _data.Add(M(4, importance: 8));

            var mid = await _service.GetPageData(Q(min: 5, max: 7));

            Assert.Equal(2, mid.total);
            Assert.All(mid.data, t => Assert.True(t.Importance is >= 5 and <= 7));
        }

        [Fact]
        public async Task GetPageData_ImportanceBoundaries_HighTier_ShouldInclude8Exclude7()
        {
            _data.Add(M(1, importance: 7));
            _data.Add(M(2, importance: 8));
            _data.Add(M(3, importance: 9));

            var high = await _service.GetPageData(Q(min: 8, max: 10));

            Assert.Equal(2, high.total);
            Assert.All(high.data, t => Assert.True(t.Importance >= 8));
        }

        [Fact]
        public async Task GetPageData_ManagementQuery_SearchKey_ShouldMatchKeywordsOrContent()
        {
            _data.Add(M(1, content: "docker 部署偏好"));
            _data.Add(M(2, keywords: "docker,compose"));
            _data.Add(M(3, content: "无关内容", keywords: "其他"));

            var result = await _service.GetPageData(Q(searchKey: "docker"));

            Assert.Equal(2, result.total);
        }

        [Fact]
        public async Task GetPageData_ManagementQuery_WhenCurrentUserNull_ShouldThrow()
        {
            // CR-005：取不到用户禁止静默放大为租户全量
            _service.CurrentUser = null;
            _data.Add(M(1));

            var ex = GetUFE(() => _service.GetPageData(Q()));
            Assert.Equal("无法获取当前登录用户，请重新登录后再管理记忆", ex.Message);
        }

        [Fact]
        public async Task GetPageData_ManagementQuery_WhenUserIdZero_ShouldThrow()
        {
            var noUser = A.Fake<ICurrentUser>();
            A.CallTo(() => noUser.UserId).Returns(0);
            _service.CurrentUser = noUser;
            _data.Add(M(1));

            var ex = GetUFE(() => _service.GetPageData(Q()));
            Assert.Equal("无法获取当前登录用户，请重新登录后再管理记忆", ex.Message);
        }

        [Fact]
        public async Task GetPageData_PlainDtoPagePar_ShouldKeepLegacyBehaviorWithoutUserFilter()
        {
            // 回归：接口签名不变，普通 dtoPagePar<string> 不进 pattern-match 分支（不加用户过滤、不抛登录异常）
            _data.Add(M(1, userId: MyUserId));
            _data.Add(M(2, userId: 99999));

            var plain = new dtoPagePar<string> { pageNum = 1, pageSize = 20 };
            var result = await _service.GetPageData(plain);

            Assert.Equal(2, result.total);
            Assert.Contains(result.data, t => t.UserId == 99999);
        }

        #endregion

        #region GetCountGroupByAIApps：徽标计数口径

        [Fact]
        public async Task GetCountGroupByAIApps_ShouldGroupCurrentUserNonDeletedNonExpired()
        {
            _data.Add(M(1, aiAppsId: 11));
            _data.Add(M(2, aiAppsId: 11));
            _data.Add(M(3, aiAppsId: 22));
            _data.Add(M(4, aiAppsId: 0));
            _data.Add(M(5, userId: 99999, aiAppsId: 11));   // 其他用户
            _data.Add(M(6, isDelete: true, aiAppsId: 11));  // 软删
            _data.Add(M(7, expireTime: DateTime.Now.AddHours(-1), aiAppsId: 11)); // 过期

            var counts = await _service.GetCountGroupByAIApps();

            Assert.Contains(counts, c => c.AIAppsId == 11 && c.Count == 2);
            Assert.Contains(counts, c => c.AIAppsId == 22 && c.Count == 1);
            Assert.Contains(counts, c => c.AIAppsId == 0 && c.Count == 1);
            Assert.Equal(3, counts.Count);
        }

        [Fact]
        public async Task GetCountGroupByAIApps_WhenCurrentUserNull_ShouldThrow()
        {
            _service.CurrentUser = null;
            _data.Add(M(1));

            var ex = GetUFE(() => _service.GetCountGroupByAIApps());
            Assert.Equal("无法获取当前登录用户，请重新登录后再管理记忆", ex.Message);
        }

        #endregion

        #region AddEdit 新增：归属强制取 CurrentUser（CR-007 + 架构纠偏口径）

        [Fact]
        public async Task AddEdit_Add_ShouldForceOwnershipFromCurrentUser_IgnoringPayloadPositiveValues()
        {
            // payload 传正值 UserId/TenantId 伪造归属 → 必须被忽略，强制取登录上下文
            CaptureAdded();
            var payload = M(0, userId: 99999, tenantId: 8888, aiAppsId: 77, content: "新增记忆");

            var ok = await _service.AddEdit(payload);

            Assert.True(ok);
            Assert.NotNull(_added);
            Assert.Equal(MyUserId, _added!.UserId);
            Assert.Equal(MyTenantId, _added.TenantId);
        }

        [Fact]
        public async Task AddEdit_Add_ShouldUseExplicitAIAppsId_WhenGreaterThanZero()
        {
            CaptureAdded();
            var payload = M(0, aiAppsId: 777);

            await _service.AddEdit(payload);

            Assert.Equal(777, _added!.AIAppsId);
        }

        [Fact]
        public async Task AddEdit_Add_ShouldFallbackAIChatsIdAndAIAppsIdFromShareInfo_WhenPayloadNotSpecified()
        {
            // 语义实测（需向用户反馈）：新增分支 AIChatsId/AIAppsId 兜底取 AIShareInfoService；
            // shareInfo 残留上次 AI 调用值时取残留值（AIChatsId=123/AIAppsId=456），归属仍以 CurrentUser 为权威
            CaptureAdded();
            StubShareInfo(new AIShareInfoDto { AIChatsId = 123, AIAppsId = 456, UserId = 88888 });
            var payload = M(0, aiAppsId: 0);

            await _service.AddEdit(payload);

            Assert.Equal(123, _added!.AIChatsId);
            Assert.Equal(456, _added.AIAppsId);
            Assert.Equal(MyUserId, _added.UserId);
        }

        [Fact]
        public async Task AddEdit_Add_WithEmptyShareInfo_ShouldFallbackToZero()
        {
            // 管理端典型场景：shareInfo 从未 InitData → 兜底值为 0（归属"全局记忆"节点）
            CaptureAdded();
            StubShareInfo(new AIShareInfoDto());
            var payload = M(0, aiAppsId: 0);

            await _service.AddEdit(payload);

            Assert.Equal(0, _added!.AIChatsId);
            Assert.Equal(0, _added.AIAppsId);
        }

        [Fact]
        public async Task AddEdit_Add_ShouldAssignSnowflakeIdAndCreateDefaults()
        {
            CaptureAdded();
            var payload = M(0, content: "新增记忆");

            await _service.AddEdit(payload);

            Assert.True(_added!.Id > 0);
            Assert.Equal(default, payload.Id);
            Assert.False(_added.IsDelete);
            Assert.True((DateTime.Now - _added.CreateTime).TotalSeconds < 60);
        }

        [Fact]
        public async Task AddEdit_WhenCurrentUserNull_ShouldThrow()
        {
            _service.CurrentUser = null;
            var payload = M(0);

            var ex = GetUFE(() => _service.AddEdit(payload));
            Assert.Equal("无法获取当前登录用户，请重新登录后再操作记忆", ex.Message);
            A.CallTo(() => _rp.Add(A<TAIAgentMemory>._)).MustNotHaveHappened();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task AddEdit_ContentTooLong_ShouldThrow(bool isAdd)
        {
            var payload = isAdd ? M(0, content: new string('长', 4001)) : M(1, content: new string('长', 4001));
            if (!isAdd) _data.Add(M(1));

            var ex = GetUFE(() => _service.AddEdit(payload));
            Assert.StartsWith("记忆内容过长", ex.Message);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task AddEdit_KeywordsTooLong_ShouldThrow(bool isAdd)
        {
            var payload = isAdd ? M(0, keywords: new string('k', 201)) : M(1, keywords: new string('k', 201));
            if (!isAdd) _data.Add(M(1));

            var ex = GetUFE(() => _service.AddEdit(payload));
            Assert.StartsWith("记忆关键词过长", ex.Message);
        }

        #endregion

        #region AddEdit 编辑：三维归属无条件保留原值（传入值被忽略）

        [Fact]
        public async Task AddEdit_Edit_ShouldPreserveOwnershipTriple_AndHitStats_IgnoringPayloadValues()
        {
            // Arrange：本人记忆；payload 故意传错归属三维 + 异常 hitCount
            var lastHit = DateTime.Now.AddHours(-2);
            var original = M(100, aiAppsId: 555, hitCount: 3, lastHitTime: lastHit);
            _data.Add(original);
            var payload = M(100, userId: 99999, tenantId: 8888, aiAppsId: 888, hitCount: 99, lastHitTime: null, content: "编辑后的内容");

            var ok = await _service.AddEdit(payload);

            // Assert：UserId/TenantId/AIAppsId 无条件保留原值；HitCount/LastHitTime 保留防清零
            Assert.True(ok);
            Assert.Same(original, _data[0]);
            Assert.Equal("编辑后的内容", original.Content);
            Assert.Equal(MyUserId, original.UserId);
            Assert.Equal(MyTenantId, original.TenantId);
            Assert.Equal(555, original.AIAppsId);
            Assert.Equal(3, original.HitCount);
            Assert.Equal(lastHit, original.LastHitTime);
            Assert.NotNull(original.UpdateTime);
        }

        [Fact]
        public async Task AddEdit_Edit_OtherUsersMemory_ShouldThrowSameHintAndNoAddFallback()
        {
            // Id 存在但归属其他用户 → 归属校验查不到（越权与不存在同提示防枚举），禁止降级新增
            _data.Add(M(200, userId: 99999));
            var payload = M(200, content: "篡改尝试");

            var ex = GetUFE(() => _service.AddEdit(payload));
            Assert.Equal("数据不存在或已删除", ex.Message);
            A.CallTo(() => _rp.Add(A<TAIAgentMemory>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task AddEdit_Edit_MissingId_ShouldThrow()
        {
            _data.Add(M(1));
            var payload = M(99999, content: "不存在的 Id");

            var ex = GetUFE(() => _service.AddEdit(payload));
            Assert.Equal("数据不存在或已删除", ex.Message);
            A.CallTo(() => _rp.Add(A<TAIAgentMemory>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task AddEdit_Edit_DeletedMemory_ShouldThrow()
        {
            _data.Add(M(300, isDelete: true));
            var payload = M(300, content: "已删除记忆的编辑");

            var ex = GetUFE(() => _service.AddEdit(payload));
            Assert.Equal("数据不存在或已删除", ex.Message);
        }

        #endregion

        #region Delete：软删 + 归属校验防枚举

        [Fact]
        public async Task Delete_OwnMemory_ShouldSoftDelete()
        {
            var original = M(400);
            _data.Add(original);

            var ok = await _service.Delete(400);

            Assert.True(ok);
            Assert.True(original.IsDelete);
            Assert.NotNull(original.DeleteTime);
            A.CallTo(() => _rp.SaveChangesWithSaveLog()).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Delete_OtherUsersMemory_ShouldThrowSameHint()
        {
            // 越权 Id 与不存在 Id 返回同一提示，防雪花 Id 枚举探测
            _data.Add(M(500, userId: 99999));

            var ex = GetUFE(() => _service.Delete(500));
            Assert.Equal("数据不存在或已删除", ex.Message);
            A.CallTo(() => _rp.SaveChangesWithSaveLog()).MustNotHaveHappened();
        }

        [Fact]
        public async Task Delete_MissingId_ShouldThrowSameHint()
        {
            _data.Add(M(1));

            var ex = GetUFE(() => _service.Delete(99999));
            Assert.Equal("数据不存在或已删除", ex.Message);
        }

        [Fact]
        public async Task Delete_AlreadyDeletedMemory_ShouldThrow()
        {
            _data.Add(M(600, isDelete: true));

            var ex = GetUFE(() => _service.Delete(600));
            Assert.Equal("数据不存在或已删除", ex.Message);
        }

        [Fact]
        public async Task Delete_WhenCurrentUserNull_ShouldThrow()
        {
            _service.CurrentUser = null;
            _data.Add(M(1));

            var ex = GetUFE(() => _service.Delete(1));
            Assert.Equal("无法获取当前登录用户，请重新登录后再操作记忆", ex.Message);
        }

        #endregion

        #region AddEdit(AIAgentMemoryDto)：DTO 白名单口径（Controller 实际入口，防 mass assignment）

        [Fact]
        public async Task AddEditDto_Add_ShouldBuildEntityFromWhitelist_ForcingCurrentUserOwnership()
        {
            // DTO 白名单不含 UserId/TenantId/AIChatsId/HitCount：payload 物理上不可能伪造归属
            CaptureAdded();
            var payload = D(aiAppsId: 777, content: "  DTO 新增内容  ", keywords: " k1 ", memoryType: "DECISION", importance: 9);

            var ok = await _service.AddEdit(payload);

            Assert.True(ok);
            Assert.NotNull(_added);
            Assert.True(_added!.Id > 0);
            Assert.Equal("DTO 新增内容", _added.Content);           // Trim
            Assert.Equal("k1", _added.Keywords);                     // Trim + 空默认
            Assert.Equal("decision", _added.MemoryType);             // Normalize 大小写归一
            Assert.Equal(9, _added.Importance);
            Assert.Equal(777, _added.AIAppsId);                      // 入参 >0 生效
            Assert.Equal(0, _added.AIChatsId);                       // DTO 口径固定 0（不取 AIShareInfoService）
            Assert.Equal(MyUserId, _added.UserId);                   // 归属强制 CurrentUser
            Assert.Equal(MyTenantId, _added.TenantId);
            Assert.False(_added.IsDelete);
            Assert.True((DateTime.Now - _added.CreateTime).TotalSeconds < 60);
        }

        [Fact]
        public async Task AddEditDto_Add_WithAIAppsIdZero_ShouldSetZero()
        {
            // 管理端无默认智能体上下文：未传 AIAppsId 置 0（归属全局记忆节点，不再依赖 shareInfo 兜底）
            CaptureAdded();
            var payload = D(aiAppsId: 0);

            await _service.AddEdit(payload);

            Assert.Equal(0, _added!.AIAppsId);
            Assert.Equal(0, _added.AIChatsId);
            A.CallTo(() => _shareInfo.GetData()).MustNotHaveHappened(); // DTO 口径不再触碰 shareInfo
        }

        [Fact]
        public async Task AddEditDto_Add_WithNullOrBlankMemoryType_ShouldNormalizeToOther()
        {
            CaptureAdded();
            var payload = D(memoryType: null);

            await _service.AddEdit(payload);

            Assert.Equal("other", _added!.MemoryType);
        }

        [Fact]
        public async Task AddEditDto_Add_WithEmptyContent_ShouldThrowValidation()
        {
            var ex = GetException(() => _service.AddEdit(D(content: "  ")));
            Assert.Contains("内容不能为空", ex.Message);
            A.CallTo(() => _rp.Add(A<TAIAgentMemory>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task AddEditDto_WithContentTooLong_ShouldThrowValidation()
        {
            var ex = GetException(() => _service.AddEdit(D(content: new string('长', 4001))));
            Assert.Contains("4000", ex.Message);
        }

        [Fact]
        public async Task AddEditDto_WithKeywordsTooLong_ShouldThrowValidation()
        {
            var ex = GetException(() => _service.AddEdit(D(keywords: new string('k', 201))));
            Assert.Contains("200", ex.Message);
        }

        [Fact]
        public async Task AddEditDto_WithImportanceOutOfRange_ShouldThrowValidation()
        {
            var ex = GetException(() => _service.AddEdit(D(importance: 11)));
            Assert.Contains("0-10", ex.Message);

            var ex2 = GetException(() => _service.AddEdit(D(importance: -1)));
            Assert.Contains("0-10", ex2.Message);
        }

        [Fact]
        public async Task AddEditDto_WithInvalidMemoryType_ShouldThrowValidation()
        {
            // Check() 只校验传入值合法性；Normalize 回退在服务层（Add 分支）
            var ex = GetException(() => _service.AddEdit(D(memoryType: "not-a-type", content: "新增内容")));
            Assert.Contains("记忆类型", ex.Message);
        }

        [Fact]
        public async Task AddEditDto_Edit_ShouldUpdateWhitelistFields_PreserveOwnershipAndStats()
        {
            // 纠偏口径：前端只传 Content；其余字段传空/0/null 均保留原值
            var lastHit = DateTime.Now.AddHours(-2);
            var original = M(100, aiAppsId: 555, hitCount: 3, lastHitTime: lastHit, importance: 6, keywords: "原关键词", expireTime: DateTime.Now.AddDays(3));
            _data.Add(original);
            var payload = D(id: 100, content: "DTO 编辑内容", keywords: "", memoryType: "", importance: 0, expireTime: null);

            var ok = await _service.AddEdit(payload);

            Assert.True(ok);
            Assert.Same(original, _data[0]);
            Assert.Equal("DTO 编辑内容", original.Content);           // 白名单字段生效
            Assert.Equal("原关键词", original.Keywords);              // 空字符串保留原值
            Assert.Equal("decision", original.MemoryType);            // 空字符串保留原值
            Assert.Equal(6, original.Importance);                     // 0 保留原值（防清零）
            Assert.Equal(DateTime.Now.AddDays(3).Date, original.ExpireTime!.Value.Date); // null 保留原值
            Assert.Equal(555, original.AIAppsId);                     // 归属三维天然保留（DTO 无法携带）
            Assert.Equal(MyUserId, original.UserId);
            Assert.Equal(MyTenantId, original.TenantId);
            Assert.Equal(3, original.HitCount);
            Assert.Equal(lastHit, original.LastHitTime);
            Assert.NotNull(original.UpdateTime);
        }

        [Fact]
        public async Task AddEditDto_Edit_WithExplicitValues_ShouldApplyPositiveValues()
        {
            var original = M(100, importance: 6);
            _data.Add(original);
            var payload = D(id: 100, content: "新内容", keywords: "新关键词", memoryType: "pitfall", importance: 9);

            await _service.AddEdit(payload);

            Assert.Equal("新内容", original.Content);
            Assert.Equal("新关键词", original.Keywords);
            Assert.Equal("pitfall", original.MemoryType);
            Assert.Equal(9, original.Importance);
        }

        [Fact]
        public async Task AddEditDto_Edit_OtherUsersMemory_ShouldThrowSameHint()
        {
            _data.Add(M(200, userId: 99999));
            var payload = D(id: 200, content: "篡改尝试");

            var ex = GetUFE(() => _service.AddEdit(payload));
            Assert.Equal("数据不存在或已删除", ex.Message);
            A.CallTo(() => _rp.Add(A<TAIAgentMemory>._)).MustNotHaveHappened();
        }

        [Fact]
        public async Task AddEditDto_Edit_MissingId_ShouldThrow()
        {
            _data.Add(M(1));
            var payload = D(id: 99999, content: "不存在的 Id");

            var ex = GetUFE(() => _service.AddEdit(payload));
            Assert.Equal("数据不存在或已删除", ex.Message);
        }

        [Fact]
        public async Task AddEditDto_WhenCurrentUserNull_ShouldThrow()
        {
            _service.CurrentUser = null;

            var ex = GetUFE(() => _service.AddEdit(D()));
            Assert.Equal("无法获取当前登录用户，请重新登录后再操作记忆", ex.Message);
            A.CallTo(() => _rp.Add(A<TAIAgentMemory>._)).MustNotHaveHappened();
        }

        #endregion
    }
}
