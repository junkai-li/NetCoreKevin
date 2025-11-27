using kevin.Application;
using kevin.Application.Services;
using kevin.Domain.Entities;
using kevin.Domain.Entities.Organizational;
using kevin.Domain.Interfaces.IRepositories.Organizational;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.Organizational;
using kevin.RepositorieRps.Repositories;
using kevin.Share.Dtos;
using Web.Global.Exceptions;

namespace kevin.Domain.Interfaces.IServices.Organizational
{
    public class PositionService : BaseService, IPositionService
    {
        public IPositionRp positionRp { get; set; }
        public PositionService(IHttpContextAccessor _httpContextAccessor, IPositionRp _positionRp) : base(_httpContextAccessor)
        {
            positionRp = _positionRp;
        }

        public async Task<dtoPageData<PositionDto>> GetPageData(dtoPagePar<string> par)
        {
            var dataPage = new dtoPageData<PositionDto>();
            int skip = par.GetSkip();
            var data = positionRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId);
            if (!string.IsNullOrEmpty(par.searchKey))
            {
                data = data.Where(t => (t.Name ?? "").Contains(par.searchKey) || (t.Code ?? "").Contains(par.searchKey));
            } 
            dataPage.total = await data.CountAsync();
            var dbdata = await data.Skip(skip).Take(par.pageSize).OrderByDescending(x => x.Sort).Include(t => t.CreateUser).Include(t => t.UpdateUser).ToListAsync();
            dataPage.data = dbdata.MapToList<TPosition, PositionDto>();
            dataPage.data.ForEach(t => {
                t.CreateUser = dbdata.FirstOrDefault(d => d.Id == t.Id)?.CreateUser?.Name;
                t.UpdateUser = dbdata.FirstOrDefault(d => d.Id == t.Id)?.UpdateUser?.Name;
            });
            return dataPage;
        } 

        public async Task<bool> AddEdit(PositionDto data)
        {
            var isAdd = data.Id == default;
            if (!isAdd)
            {
                var msg = positionRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = data.MapTo<TPosition>();
                add.Id = data.Id == default ? SnowflakeIdService.GetNextId() : data.Id;
                add.IsDelete = false;
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                positionRp.Add(add);
            }
            else
            {
                var position = positionRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (position != default)
                {
                    position.UpdateTime = DateTime.Now;
                    position.UpdateUserId = CurrentUser.UserId;
                    position.Name = data.Name;
                    position.Code = data.Code;
                    position.Description = data.Description;
                    position.ParentId = data.ParentId;
                    position.Status = data.Status;
                    position.Sort = data.Sort;
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }

            }
            await positionRp.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Delete(long id)
        {
            var data = await positionRp.Query().Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();

            if (data != null)
            {
                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;
                positionRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }
    }
}
