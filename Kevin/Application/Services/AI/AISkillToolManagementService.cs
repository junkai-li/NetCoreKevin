using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// AISkillToolManagement服务接口
    /// </summary>
    public class AISkillToolManagementService : BaseService, IAISkillToolManagementService
    {
        public IAISkillToolManagementRp AISkillToolManagementRp { get; set; }
        public AISkillToolManagementService(IHttpContextAccessor _httpContextAccessor, IAISkillToolManagementRp _AISkillToolManagementRp) : base(_httpContextAccessor)
        {
            this.AISkillToolManagementRp = _AISkillToolManagementRp;
        }

        public async Task<dtoPageData<TAISkillToolManagement>> GetPageData(dtoPagePar<string> dtoPagePar)
        {
            int skip = dtoPagePar.GetSkip();
            var result = new dtoPageData<TAISkillToolManagement>();
            var data = AISkillToolManagementRp.Query(isDataPer: true).Where(t => t.IsDelete == false);
            result.total = await data.CountAsync();
            result.data = await data.Skip(skip).Take(dtoPagePar.pageSize).OrderByDescending(x => x.CreateTime).ToListAsync();
            result.pageSize = dtoPagePar.pageSize;
            result.pageNum = dtoPagePar.pageNum;
            return result;
        }

        public async Task<bool> AddEdit(TAISkillToolManagement data)
        {
            var isAdd = data.Id == default;
            if (!isAdd)
            {
                var msg = AISkillToolManagementRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = data.MapTo<TAISkillToolManagement>();
                add.Id = data.Id == default ? SnowflakeIdService.GetNextId() : data.Id;
                add.IsDelete = false; 
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                AISkillToolManagementRp.Add(add);
            }
            else
            {
                var upData = AISkillToolManagementRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (upData != default)
                {
                    upData= data.MapTo(upData);
                    upData.UpdateTime = DateTime.Now;
                    upData.UpdateUserId = CurrentUser.UserId;
                    upData.TenantId = CurrentUser.TenantId; 
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                } 
            }
            await AISkillToolManagementRp.SaveChangesAsync(); 
            return true;
        }

        public async Task<bool> Delete(long id)
        {
            var data = await AISkillToolManagementRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync(); 
            if (data != default)
            {
                
                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;
                AISkillToolManagementRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }
    }
}
