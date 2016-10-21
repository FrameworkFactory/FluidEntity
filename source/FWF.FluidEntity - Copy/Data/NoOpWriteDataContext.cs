using System.Data.Common;

namespace FWF.FluidEntity.Data
{
    public class NoOpWriteDataContext : IWriteDataContext
    {
        public IWriteDataSet<T> GetDataSet<T>() where T : class
        {
            return new LocalDataSet<T>(new NoOpDataContext());
        }

        public IDataContextTransaction BeginTransaction(DbTransaction useTransaction = null)
        {
            return new NoOpDataContextTransaction();
        }

        public void SaveChanges()
        {
        }

        public void Dispose()
        {
        }
    }
}
