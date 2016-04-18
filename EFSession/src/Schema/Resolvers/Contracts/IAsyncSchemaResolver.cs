using System.Threading.Tasks;
using EFSession.Schema.Parameters.Contracts;

namespace EFSession.Schema.Resolvers.Contracts
{
    public interface IAsyncSchemaResolver
    {
        bool CanResolve(ISchemaCriteria schema);

        Task<string> ResolveAsync(ISchemaCriteria schema);
    }
}