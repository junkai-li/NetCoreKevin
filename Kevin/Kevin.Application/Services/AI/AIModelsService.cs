using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.AI;
using kevin.Share.Dtos;
using Web.Global.Exceptions;

namespace kevin.Application.Services.AI
{
    public class AIModelsService : BaseService, IAIModelsService
    {
        public IAIModelsRp aIModelsRp { get; set; }
        public AIModelsService(IHttpContextAccessor _httpContextAccessor, IAIModelsRp _aIModelsRp) : base(_httpContextAccessor)
        {
            this.aIModelsRp = _aIModelsRp;
        }

        /// <summary>
        /// 获取ai模型列表
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public async Task<dtoPageData<AIModelsDto>> GetPageData(dtoPagePar<string> dtoPage)
        {
            var result = new dtoPageData<AIModelsDto>();
            int skip = dtoPage.GetSkip();
            var data = aIModelsRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId);
            if (!string.IsNullOrEmpty(dtoPage.searchKey))
            {
                data = data.Where(t => (t.ModelName ?? "").Contains(dtoPage.searchKey));
            }
            result.total = await data.CountAsync();
            result.data = (await data.Skip(skip).Take(dtoPage.pageSize).OrderByDescending(x => x.CreateTime).ToListAsync()).MapToList<TAIModels, AIModelsDto>();
            return result;
        }
        /// <summary>
        /// 获取ai模型
        /// </summary>
        /// <param name="id"></param> 
        /// <returns></returns> 
        public async Task<AIModelsDto> GetDetails(long id)
        {
            var data = (await aIModelsRp.Query().FirstOrDefaultAsync(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId && t.Id == id)).MapTo<AIModelsDto>();
            if (data == default)
            {
                throw new UserFriendlyException("ai模型数据不存在或已删除");
            }
            return data;
        }
        /// <summary>
        /// 获取ai模型列表
        /// </summary>
        /// <param name="dtoPage"></param> 
        /// <returns></returns> 
        public async Task<List<AIModelsDto>> GetALLList()
        {
            var result = new List<AIModelsDto>();
            var data = aIModelsRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId);
            result = (await data.OrderByDescending(x => x.CreateTime).ToListAsync()).MapToList<TAIModels, AIModelsDto>();
            return result;
        }
        /// <summary>
        /// 编辑或添加ai模型
        /// </summary>
        /// <param name="par"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> AddEdit(AIModelsDto par)
        {
            var isAdd = par.Id == default;
            if (!isAdd)
            {
                var msg = aIModelsRp.Query().Where(t => t.IsDelete == false && t.Id == par.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = par.MapTo<TAIModels>();
                add.Id = par.Id == default ? SnowflakeIdService.GetNextId() : par.Id;
                add.IsDelete = false;
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                aIModelsRp.Add(add);
            }
            else
            {
                var msg = aIModelsRp.Query().Where(t => t.IsDelete == false && t.Id == par.Id).FirstOrDefault();
                if (msg != default)
                { 
                    msg.UpdateTime = DateTime.Now;
                    msg.UpdateUserId = CurrentUser.UserId;
                    msg.TenantId = CurrentUser.TenantId; 
                    msg.AIType= par.AIType;
                    msg.AIModelType = par.AIModelType;
                    msg.EndPoint = par.EndPoint;
                    msg.ModelName = par.ModelName;
                    msg.ModelKey = par.ModelKey;
                    msg.ModelDescription = par.ModelDescription; 

                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }

            }
            await aIModelsRp.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// 删除ai模型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        public async Task<bool> Delete(long id)
        {
            var like = await aIModelsRp.Query().Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();

            if (like != null)
            {
                like.IsDelete = true;
                like.DeleteTime = DateTime.Now;
                aIModelsRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }
    }
}
