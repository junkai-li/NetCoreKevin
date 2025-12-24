namespace kevin.Application.Services
{
    /// <summary>
    /// WeiXinKey服务接口
    /// </summary>
    public class WeiXinKeyService : BaseService, IWeiXinKeyService
    {
        public IWeiXinKeyRp WeiXinKeyRp { get; set; }
        public WeiXinKeyService(IHttpContextAccessor _httpContextAccessor, IWeiXinKeyRp _WeiXinKeyRp) : base(_httpContextAccessor)
        {
            this.WeiXinKeyRp = _WeiXinKeyRp;
        }

          public async Task<dtoPageData<TWeiXinKey>> GetPageData(dtoPagePar<string> dtoPagePar)
        {
            int skip = dtoPagePar.GetSkip();
            var result = new dtoPageData<TWeiXinKey>();
            var data = WeiXinKeyRp.Query(isDataPer: true).Where(t => t.IsDelete == false);
            result.total = await data.CountAsync();
            result.data = await data.Skip(skip).Take(dtoPagePar.pageSize).OrderByDescending(x => x.CreateTime).ToListAsync();
            result.pageSize = dtoPagePar.pageSize;
            result.pageNum = dtoPagePar.pageNum;
            return result;
        }

        public async Task<bool> AddEdit(TWeiXinKey data)
        {
            var isAdd = data.Id == default;
            if (!isAdd)
            {
                var msg = WeiXinKeyRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = data.MapTo<TWeiXinKey>();
                add.Id = data.Id == default ? SnowflakeIdService.GetNextId() : data.Id;
                add.IsDelete = false; 
                add.CreateTime = DateTime.Now; 
                add.TenantId = CurrentUser.TenantId;
                WeiXinKeyRp.Add(add);
            }
            else
            {
                var upData = WeiXinKeyRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (upData != default)
                {
                    upData= data.MapTo(upData); 
                    upData.TenantId = CurrentUser.TenantId; 
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                } 
            }
            await WeiXinKeyRp.SaveChangesAsync(); 
            return true;
        }

        public async Task<bool> Delete(long id)
        {
            var data = await WeiXinKeyRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync(); 
            if (data != default)
            {
                
                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;
                WeiXinKeyRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }
    }
}
