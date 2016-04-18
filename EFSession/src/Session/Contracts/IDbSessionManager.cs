using System;
using EFSession.Schema.Parameters.Contracts;

namespace EFSession.Session
{
    public interface IDbSessionManager : IDisposable
    {
        bool HasAliveSessions { get; }

        ISeedSession<IDbSession> Start(string schema, SessionHint sessionHint = SessionHint.None);

        ISeedSession<IDbSession> Start(ISchemaCriteria schema, SessionHint sessionHint = SessionHint.None);
    }
}