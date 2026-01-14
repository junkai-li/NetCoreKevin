using kevin.Domain.Bases;
using kevin.Domain.Interface;
using Kevin.Common.App;
using Kevin.Common.Extension;
using Kevin.SnowflakeId.Service;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using NPOI.SS.Formula.Functions;
using Repository.Database;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Web.Global.User;

namespace Kevin.EntityFrameworkCore._.Data
{
    /// <summary>
    /// 仓储类与类型Id
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public partial class Repository<T, TId> : IRepository<T, TId> where T : class
    {
        public Repository(IServiceProvider serviceProvider)
        {

            try
            {
                Context = serviceProvider.GetService<KevinDbContext>();
                DbSet = Context.Set<T>();
                ServiceProvider = serviceProvider;
                CurrentUser = serviceProvider.GetService<ICurrentUser>();
                SnowflakeIdService = serviceProvider.GetService<ISnowflakeIdService>();

            }
            catch (Exception ex)
            {
                Type t = typeof(T);
                var name = t.Name;
                throw new Exception($"仓储类{t.Name}初始化失败！", ex);
            }
        }

        protected KevinDbContext Context { get; }

        protected DbSet<T> DbSet { get; }

        protected IServiceProvider ServiceProvider;

        protected ICurrentUser CurrentUser;

        protected ISnowflakeIdService SnowflakeIdService;

        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public void Update(T entity)
        {
            DbSet.Update(entity);
        }
        private Expression<Func<T, bool>> GetTenantDataPerExp(bool isTenant = true, bool isDataPer = true)
        {
            try
            {
                Expression<Func<T, bool>> t_d_p_predicate = t => 1 == 1;
                Type t = typeof(T);
                if (isTenant)
                {
                    var fieldInfo = t.GetProperty("TenantId");
                    if (fieldInfo != default)
                    {
                        if (CurrentUser != default && CurrentUser.TenantId > 0)
                        {
                            t_d_p_predicate = t_d_p_predicate.And(e => EF.Property<Int32>(e, "TenantId") == CurrentUser.TenantId);
                        }
                        else
                        {
                            t_d_p_predicate = t_d_p_predicate.And(e => EF.Property<Int32>(e, "TenantId") == TenantHelper.GetSettingsTenantId().ToTryInt32());
                        }
                    }

                }
                if (isDataPer)
                {
                    if (CurrentUser != default && CurrentUser.UserId > 0)
                    {
                        var fieldInfo_t_create_userId = t.GetProperty("CreateUserId");
                        if (fieldInfo_t_create_userId != default)
                        {
                            var userIds = CurrentUser.GetModuleDataPermissionsUserIds().Result;
                            if (userIds.Count > 0)
                            { 
                                t_d_p_predicate = t_d_p_predicate.And(e => userIds.Contains(EF.Property<long>(e, "CreateUserId")));
                            }
                        }
                    }
                }
                return t_d_p_predicate;
            }
            catch (Exception ex)
            {

                throw new Exception($"默认租户过滤和数据权限查询失败！", ex);
            }
        }
        public T FirstOrDefault(Expression<Func<T, bool>> predicate, bool isTenant = true, bool isDataPer = true)
        {
            var exp = GetTenantDataPerExp(isTenant, isDataPer);
            if (exp != default)
            {
                predicate.And(exp);
            }
            return DbSet.FirstOrDefault(predicate);
        }
        public bool Any(Expression<Func<T, bool>> predicate, bool isTenant = true, bool isDataPer = true)
        {
            var exp = GetTenantDataPerExp(isTenant, isDataPer);
            if (exp != default)
            {
                predicate.And(exp);
            }
            return DbSet.Any(predicate);
        }
        public void UpdateRange(params T[] entities)
        {
            DbSet.UpdateRange(entities);
        }

        public void AddRange(IEnumerable<T> entity)
        {
            DbSet.AddRange(entity);
        }

        public IDbContextTransaction BeginTransaction()
        {
            return Context.Database.BeginTransaction();
        }

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public int SaveChangesWithSaveLog()
        {
            return Context.SaveChangesWithSaveLog();
        }
        public Task SaveChangesAsync()
        {
            return Context.SaveChangesAsync(false);
        }

        /// <summary>
        /// 默认租户过滤的查询和数据权限过滤
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> Query(bool isTenant = true, bool DataPer = true)
        {
            var exp = GetTenantDataPerExp(isTenant, DataPer);
            if (exp != default)
            {
                return DbSet.Where(exp);
            }
            return DbSet;
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }
        public void RemoveRange(params T[] entity)
        {
            DbSet.RemoveRange(entity);
        }
    }
}
