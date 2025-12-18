using kevin.Domain.Share.Interfaces;
using System.Linq.Expressions;
using Web.Global.User;

namespace kevin.Domain.Interface
{
    /// <summary>
    /// Repository Interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TId"></typeparam>
    public partial interface IRepository<T, TId> : IBaseRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isTenant"></param>
        /// <param name="isDataPer">默认为false，如果为true，则只查询当前用户的数据权限 指定才需要 不然有些地方没指定会造成循环调用死循环bug</param>
        /// <returns></returns>
        IQueryable<T> Query(bool isTenant = true, bool isDataPer = false);
        void Add(T entity);

        void AddRange(IEnumerable<T> entity);

        int SaveChanges();

        int SaveChangesWithSaveLog();
        T FirstOrDefault(Expression<Func<T, bool>> predicate, bool isTenant = true, bool isDataPer = false);
        bool Any(Expression<Func<T, bool>> predicate, bool isTenant = true, bool isDataPer = false);
        Task SaveChangesAsync();

        void Remove(T entity);

        void Update(T entity);
        void UpdateRange(params T[] entities);
        void RemoveRange(params T[] entity);
    }
}
