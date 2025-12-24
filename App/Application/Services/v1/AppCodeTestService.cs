namespace App.Application.Services.v1
{
    /// <summary>
    /// AppCodeTest服务接口
    /// </summary>
    public class AppCodeTestService : BaseService, IAppCodeTestService
    {
        public IAppCodeTestRp AppCodeTestRp { get; set; }
        public AppCodeTestService(IHttpContextAccessor _httpContextAccessor, IAppCodeTestRp _AppCodeTestRp) : base(_httpContextAccessor)
        {
            this.AppCodeTestRp = _AppCodeTestRp;
        }

          public async Task<dtoPageData<TAppCodeTest>> GetPageData(dtoPagePar<string> dtoPagePar)
        {
            int skip = dtoPagePar.GetSkip();
            var result = new dtoPageData<TAppCodeTest>();
            var data = AppCodeTestRp.Query(isDataPer: true).Where(t => t.IsDelete == false);
            result.total = await data.CountAsync();
            result.data = await data.Skip(skip).Take(dtoPagePar.pageSize).OrderByDescending(x => x.CreateTime).ToListAsync();
            result.pageSize = dtoPagePar.pageSize;
            result.pageNum = dtoPagePar.pageNum;
            return result;
        }

        public async Task<bool> AddEdit(TAppCodeTest data)
        {
            var isAdd = data.Id == default;
            if (!isAdd)
            {
                var msg = AppCodeTestRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = data.MapTo<TAppCodeTest>();
                add.Id = data.Id == default ? SnowflakeIdService.GetNextId() : data.Id;
                add.IsDelete = false; 
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                AppCodeTestRp.Add(add);
            }
            else
            {
                var upData = AppCodeTestRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
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
            await AppCodeTestRp.SaveChangesAsync(); 
            return true;
        }

        public async Task<bool> Delete(long id)
        {
            var data = await AppCodeTestRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync(); 
            if (data != default)
            {
                
                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;
                AppCodeTestRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }
    }
}
