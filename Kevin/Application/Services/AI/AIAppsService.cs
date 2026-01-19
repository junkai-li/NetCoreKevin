using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.AI;
using kevin.Share.Dtos;
using System;
using Web.Global.Exceptions;

namespace kevin.Application.Services.AI
{
    public class AIAppsService : BaseService, IAIAppsService
    {
        public IAIAppsRp aIAppsRp { get; set; }
        public AIAppsService(IHttpContextAccessor _httpContextAccessor, IAIAppsRp _aIAppsRp) : base(_httpContextAccessor)
        {
            this.aIAppsRp = _aIAppsRp;
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
            result.data = (await data.Skip(skip).Take(dtoPage.pageSize).OrderByDescending(x => x.CreateTime).ToListAsync()).MapToList<TAIApps, AIAppsDto>();
            return result;
        }


        /// <summary>
        /// 获取ai应用
        /// </summary>
        /// <param name="id"></param> 
        /// <returns></returns> 
        public async Task<AIAppsDto> GetDetails(long id)
        {
            var data = (await aIAppsRp.Query(isDataPer: true).FirstOrDefaultAsync(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId && t.Id == id)).MapTo<AIAppsDto>();
            if (data == default)
            {
                throw new UserFriendlyException("ai应用数据不存在或已删除");
            }
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
        /// 编辑或添加ai应用
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> AddEdit(AIAppsDto par)
        {
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
                    msg.MaxAskPromptSize = par.MaxAskPromptSize;
                    msg.MaxMatchesCount = par.MaxMatchesCount;
                    msg.RerankCount = par.RerankCount;
                    msg.AnswerTokens = par.AnswerTokens;
                    msg.AIPromptID = par.AIPromptID;
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }

            }
            await aIAppsRp.SaveChangesAsync();

            return true;
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
