using System;
using System.Linq;
using System.Linq.Expressions;

namespace System.Linq
{
    /// <summary>
    /// IQueryableExtensions
    /// </summary>
    public static class QueryableExtension
    {
        /// <summary>
        /// 如果 value 不为 null 或者 空字符串，则进行 predicate 筛选
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IQueryable<TSource> WhereIfNotNullOrEmpty<TSource>(this IQueryable<TSource> source, string value, Expression<Func<TSource, bool>> predicate)
        {
            return source.WhereIf(!string.IsNullOrEmpty(value), predicate);
        }
        /// <summary>
        /// 如果 value 不为 null，则进行 predicate 筛选
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IQueryable<TSource> WhereIfNotNull<TSource, TValue>(this IQueryable<TSource> source, Nullable<TValue> value, Expression<Func<TSource, bool>> predicate) where TValue : struct
        {
            return source.WhereIf(value != null, predicate);
        }

        /// <summary>
        /// 如果 value 不为 null，则进行 predicate 筛选
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IQueryable<TSource> WhereIfNotNull<TSource>(this IQueryable<TSource> source, object value, Expression<Func<TSource, bool>> predicate)
        {
            return source.WhereIf(value != null, predicate);
        }

        /// <summary>
        /// 如果 condition 为 true，则进行 predicate 筛选
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="condition"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
            {
                return source.Where(predicate);
            }
            
            return source;
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="pageNumber">页数</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public static IQueryable<TSource> TakePage<TSource>(this IQueryable<TSource> source, int pageNumber, int pageSize)
        {
            int skipCount = (pageNumber - 1) * pageSize;
            int takeCount = pageSize;

            IQueryable<TSource> q = source.Skip(skipCount).Take(takeCount);
            return q;
        }
    }
}
