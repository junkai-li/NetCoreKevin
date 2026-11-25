using FakeItEasy;
using kevin.AI.AgentFramework.Interfaces;
using kevin.AI.AgentFramework.Interfaces.Msg;
using kevin.AI.AgentFramework.Interfaces.Tasks;
using kevin.AI.AgentFramework.Interfaces.Tools;
using kevin.Application;
using kevin.Application.Services.AI;
using kevin.Domain.Entities;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IServices;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using Kevin.SnowflakeId.Service;
using Microsoft.AspNetCore.Http;
using Web.Global.User;

namespace Kevin.Unit.Kevin.Services
{
    /// <summary>
    /// GetUserAIAgentSkillsAsync 追加挂载逻辑单元测试（TASK-04）：
    /// 覆盖 / 弹层动态追加 Skill 的合并挂载 —— 不传新参数时旧行为回归、默认+追加合并、
    /// excludeDefault 剔除默认绑定、无效/无权限 Id 静默跳过（Warn 不抛错）、结果去重。
    /// 依赖（绑定服务、技能管理服务）全部用 FakeItEasy 桩替，不触库。
    /// </summary>
    public class AIAgentToolSkillServiceGetUserSkillsTests
    {
        private readonly IAISkillToolBindIdService _bindIdService;
        private readonly IAISkillToolManagementService _skillManagementService;
        private readonly AIAgentToolSkillService _service;

        public AIAgentToolSkillServiceGetUserSkillsTests()
        {
            _bindIdService = A.Fake<IAISkillToolBindIdService>();
            _skillManagementService = A.Fake<IAISkillToolManagementService>();

            var httpAccessor = A.Fake<IHttpContextAccessor>();
            // 显式桩定为无 Http 上下文：让 BaseService 构造走非 Http 分支（避免隐式代理触发 GetService 转型异常）
            A.CallTo(() => httpAccessor.HttpContext).Returns((HttpContext?)null);
            // 构造服务：除被测依赖外，其余 15 个构造参数全部用 Fake 桩替（本测试不触及其路径）；
            // BaseService 的 required 成员在初始值设定项中显式补齐（非 Http 上下文）
            _service = new AIAgentToolSkillService(
                A.Fake<IKevinAITaskService>(),
                _bindIdService,
                _skillManagementService,
                A.Fake<ICommonToolsService>(),
                A.Fake<IPythonToolsService>(),
                A.Fake<IShellToolsService>(),
                A.Fake<IAgentHttpClientToolsService>(),
                A.Fake<IAgentFileParseToolsService>(),
                A.Fake<IUserService>(),
                A.Fake<IAIJsonLogService>(),
                A.Fake<IAIFileToolService>(),
                A.Fake<IAIMsgService>(),
                A.Fake<IAuthorizedToolsService>(),
                A.Fake<IWebSearchEngine>(),
                A.Fake<IAIAgentMemoryService>(),
                A.Fake<IAuthorizeService>(),
                A.Fake<IAIShareInfoService>(),
                httpAccessor)
            {
                HttpContextAccessor = httpAccessor,
                CurrentUser = new CurrentUser(httpAccessor, null),
                SnowflakeIdService = new SnowflakeIdService()
            };
        }

        /// <summary>默认绑定桩：agentId=1 绑定 Skill 10/11，名称 Alpha/Beta</summary>
        private void SetupDefaultBindings()
        {
            A.CallTo(() => _bindIdService.GetListById("1")).Returns(new List<TAISkillToolBindId>
            {
                new() { AISkillToolManagementId = 10 },
                new() { AISkillToolManagementId = 11 }
            });
            A.CallTo(() => _skillManagementService.GetNotDataPerAllSkills()).Returns(new List<AISkillToolManagementDto>
            {
                new() { Id = 10, Name = "Alpha" },
                new() { Id = 11, Name = "Beta" }
            });
        }

        private async Task<List<string>> CallAsync(List<long>? ids, bool excludeDefault = false)
        {
            return await _service.GetUserAIAgentSkillsAsync("1", "u1", ids, excludeDefault);
        }

        /// <summary>桩定按 Id 追加查询：给定的 ids 返回对应 Skill（Id*10 命名便于断言）</summary>
        private void SetupAdditional(params long[] foundIds)
        {
            A.CallTo(() => _skillManagementService.GetDataPerSkillsByIds(A<List<long>>._))
                .ReturnsLazily((List<long> ids) =>
                    ids.Where(foundIds.Contains)
                       .Select(id => new AISkillToolManagementDto { Id = id, Name = "Skill" + id })
                       .ToList());
        }

        #region 兼容性：不传新参数 → 仅默认绑定（旧行为回归）

        [Fact]
        public async Task GetUserSkills_WithoutAdditionalParams_ShouldReturnDefaultBindingsOnly()
        {
            // Arrange
            SetupDefaultBindings();

            // Act：双参旧签名调用（子智能体路径 :825 与能力清单等存量调用点的回归面）
            var result = await CallAsync(null);

            // Assert
            Assert.Equal(new List<string> { "Alpha", "Beta" }, result);
            A.CallTo(() => _skillManagementService.GetDataPerSkillsByIds(A<List<long>>._))
                .MustNotHaveHappened();
        }

        #endregion

        #region 合并挂载：默认 ∪ 追加

        [Fact]
        public async Task GetUserSkills_WithAdditionalIds_ShouldMergeDefaultAndAdditional()
        {
            // Arrange
            SetupDefaultBindings();
            SetupAdditional(20);

            // Act
            var result = await CallAsync(new List<long> { 20 });

            // Assert：默认 + 追加，默认绑定在前
            Assert.Equal(new List<string> { "Alpha", "Beta", "Skill20" }, result);
        }

        [Fact]
        public async Task GetUserSkills_ShouldDeduplicateNamesAcrossDefaultAndAdditional()
        {
            // Arrange：追加 Skill 与默认绑定同名（不同 Id）
            SetupDefaultBindings();
            A.CallTo(() => _skillManagementService.GetDataPerSkillsByIds(A<List<long>>._))
                .Returns(new List<AISkillToolManagementDto> { new() { Id = 21, Name = "Alpha" } });

            // Act
            var result = await CallAsync(new List<long> { 21 });

            // Assert：Distinct 后 Alpha 只出现一次
            Assert.Equal(new List<string> { "Alpha", "Beta" }, result);
        }

        #endregion

        #region excludeDefault：忽略默认绑定

        [Fact]
        public async Task GetUserSkills_WithExcludeDefaultTrue_ShouldReturnAdditionalOnly()
        {
            // Arrange
            SetupDefaultBindings();
            SetupAdditional(30);

            // Act
            var result = await CallAsync(new List<long> { 30 }, excludeDefault: true);

            // Assert：默认绑定被剔除，不触发默认绑定查询
            Assert.Equal(new List<string> { "Skill30" }, result);
            A.CallTo(() => _bindIdService.GetListById(A<string>._)).MustNotHaveHappened();
            A.CallTo(() => _skillManagementService.GetNotDataPerAllSkills()).MustNotHaveHappened();
        }

        [Fact]
        public async Task GetUserSkills_WithExcludeDefaultTrueAndNoAdditionalIds_ShouldReturnEmpty()
        {
            // Arrange：忽略默认 + 无追加 → 空列表（调用方该类工具不挂载，本轮回退纯对话）
            SetupDefaultBindings();

            // Act
            var result = await CallAsync(null, excludeDefault: true);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region 容错：追加 Id 查不到（已删/无权限/未启用）→ 静默跳过不抛错

        [Fact]
        public async Task GetUserSkills_WithMissingAdditionalIds_ShouldSkipSilently()
        {
            // Arrange：请求 3 个追加 Id，仅 40 存在（41/42 已删或无权限，不在 isDataPer 结果中）
            SetupDefaultBindings();
            SetupAdditional(40);

            // Act：不抛异常即通过（缺失 Id 走 Warn 日志跳过）
            var result = await CallAsync(new List<long> { 40, 41, 42 });

            // Assert：仅返回查到的 Skill
            Assert.Equal(new List<string> { "Alpha", "Beta", "Skill40" }, result);
        }

        [Fact]
        public async Task GetUserSkills_WithAllMissingAdditionalIds_ShouldReturnDefaultOnly()
        {
            // Arrange：全部追加 Id 查不到
            SetupDefaultBindings();
            SetupAdditional();

            // Act
            var result = await CallAsync(new List<long> { 50, 51 });

            // Assert：追加全部跳过，仅剩默认绑定
            Assert.Equal(new List<string> { "Alpha", "Beta" }, result);
        }

        #endregion
    }
}
