using kevin.Application;
using kevin.Domain.Entities.Organizational;
using kevin.Domain.Interfaces.IRepositories.Organizational;
using kevin.Domain.Share.Dtos;
using kevin.Domain.Share.Dtos.Organizational;
using kevin.Share.Dtos;
using Web.Global.Exceptions;

namespace kevin.Domain.Interfaces.IServices.Organizational
{ 
    public class DepartmentService : BaseService, IDepartmentService
    {
        public IDepartmentRp departmentRp { get; set; }
        public DepartmentService(IHttpContextAccessor _httpContextAccessor, IDepartmentRp _departmentRp) : base(_httpContextAccessor)
        {
            departmentRp= _departmentRp;
        }

        public async Task<dtoPageData<DepartmentDto>> GetPageData(dtoPagePar<string> par)
        {
            var dataPage = new dtoPageData<DepartmentDto>();
            int skip = par.GetSkip();
            var data = departmentRp.Query().Where(t => t.IsDelete == false && t.TenantId == CurrentUser.TenantId);
            if (!string.IsNullOrEmpty(par.searchKey))
            {
                data = data.Where(t => (t.Name ?? "").Contains(par.searchKey) || (t.Code ?? "").Contains(par.searchKey));
            }
            dataPage.total = await data.CountAsync();
            var dbdata = await data.Skip(skip).Take(par.pageSize).OrderByDescending(x => x.Sort).Include(t => t.CreateUser).Include(t => t.UpdateUser).ToListAsync();
            dataPage.data = dbdata.MapToList<TDepartment, DepartmentDto>();
            dataPage.data.ForEach(t => {
                t.CreateUser = dbdata.FirstOrDefault(d => d.Id == t.Id)?.CreateUser?.Name;
                t.UpdateUser = dbdata.FirstOrDefault(d => d.Id == t.Id)?.UpdateUser?.Name;
            });
            return dataPage;
        }

        public async Task<bool> AddEdit(DepartmentDto data)
        {
            var isAdd = data.Id == default;
            if (!isAdd)
            {
                var msg = departmentRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (msg == default)
                {
                    isAdd = true;
                }
            }
            if (isAdd)
            {
                var add = data.MapTo<TDepartment>();
                add.Id = data.Id == default ? SnowflakeIdService.GetNextId() : data.Id;
                add.IsDelete = false;
                add.CreateTime = DateTime.Now;
                add.CreateUserId = CurrentUser.UserId;
                add.TenantId = CurrentUser.TenantId;
                departmentRp.Add(add);
            }
            else
            {
                var department = departmentRp.Query().Where(t => t.IsDelete == false && t.Id == data.Id).FirstOrDefault();
                if (department != default)
                {
                    department.UpdateTime = DateTime.Now;
                    department.UpdateUserId = CurrentUser.UserId;
                    department.Name = data.Name;
                    department.Code = data.Code;
                    department.Description = data.Description;
                    department.ParentId = data.ParentId;
                    department.Status = data.Status;
                    department.Description=data.Description;
                    department.ManagerUserId = data.ManagerUserId;
                    department.Sort=data.Sort;
                }
                else
                {
                    throw new UserFriendlyException("数据不存在或已删除");
                }

            }
            await departmentRp.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Delete(long id)
        {
            var data = await departmentRp.Query().Where(t => t.IsDelete == false && t.Id == id).FirstOrDefaultAsync();

            if (data != null)
            {
                data.IsDelete = true;
                data.DeleteTime = DateTime.Now;
                departmentRp.SaveChangesWithSaveLog();
            }
            else
            {
                throw new UserFriendlyException("数据不存在或已删除");
            }
            return true;
        }
    }
}
