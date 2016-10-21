using System;
using System.Threading;

namespace FWF.FluidEntity.Threading
{
    public delegate void RestartableThreadStart(ManualResetEvent resetEvent, object state);

    public class RestartableThread : Startable
    {
        private readonly string _name;
        private readonly RestartableThreadStart _threadStart;
        private readonly bool _isBackground;
        private readonly object _objectState;
        private readonly ManualResetEvent _manualResetEvent = new ManualResetEvent(false);

        private Thread _thread;

        public RestartableThread(
            string name,
            RestartableThreadStart threadStart,
            bool isBackground = true,
            object objectState = null
            )
        {
            _name = name;
            _threadStart = threadStart;
            _isBackground = isBackground;
            _objectState = objectState;
        }

        protected override void OnStart()
        {
            _thread = new Thread(ThreadStart)
            {
                Name = _name,
                IsBackground = _isBackground
            };

            _manualResetEvent.Reset();
            _thread.Start();
        }

        protected override void OnStop()
        {
            _manualResetEvent.Set();

            if (_thread != null)
            {
                if (_thread.IsAlive)
                {
                    _thread.Join(TimeSpan.FromSeconds(5));
                }
            }
        }

        private void ThreadStart()
        {
            if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
            {
                Thread.CurrentThread.Name = _name;
            }

            _threadStart(_manualResetEvent, _objectState);
        }

    }
}
