using kevin.Domain.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Repository.Database;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Web.Global.User;

namespace Ax.DataAccess
{
    /// <summary>
    /// 仓储类与类型Id
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public class Repository<T, TId> : IRepository<T, TId> where T : class
    {
        public Repository(dbContext context, IServiceProvider serviceProvider)
        {

            try
            {
                Context = context;
                DbSet = Context.Set<T>();
                ServiceProvider = serviceProvider;
                CurrentUser = serviceProvider.GetService<ICurrentUser>();

            }
            catch (Exception ex)
            {
                Type t = typeof(T);
                var name = t.Name;
            }
        }

        protected dbContext Context { get; }

        protected DbSet<T> DbSet { get; }

        protected IServiceProvider ServiceProvider;

        protected ICurrentUser CurrentUser;

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

        public Task SaveChangesAsync()
        {
            return Context.SaveChangesAsync();
        }

        public IQueryable<T> Query()
        {
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
