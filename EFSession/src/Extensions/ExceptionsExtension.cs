using System;
using System.Threading;

namespace EFSession.Extensions
{
    public static class ExceptionsExtension
    {
        public static bool IsHandleable(this Exception ex)
        {
            return !(ex is OutOfMemoryException) &&
                   !(ex is DllNotFoundException) &&
                   !(ex is StackOverflowException) &&
                   !(ex is ThreadAbortException);
        }
    }
}