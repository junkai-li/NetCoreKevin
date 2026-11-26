using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Kevin.Unit.Infrastructure
{
    /// <summary>
    /// 测试支撑（仅测试工程使用，非生产代码）：让内存 IQueryable 支持 EF Core 的异步算子
    /// （CountAsync / ToListAsync / FirstOrDefaultAsync）——同步求值内存集合，结果包装为 Task。
    /// 用法：仓储 Query() 桩替返回 new TestAsyncEnumerable&lt;T&gt;(内存集合)。
    /// </summary>
    internal sealed class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
            => new TestAsyncEnumerable<TEntity>(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new TestAsyncEnumerable<TElement>(expression);

        public object Execute(Expression expression)
            => _inner.Execute(expression);

        public TResult Execute<TResult>(Expression expression)
            => (TResult)_inner.Execute(expression)!;

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var result = Execute(expression);
            if (typeof(TResult) == typeof(Task))
            {
                return (TResult)(object)Task.CompletedTask;
            }
            var elementType = typeof(TResult).GetGenericArguments()[0];
            var fromResult = typeof(Task).GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(elementType);
            return (TResult)fromResult.Invoke(null, new[] { result })!;
        }
    }

    internal sealed class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public T Current => _inner.Current;

        public ValueTask<bool> MoveNextAsync()
            => new(_inner.MoveNext());

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }
    }

    internal sealed class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IOrderedQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }

        public TestAsyncEnumerable(Expression expression) : base(expression) { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }
}
