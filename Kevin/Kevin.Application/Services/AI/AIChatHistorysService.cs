using kevin.AI.AgentFramework;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.AI;
using kevin.Share.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Global.Exceptions;

namespace kevin.Application.Services.AI
{

    public class AIChatHistorysService : BaseService, IAIChatHistorysService
    {
        public IAIChatHistorysRp aIChatHistorysRp { get; set; }
        public IAIAgentService aIAgentService { get; set; }
        public IAIModelsService aIModelsService { get; set; }

        public IAIPromptsService aIPromptsService { get; set; }
        public IAIChatsService aIChatsService { get; set; }
        public IAIAppsService aIAppsService { get; set; }
        public AIChatHistorysService(IHttpContextAccessor _httpContextAccessor, IAIChatHistorysRp _aIChatHistorysRp,
            IAIAgentService _aIAgentService, IAIModelsService _aIModelsService, IAIPromptsService _aIPromptsService,
            IAIChatsService _aIChatsService, IAIAppsService _aIAppsService) : base(_httpContextAccessor)
        {
            this.aIChatHistorysRp = _aIChatHistorysRp;
            this.aIChatsService = _aIChatsService;
            this.aIAgentService = _aIAgentService;
            this.aIModelsService = _aIModelsService;
            this.aIPromptsService = _aIPromptsService;
            this.aIAppsService = _aIAppsService;

        }

        /// <summary>
        /// 获取我的ai聊天列表
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public async Task<dtoPageData<AIChatHistorysDto>> GetPageData(dtoPagePar<string> dtoPage)
        {
            var result = new dtoPageData<AIChatHistorysDto>();
            int skip = dtoPage.GetSkip();
            var data = aIChatHistorysRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId);
            if (dtoPage.whereId > 0)
            {
                data = data.Where(t => t.AIChatsId == dtoPage.whereId);
            }
            else
            {
                throw new UserFriendlyException("必须传入聊天Id");
            }
            result.total = await data.CountAsync();
            result.data = (await data.OrderByDescending(x => x.CreateTime).Skip(skip).Take(dtoPage.pageSize).ToListAsync()).MapToList<TAIChatHistorys, AIChatHistorysDto>();
            return result;
        }


        /// <summary>
        /// 新建聊天
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<AIChatHistorysDto> Add(AIChatHistorysDto par)
        {
            var count = aIChatHistorysRp.Query().Where(t => t.IsDelete == false && t.AIChatsId == par.AIChatsId).Count();
            var aichas = await aIChatsService.GetDetails(par.AIChatsId);
            var aiapp = await aIAppsService.GetDetails(aichas.AppId);
            var aIModels = await aIModelsService.GetDetails(aiapp.ChatModelID.ToTryInt64());
            var aIPrompts = await aIPromptsService.GetDetails(aiapp.AIPromptID);
            var add = par.MapTo<TAIChatHistorys>();
            add.Id = par.Id == default ? SnowflakeIdService.GetNextId() : par.Id;
            add.IsDelete = false;
            add.CreateTime = DateTime.Now;
            add.CreateUserId = CurrentUser.UserId;
            add.TenantId = CurrentUser.TenantId;
            add.IsSend = true;
            aIChatHistorysRp.Add(add);
            await aIChatHistorysRp.SaveChangesAsync();
            //回复消息
            var addAi = new TAIChatHistorys();
            addAi.Id = SnowflakeIdService.GetNextId();
            addAi.IsDelete = false;
            addAi.CreateTime = DateTime.Now;
            addAi.CreateUserId = CurrentUser.UserId;
            addAi.TenantId = CurrentUser.TenantId;
            addAi.IsSend = false;
            addAi.AIChatsId = par.AIChatsId;
            switch (aIModels.AIType)
            {
                case Domain.Share.Enums.AIType.OpenAI:
                    addAi.Content = (await aIAgentService.CreateOpenAIAgentAndSendMSG(add.Content, aiapp.Name, aIPrompts.Prompt, aIPrompts.Description ?? "你是一个智能体,请根据你的提示词进行相关回答", aIModels.EndPoint, aIModels.ModelName, aIModels.ModelKey)).Item2.Text;
                    break;
                case Domain.Share.Enums.AIType.AzureOpenAI:
                    break;
                case Domain.Share.Enums.AIType.SparkDesk:
                    break;
                case Domain.Share.Enums.AIType.DashScope:
                    break;
                case Domain.Share.Enums.AIType.LLamaFactory:
                    break;
                case Domain.Share.Enums.AIType.BgeEmbedding:
                    break;
                case Domain.Share.Enums.AIType.BgeRerank:
                    break;
                case Domain.Share.Enums.AIType.Ollama:
                    break;
                case Domain.Share.Enums.AIType.OllamaEmbedding:
                    break;
                case Domain.Share.Enums.AIType.Mock:
                    break;
                default:
                    break;
            }
            aIChatHistorysRp.Add(addAi);
            await aIChatHistorysRp.SaveChangesAsync();
            await aIChatsService.UpdateNameAndMsg(par.AIChatsId, count == 1 ? par.Content : "", addAi.Content);
            return addAi.MapTo<AIChatHistorysDto>();
        }

        /// <summary>
        /// 删除聊天记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> Delete(long id)
        {
            var like = await aIChatHistorysRp.Query().Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();

            if (like != null)
            {
                like.IsDelete = true;
                like.DeleteTime = DateTime.Now;
                aIChatHistorysRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }
    }
}
