using System;
using System.Threading.Tasks;

namespace EFSession.Session
{
    public interface IDbExecutionPolicy
    {
        TResult Execute<TResult>(Func<TResult> func);
        void Execute(Action action);

        Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> func);
        Task ExecuteAsync(Func<Task> func);
    }
}