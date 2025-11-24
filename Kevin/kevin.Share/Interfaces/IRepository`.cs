using kevin.Domain.Share.Interfaces;
using System.Linq.Expressions;
using Web.Global.User;

namespace kevin.Domain.Interface
{
    public interface IRepository<T, TId>: IBaseRepository
    {
        IQueryable<T> Query(); 
        bool Any(Expression<Func<T, bool>> predicate);
        void Add(T entity);

        void AddRange(IEnumerable<T> entity);

        int SaveChanges();

        int SaveChangesWithSaveLog();
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        Task SaveChangesAsync();

        void Remove(T entity);

        void Update(T entity);
        void UpdateRange(params T[] entities);
        void RemoveRange(params T[] entity); 
    }
}
