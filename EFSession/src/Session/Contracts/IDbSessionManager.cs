using System;
using EFSession.Schema.Parameters.Contracts;

namespace EFSession.Session
{
    public interface IDbSessionManager : IDisposable
    {
        bool HasAliveSessions { get; }

        IDbSeedSession<IDbSession> Start(string schema, SessionHint sessionHint = SessionHint.None);

        IDbSeedSession<IDbSession> Start(ISchemaCriteria schema, SessionHint sessionHint = SessionHint.None);
    }
}