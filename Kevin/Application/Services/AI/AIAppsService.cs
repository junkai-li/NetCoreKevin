using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;

namespace kevin.Application.Services.AI
{
    public class AIAppsService : BaseService, IAIAppsService
    {
        public IAIAppsRp aIAppsRp { get; set; }

        public readonly IAISkillToolManagementService aISkillToolManagementService;

        public readonly IAISkillToolBindIdService aISkillToolBindIdService;

        public readonly IAIAppsBindIdService aIAppsBindIdService;
        public AIAppsService(IHttpContextAccessor _httpContextAccessor, IAIAppsRp _aIAppsRp,
            IAISkillToolManagementService aISkillToolManagementService, IAISkillToolBindIdService aISkillToolBindIdService, IAIAppsBindIdService aIAppsBindIdService) : base(_httpContextAccessor)
        {
            this.aIAppsRp = _aIAppsRp;
            this.aISkillToolManagementService = aISkillToolManagementService;
            this.aISkillToolBindIdService = aISkillToolBindIdService;
            this.aIAppsBindIdService = aIAppsBindIdService;
        }

        /// <summary>
        /// 获取ai应用列表
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public async Task<dtoPageData<AIAppsDto>> GetPageData(dtoPagePar<string> dtoPage)
        {
            var result = new dtoPageData<AIAppsDto>();
            int skip = dtoPage.GetSkip();
            var data = aIAppsRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId);
            if (!string.IsNullOrEmpty(dtoPage.searchKey))
            {
                data = data.Where(t => (t.Name ?? "").Contains(dtoPage.searchKey));
            }
            result.total = await data.CountAsync();
            var dbdata = await data.Skip(skip).Take(dtoPage.pageSize).OrderByDescending(x => x.CreateTime).Include(t => t.CreateUser).Include(t => t.UpdateUser).ToListAsync();
            result.data = dbdata.MapToList<TAIApps, AIAppsDto>();
            result.data.ForEach(t =>
            {
                t.CreateUser = dbdata.FirstOrDefault(d => d.Id == t.Id)?.CreateUser?.Name;
                t.UpdateUser = dbdata.FirstOrDefault(d => d.Id == t.Id)?.UpdateUser?.Name;
            });
            return result;
        }


        /// <summary>
        /// 获取ai应用
        /// </summary>
        /// <param name="id"></param> 
        /// <returns></returns> 
        public async Task<AIAppsDto> GetDetails(long id)
        {
            var data = (await aIAppsRp.Query(isDataPer: false).FirstOrDefaultAsync(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId && t.Id == id)).MapTo<AIAppsDto>();
            if (data == default)
            {
                throw new UserFriendlyException("ai应用数据不存在或已删除");
            }
            var skills = await aISkillToolManagementService.GetAllSkills();
            var tools = await aISkillToolManagementService.GetAllTools();
            var myIds = await aISkillToolBindIdService.GetListById(data.Id.ToString());
            data.Skills = skills.Select(t => new AIAppsBindSkillToolsDto
            {
                IsSelect = myIds.Any(x => x.AISkillToolManagementId == t.Id),
                AISkillToolManagementName = t.Name,
                AISkillToolManagementDescription = t.Description,
                AISkillToolManagementId = t.Id
            }).ToList();
            data.Tools = tools.Select(t => new AIAppsBindSkillToolsDto
            {
                IsSelect = myIds.Any(x => x.AISkillToolManagementId == t.Id),
                AISkillToolManagementName = t.Name,
                AISkillToolManagementDescription = t.Description,
                AISkillToolManagementId = t.Id
            }).ToList();
            data.BindIds = (await aIAppsBindIdService.GetListByBindId(data.Id.ToString())).Select(t => t.BindId).ToList();
            return data;
        }
        /// <summary>
        /// 获取ai应用列表
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public async Task<List<AIAppsDto>> GetALLList()
        {
            var result = new List<AIAppsDto>();
            var data = aIAppsRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId);
            result = (await data.OrderByDescending(x => x.CreateTime).ToListAsync()).MapToList<TAIApps, AIAppsDto>();
            return result;
        }

        /// <summary>
        /// 获取我可用的ai应用列表
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public async Task<List<AIAppsDto>> GetMyALLList()
        {
            if (!CurrentUser.IsSuperAdmin)
            {
                var result = new List<AIAppsDto>();
                List<string> bingIds = new List<string> {
            "user_"+CurrentUser.UserId.ToString()
            };
                if (CurrentUser.RoleIds?.Count > 0)
                {
                    bingIds.AddRange(CurrentUser.RoleIds.Select(t => "role_" + t.ToString()).ToList());
                }
                var appIds = (await aIAppsBindIdService.GetListById(bingIds)).Select(t => t.TAIAppsId).ToList();
                var data = aIAppsRp.Query(isDataPer: false).Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId && appIds.Contains(t.Id));
                result = (await data.OrderByDescending(x => x.CreateTime).ToListAsync()).MapToList<TAIApps, AIAppsDto>();
                return result;
            }
            else
            {
                return await GetALLList();
            }


        }

        /// <summary>
        /// 编辑或添加ai应用
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> AddEdit(AIAppsDto par)
        {
            par.Check();
            var isAdd = par.Id == default;
            if (!isAdd)
            {
                var msg = aIAppsRp.Query().Where(t => t.IsDelete == false && t.Id == par.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = par.MapTo<TAIApps>();
                add.Id = par.Id == default ? SnowflakeIdService.GetNextId() : par.Id;
                add.IsDelete = false;
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                aIAppsRp.Add(add);
            }
            else
            {
                var msg = aIAppsRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Id == par.Id).FirstOrDefault();
                if (msg != default)
                {
                    msg.UpdateTime = DateTime.Now;
                    msg.UpdateUserId = CurrentUser.UserId;
                    msg.TenantId = CurrentUser.TenantId;
                    msg.Name = par.Name;
                    msg.Describe = par.Describe;
                    msg.Icon = par.Icon;
                    msg.Type = par.Type;
                    msg.ChatModelID = par.ChatModelID;
                    msg.RerankModelID = par.RerankModelID;
                    msg.Temperature = par.Temperature;
                    msg.KmsId = par.KmsId;
                    msg.SecretKey = par.SecretKey;
                    msg.Relevance = par.Relevance;
                    msg.MsgType = par.MsgType;
                    msg.MaxAskPromptSize = par.MaxAskPromptSize;
                    msg.MaxMatchesCount = par.MaxMatchesCount;
                    msg.RerankCount = par.RerankCount;
                    msg.AnswerTokens = par.AnswerTokens;
                    msg.AIPromptID = par.AIPromptID;
                    msg.IsAITools = par.IsAITools;
                    msg.IsSkill = par.IsSkill;
                    msg.MaxAskPromptSize = par.MaxAskPromptSize;
                    msg.NetworkTimeout = par.NetworkTimeout;
                    msg.IsHttpLog = par.IsHttpLog;
                    msg.AuthorizedDomains = par.AuthorizedDomains;
                    msg.ChatMessageLimit = par.ChatMessageLimit;
                    msg.IsToolLog = par.IsToolLog;
                    msg.IsThinkingLog = par.IsThinkingLog;
                    msg.ContentLengthLimit = par.ContentLengthLimit;
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }

            }
            await aIAppsRp.SaveChangesAsync();
            var ids = par.Skills.Where(t => t.IsSelect).Select(t => t.AISkillToolManagementId).ToList();
            ids.AddRange(par.Tools.Where(t => t.IsSelect).Select(t => t.AISkillToolManagementId).ToList());
            await aISkillToolBindIdService.BatchAddIds(par.Id.ToString(), ids);
            await aIAppsBindIdService.BatchAddIds(par.Id.ToString(), par.BindIds);
            return true;
        }


        /// <summary>
        ///新增初始化
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<AIAppsDto> NewInitialization()
        {
            var data = new AIAppsDto();
            data.Id = SnowflakeIdService.GetNextId();
            data.CreateTime = DateTime.Now;
            data.CreateUserId = CurrentUser.UserId;
            var skills = await aISkillToolManagementService.GetAllSkills();
            var tools = await aISkillToolManagementService.GetAllTools();
            var myIds = await aISkillToolBindIdService.GetListById(data.Id.ToString());
            data.Skills = skills.Select(t => new AIAppsBindSkillToolsDto
            {
                IsSelect = myIds.Any(x => x.AISkillToolManagementId == t.Id),
                AISkillToolManagementName = t.Name,
                AISkillToolManagementDescription = t.Description,
                AISkillToolManagementId = t.Id
            }).ToList();
            data.Tools = tools.Select(t => new AIAppsBindSkillToolsDto
            {
                IsSelect = myIds.Any(x => x.AISkillToolManagementId == t.Id),
                AISkillToolManagementDescription = t.Description,
                AISkillToolManagementName = t.Name,
                AISkillToolManagementId = t.Id
            }).ToList();
            return data;
        }

        /// <summary>
        /// 删除ai应用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> Delete(long id)
        {
            var like = await aIAppsRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();

            if (like != null)
            {
                like.IsDelete = true;
                like.DeleteTime = DateTime.Now;
                aIAppsRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }
    }
}
