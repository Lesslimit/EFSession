using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EFSession.StoredProcedures
{
    public interface ISpResult<T>
    {
        Task<T> SingleAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<T> SingleAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken));

        Task<T> SingleOrDefaultAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<T> SingleOrDefaultAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken));

        Task<T> FirstAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<T> FirstAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken));

        Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<T> FirstOrDefaultAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<T>> ToListAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<Dictionary<TKey, T>> ToDictionaryAsync<TKey>(Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer = null,
                                                          CancellationToken cancellationToken = default(CancellationToken));

        Task<Dictionary<TKey, TResult>> ToDictionaryAsync<TKey, TResult>(Func<T, TKey> keySelector, Func<T, TResult> elementSelector, IEqualityComparer<TKey> comparer = null,
                                                                         CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> AnyAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> AnyAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> AllAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> ContainsAsync(T value, CancellationToken cancellationToken = default(CancellationToken));

        Task<int> CountAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<int> CountAsync(Func<T, bool> predicate, CancellationToken cancellationToken = default(CancellationToken));

        Task ForEachAsync(Action<T> action, CancellationToken cancellationToken = default(CancellationToken));
       
    }
}