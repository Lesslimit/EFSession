using System;

namespace EFSession.Session
{
    [Flags]
    public enum SessionHint
    {
        None = 0,
        NoTracking = 1,
        SingleQuery = 1 << 1,
        DisposeOnSave = 1 << 2,
        AtomicOffspings = 1 << 3,
        HugeUpdate = 1 << 4,
        SystemDb = 1 << 5
    }
}