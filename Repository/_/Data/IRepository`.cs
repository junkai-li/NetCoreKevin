using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Ax.DataAccess
{
    public interface IRepository<T, TId>
    {
        IQueryable<T> Query();

        bool Any(Expression<Func<T, bool>> predicate);
        void Add(T entity);

        void AddRange(IEnumerable<T> entity);

        IDbContextTransaction BeginTransaction();

        void SaveChanges();
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        Task SaveChangesAsync();

        void Remove(T entity);
         
        void Update(T entity);
        void UpdateRange(params T[] entities);
        void RemoveRange(params T[] entity);
    }
}
