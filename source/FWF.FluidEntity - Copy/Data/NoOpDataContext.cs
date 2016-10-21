using System.Linq;

namespace FWF.FluidEntity.Data
{
    public class NoOpDataContext : IDataContext
    {
        private bool _running;

        public IQueryable<T> Get<T>() where T : class
        {
            return Enumerable.Empty<T>().AsQueryable();
        }

        public IWriteDataContext StartWriteSession()
        {
            return new NoOpWriteDataContext();
        }

        public bool IsRunning
        {
            get { return _running; }
        }

        public void Start()
        {
            _running = true;
        }

        public void Stop()
        {
            _running = false;
        }

        public void Dispose()
        {
        }

        public IProcResult<T> Execute<T>(IProc procedure)
        {
            return new ProcResult<T>();
        }

        public IProcResult Execute(IProc procedure)
        {
            return new ProcResult();
        }
    }
}
