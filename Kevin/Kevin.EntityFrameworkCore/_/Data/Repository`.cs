using kevin.Domain.Bases;
using kevin.Domain.Interface;
using Kevin.Common.App;
using Kevin.SnowflakeId.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
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
    public class Repository<T, TId> : IRepository<T, TId> where T : class
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

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }
        public bool Any(Expression<Func<T, bool>> predicate)
        {
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
        /// 默认租户过滤的查询
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> Query(bool isTenantId = true)
        {
            try
            {
                if (!isTenantId)
                {
                    return DbSet;
                }
                if (CurrentUser != default && CurrentUser.TenantId > 0)
                {
                    return DbSet.Where(e => EF.Property<Int32>(e, "TenantId") == CurrentUser.TenantId);
                }
                Type t = typeof(T);
                FieldInfo fieldInfo = t.GetType().GetField("TenantId");
                if (fieldInfo != null)
                {
                    return DbSet.Where(e => EF.Property<string>(e, "TenantId") == TenantHelper.GetSettingsTenantId());
                }
                else
                {
                    return DbSet;
                }
            }
            catch (Exception)
            {

                return DbSet;
            }
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
