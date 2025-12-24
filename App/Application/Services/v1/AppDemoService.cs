using App.Domain.Entities;
using kevin.Domain.Share.Dtos;
using kevin.Share.Dtos;
using System;
using System.Threading.Tasks;
using Web.Global.Exceptions;

namespace App.Application.Services.v1
{
    /// <summary>
    /// AppDemo服务接口
    /// </summary>
    public class AppDemoService : BaseService, IAppDemoService
    {
        public IAppDemoRp AppDemoRp { get; set; }
        public AppDemoService(IHttpContextAccessor _httpContextAccessor, IAppDemoRp _AppDemoRp) : base(_httpContextAccessor)
        {
            this.AppDemoRp = _AppDemoRp;
        }

        public async Task<dtoPageData<TAppDemo>> GetPageData(dtoPagePar<string> dtoPagePar)
        {
            int skip = dtoPagePar.GetSkip();
            var result = new dtoPageData<TAppDemo>();
            var data = AppDemoRp.Query(isDataPer: true).Where(t => t.IsDelete == false);
            result.total = await data.CountAsync();
            result.data = await data.Skip(skip).Take(dtoPagePar.pageSize).OrderByDescending(x => x.CreateTime).ToListAsync();
            result.pageSize = dtoPagePar.pageSize;
            result.pageNum = dtoPagePar.pageNum;
            return result;
        }

        public async Task<bool> AddEdit(TAppDemo data)
        {
            var isAdd = data.Id == default;
            if (!isAdd)
            {
                var msg = AppDemoRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = data.MapTo<TAppDemo>();
                add.Id = data.Id == default ? SnowflakeIdService.GetNextId() : data.Id;
                add.IsDelete = false; 
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                AppDemoRp.Add(add);
            }
            else
            {
                var upData = AppDemoRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
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
            await AppDemoRp.SaveChangesAsync(); 
            return true;
        }

        public async Task<bool> Delete(long id)
        {
            var data = await AppDemoRp.Query(isDataPer: true).Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync(); 
            if (data != default)
            {
                
                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;
                AppDemoRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }
    }
}
