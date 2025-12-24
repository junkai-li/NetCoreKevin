using App.Domain.Entities.AppInfoTest;
using App.Domain.Interfaces.Repositorie.v1.AppInfoTest;
using App.Domain.Interfaces.Services.v1.AppInfoTest;

namespace App.Application.Services.v1.AppInfoTest
{
    /// <summary>
    /// AppInfoTest服务接口
    /// </summary>
    public class AppInfoTestService : BaseService, IAppInfoTestService
    {
        public IAppInfoTestRp AppInfoTestRp { get; set; }
        public AppInfoTestService(IHttpContextAccessor _httpContextAccessor, IAppInfoTestRp _AppInfoTestRp) : base(_httpContextAccessor)
        {
            this.AppInfoTestRp = _AppInfoTestRp;
        }

        public async Task<dtoPageData<TAppInfoTest>> GetPageData(dtoPagePar<string> dtoPagePar)
        {
            int skip = dtoPagePar.GetSkip();
            var result = new dtoPageData<TAppInfoTest>();
            var data = AppInfoTestRp.Query(isDataPer: true).Where(t => t.IsDelete == false);
            result.total = await data.CountAsync();
            result.data = await data.Skip(skip).Take(dtoPagePar.pageSize).OrderByDescending(x => x.CreateTime).ToListAsync();
            result.pageSize = dtoPagePar.pageSize;
            result.pageNum = dtoPagePar.pageNum;
            return result;
        }

        public async Task<bool> AddEdit(TAppInfoTest data)
        {
            var isAdd = data.Id == default;
            if (!isAdd)
            {
                var msg = AppInfoTestRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = data.MapTo<TAppInfoTest>();
                add.Id = data.Id == default ? SnowflakeIdService.GetNextId() : data.Id;
                add.IsDelete = false;
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                AppInfoTestRp.Add(add);
            }
            else
            {
                var upData = AppInfoTestRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (upData != default)
                {
                    upData = data.MapTo(upData);
                    upData.UpdateTime = DateTime.Now;
                    upData.UpdateUserId = CurrentUser.UserId;
                    upData.TenantId = CurrentUser.TenantId;
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }
            }
            await AppInfoTestRp.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(long id)
        {
            var data = await AppInfoTestRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();
            if (data != default)
            {

                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;
                AppInfoTestRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }
    }
}
