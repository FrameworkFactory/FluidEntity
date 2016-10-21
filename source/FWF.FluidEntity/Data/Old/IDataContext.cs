using System;
using System.Linq;

namespace FWF.FluidEntity.Data
{
    public interface IDataContext : IRunnable, IDisposable
    {
        IQueryable<T> Get<T>() where T : class;

        IWriteDataContext StartWriteSession();
    }
}
