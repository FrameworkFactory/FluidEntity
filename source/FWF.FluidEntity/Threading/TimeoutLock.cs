using System;
using System.Threading;

namespace FWF.FluidEntity.Threading
{
    public sealed class TimeoutLock : IDisposable
    {
        private readonly object _lockObject;
        private TimeSpan _timeout;
        private static readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(60);

        public TimeoutLock(object lockObject)
            : this(lockObject, _defaultTimeout)
        {
        }

        public TimeoutLock(object lockObject, TimeSpan timeout)
        {
            _lockObject = lockObject;
            _timeout = timeout;

            bool didEnter = Monitor.TryEnter(_lockObject, _timeout);

            if (!didEnter)
            {
                throw new TimeoutException(
                    string.Format(
                        "Unable to acquire a lock in {0}ms",
                        _timeout.TotalMilliseconds
                        )
                    );
            }
        }

        public void Dispose()
        {
            Monitor.Exit(_lockObject);
        }
        
    }
}
