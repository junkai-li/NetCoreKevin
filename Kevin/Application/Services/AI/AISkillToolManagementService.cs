using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using kevin.Domain.Share.Enums;

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

        public async Task<dtoPageData<AISkillToolManagementDto>> GetPageData(dtoPagePar<int> dtoPagePar)
        {
            int skip = dtoPagePar.GetSkip();
            var result = new dtoPageData<AISkillToolManagementDto>();
            var data = AISkillToolManagementRp.Query(isDataPer: true).Where(t => t.IsDelete == false);
            if (!string.IsNullOrEmpty(dtoPagePar.searchKey))
            {
                data = data.Where(t => (t.Name ?? "").Contains(dtoPagePar.searchKey));
            }
            if (dtoPagePar.Parameter != null && dtoPagePar.Parameter > 0)
            {
                data = data.Where(t => t.SkillToolType == (AISkillToolTypeEnums)dtoPagePar.Parameter);
            }
            result.total = await data.CountAsync();
            result.data = (await data.Skip(skip).Take(dtoPagePar.pageSize).OrderByDescending(x => x.CreateTime).ToListAsync()).MapTo<List<AISkillToolManagementDto>>();
            result.pageSize = dtoPagePar.pageSize;
            result.pageNum = dtoPagePar.pageNum;
            return result;
        }

        public async Task<bool> AddEdit(AISkillToolManagementDto data)
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
                add.IsSystem = false;
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
                    if (upData.IsSystem)
                    {
                        throw new UserFriendlyException("系统内置工具不允许修改");
                    } 
                    upData.Name = data.Name;
                    upData.SkillToolType = data.SkillToolType;
                    upData.Url = data.Url;
                    upData.ActiveStatus = data.ActiveStatus;
                    upData.ClassMethod = data.ClassMethod;
                    upData.Description = data.Description; 
                    upData.UpdateTime = DateTime.Now;
                    upData.UpdateUserId = CurrentUser.UserId;
                    upData.TenantId = CurrentUser.TenantId;
                    upData.IsDelete = false;
                    upData.IsSystem = false;
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
                if (data.IsSystem)
                {
                    throw new UserFriendlyException("系统内置工具不允许删除");
                }
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
