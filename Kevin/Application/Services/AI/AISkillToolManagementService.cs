using kevin.Domain.Entities.AI;
using kevin.Domain.Interfaces.IRepositories.AI;
using kevin.Domain.Interfaces.IServices.AI;
using kevin.Domain.Share.Dtos.AI;
using kevin.Domain.Share.Enums;
using kevin.FileStorage;
using Kevin.Common;

namespace kevin.Application.Services.AI
{
    /// <summary>
    /// AISkillToolManagement服务接口
    /// </summary>
    public class AISkillToolManagementService : BaseService, IAISkillToolManagementService
    {
        public readonly IAISkillToolManagementRp AISkillToolManagementRp;
        public readonly IFileRp _FileRp;

        public readonly IFileStorage _FileStorage;
        public AISkillToolManagementService(IHttpContextAccessor _httpContextAccessor, IAISkillToolManagementRp _AISkillToolManagementRp, IFileRp _IFileRp, IFileStorage _IFileStorage) : base(_httpContextAccessor)
        {
            this.AISkillToolManagementRp = _AISkillToolManagementRp;
            this._FileRp = _IFileRp;
            this._FileStorage = _IFileStorage;
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
            var dbdata = await data.Skip(skip).Take(dtoPagePar.pageSize).OrderByDescending(x => x.CreateTime).Include(t => t.CreateUser).Include(t => t.UpdateUser).ToListAsync();
            result.data = dbdata.MapToList<TAISkillToolManagement, AISkillToolManagementDto>();
            var flieData = _FileRp.Query().Where(t => t.IsDelete == false && t.Table == "AISkillToolManagement" && t.Sign == "SkillZip" && result.data.Select(a => a.Id.ToString()).ToList().Contains(t.TableId)).ToList().MapToList<TFile, FileDto>().ToList();
            foreach (var item in result.data)
            {
                item.SkillFile = flieData.Where(t => t.TableId == item.Id.ToString()).FirstOrDefault();
                item.CreateUser = dbdata.FirstOrDefault(d => d.Id == item.Id)?.CreateUser?.Name;
                item.UpdateUser = dbdata.FirstOrDefault(d => d.Id == item.Id)?.UpdateUser?.Name;
            }
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
            if (data.SkillToolType == AISkillToolTypeEnums.Skill)
            {

                //处理skill技能附件包
                var flieData = _FileRp.Query().Where(t => t.IsDelete == false && t.Table == "AISkillToolManagement" && t.Sign == "SkillZip" && data.Id.ToString() == t.TableId).FirstOrDefault();
                if (flieData != default && !string.IsNullOrEmpty(flieData.Url))
                {
                    //拼接路径
                    var path = Path.Combine(AppContext.BaseDirectory, "Skills", data.Name, data.Name);

                    //如果目录存在则删除目录下的所有文件  
                    if (Directory.Exists(path))
                    {
                        Directory.Delete(path, true);
                    }
                    Directory.CreateDirectory(path);
                    _FileStorage.FileDownload(flieData.Url, path + flieData.Name);
                    //将zip文件流解压到写入磁盘
                    using (var fileStream = File.OpenRead(path + flieData.Name))
                    {
                        FileZipHelper.ExtractZipStreamToDirectory(fileStream, path);
                    }
                    File.Delete(path + flieData.Name);
                }
                else
                {
                    throw new UserFriendlyException("请上传skill技能附件包");
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

        public async Task<List<AISkillToolManagementDto>> GetAllSkills()
        {
            return (await AISkillToolManagementRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.SkillToolType == AISkillToolTypeEnums.Skill && t.ActiveStatus == InActiveStatusEnums.Active).ToListAsync()).MapToList<TAISkillToolManagement, AISkillToolManagementDto>();

        }

        public async Task<List<AISkillToolManagementDto>> GetAllTools()
        {
            return (await AISkillToolManagementRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.SkillToolType == AISkillToolTypeEnums.Tool && t.ActiveStatus == InActiveStatusEnums.Active).ToListAsync()).MapToList<TAISkillToolManagement, AISkillToolManagementDto>();

        }
    }
}
