using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace EFSession.Extensions
{
    public static class DbConnectionExtension
    {
        private static readonly SemaphoreSlim connectionLock = new SemaphoreSlim(0);

        public static void EnsureConnectionOpened(this DbConnection connection, TimeSpan? waitForOpenTimeout = null)
        {
            if (connection.State == ConnectionState.Broken)
            {
                connection.Close();
            }

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            if (connection.State == ConnectionState.Connecting)
            {
                connection.StateChange += ReleaseOnOpened;

                connectionLock.Wait(waitForOpenTimeout ?? TimeSpan.FromSeconds(5));

                connection.StateChange -= ReleaseOnOpened;
            }
        }

        public static async Task EnsureConnectionOpenedAsync(this DbConnection connection, TimeSpan? waitForOpenTimeout = null,
                                                            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (connection.State == ConnectionState.Broken)
            {
                connection.Close();
            }

            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync(cancellationToken);
            }

            if (connection.State == ConnectionState.Connecting)
            {
                connection.StateChange += ReleaseOnOpened;

                await connectionLock.WaitAsync(waitForOpenTimeout ?? TimeSpan.FromSeconds(5), cancellationToken);

                connection.StateChange -= ReleaseOnOpened;
            }
        }

        private static void ReleaseOnOpened(object sender, StateChangeEventArgs args)
        {
            if (args.CurrentState == ConnectionState.Open)
            {
                connectionLock.Release();
            }
        }
    }
}