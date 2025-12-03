using kevin.AI.AgentFramework;
using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.AI;
using kevin.Share.Dtos;
using Kevin.AI;
using Web.Global.Exceptions;

namespace kevin.Application.Services.AI
{
    public class AIChatsService : BaseService, IAIChatsService
    {
        public IAIChatsRp aIChatsRp { get; set; }

        public IAIAppsService aIAppsService { get; set; }
        public IAIChatHistorysRp aIChatHistorysRp { get; set; }
        public IAIAgentService aIAgentService { get; set; }
        /// public IAIClient aIClient { get; set; }
        public IAIModelsService aIModelsService { get; set; }

        public IAIPromptsService aIPromptsService { get; set; }

        public AIChatsService(IHttpContextAccessor _httpContextAccessor, IAIChatsRp _aIChatsRp,
            IAIAgentService _aIAgentService,
            IAIChatHistorysRp _aIChatHistorysRp, IAIAppsService _aIAppsService,
            IAIModelsService _aIModelsService, IAIPromptsService aIPromptsService
            // IAIClient _aIClient
            ) : base(_httpContextAccessor)
        {
            this.aIChatsRp = _aIChatsRp;
            this.aIAgentService = _aIAgentService;
            this.aIChatHistorysRp = _aIChatHistorysRp;
            this.aIAppsService = _aIAppsService;
            this.aIModelsService = _aIModelsService;
            this.aIPromptsService = aIPromptsService;
            // this.aIClient = _aIClient;
        }

        /// <summary>
        /// 获取我的ai聊天列表
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public async Task<dtoPageData<AIChatsDto>> GetMyPageData(dtoPagePar<string> dtoPage)
        {
            var result = new dtoPageData<AIChatsDto>();
            int skip = dtoPage.GetSkip();
            var data = aIChatsRp.Query().Where(t => t.IsDelete == false && t.UserId == CurrentUser.UserId && t.TenantId == CurrentUser.TenantId);
            if (!string.IsNullOrEmpty(dtoPage.searchKey))
            {
                data = data.Where(t => (t.Name ?? "").Contains(dtoPage.searchKey));
            }
            result.total = await data.CountAsync();
            result.data = (await data.Skip(skip).Take(dtoPage.pageSize).OrderByDescending(x => x.CreateTime).ToListAsync()).MapToList<TAIChats, AIChatsDto>();
            return result;
        }


        /// <summary>
        /// 新建聊天
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<AIChatHistorysDto> Add(AIChatsDto par)
        {
            var aiapp = await aIAppsService.GetDetails(par.AppId);
            var aIModels = await aIModelsService.GetDetails(aiapp.ChatModelID.ToTryInt64());
            var aIPrompts = await aIPromptsService.GetDetails(aiapp.AIPromptID);
            var add = par.MapTo<TAIChats>();
            add.Id = par.Id == default ? SnowflakeIdService.GetNextId() : par.Id;
            add.IsDelete = false;
            add.CreateTime = DateTime.Now;
            add.CreateUserId = CurrentUser.UserId;
            add.TenantId = CurrentUser.TenantId;
            add.UserId = CurrentUser.UserId;
            aIChatsRp.Add(add);
            await aIChatsRp.SaveChangesAsync();

            var addHist = new TAIChatHistorys();
            addHist.Id = SnowflakeIdService.GetNextId();
            addHist.IsDelete = false;
            addHist.CreateTime = DateTime.Now;
            addHist.CreateUserId = CurrentUser.UserId;
            addHist.TenantId = CurrentUser.TenantId;
            addHist.IsSend = false;
            addHist.AIChatsId = add.Id;
            switch (aIModels.AIType)
            {
                case Domain.Share.Enums.AIType.OpenAI:
                    addHist.Content = (await aIAgentService.CreateOpenAIAgentAndSendMSG("请开始你的自我介绍", aiapp.Name, aIPrompts.Prompt, aIPrompts.Description ?? "你是一个智能体,请根据你的提示词进行相关回答", aIModels.EndPoint, aIModels.ModelName, aIModels.ModelKey)).Item2.Text;
                    //addHist.Content = aIClient.SendMsg("请开始你的自我介绍", aIModels.EndPoint, aIModels.ModelKey, aIModels.ModelName, aIPrompts.Prompt + (aIPrompts.Description ?? "你是一个智能体,请根据你的提示词进行相关回答")).choices.FirstOrDefault().message.content;
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
            aIChatHistorysRp.Add(addHist);
            await aIChatHistorysRp.SaveChangesAsync();
            return addHist.MapTo<AIChatHistorysDto>();
        }

        /// <summary>
        /// 修改对话的主题和最后一条消息
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Name"></param>
        /// <param name="LastMessage"></param>
        /// <returns></returns>
        public async Task<bool> UpdateNameAndMsg(long Id, string Name = "", string LastMessage = "")
        {
            var ai = await aIChatsRp.Query().Where(t => t.IsDelete == false && t.Id == Id).FirstOrDefaultAsync();
            if (ai != null)
            {
                if (!string.IsNullOrEmpty(Name))
                {
                    ai.Name = Name;
                }
                if (!string.IsNullOrEmpty(LastMessage))
                {
                    ai.LastMessage = LastMessage;
                } 
                ai.UpdateTime = DateTime.Now;
                ai.UpdateUserId = CurrentUser.UserId;
                await aIChatsRp.SaveChangesAsync();
            }
            return true;
        }

        /// <summary>
        /// 获取ai聊天对话
        /// </summary>
        /// <param name="id"></param> 
        /// <returns></returns> 
        public async Task<AIChatsDto> GetDetails(long id)
        {
            var data = (await aIChatsRp.Query().FirstOrDefaultAsync(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId && t.Id == id)).MapTo<AIChatsDto>();
            if (data == default)
            {
                throw new UserFriendlyException("获取ai聊天对话不存在或已删除");
            }
            return data;
        }

        /// <summary>
        /// 删除ai聊天
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> Delete(long id)
        {
            var like = await aIChatsRp.Query().Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();

            if (like != null)
            {
                like.IsDelete = true;
                like.DeleteTime = DateTime.Now;
                aIChatsRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }
    }


}
