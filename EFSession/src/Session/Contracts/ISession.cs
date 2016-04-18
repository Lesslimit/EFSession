using System;

namespace EFSession.Session
{
    public interface ISession
    {
        Guid Id { get; }

        bool IsActive { get; }
    }
}