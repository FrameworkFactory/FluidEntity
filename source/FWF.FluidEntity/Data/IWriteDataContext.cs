using System;
using System.Data.Common;

namespace FWF.FluidEntity.Data
{
    public interface IWriteDataContext : IDisposable
    {
        IWriteDataSet<T> GetDataSet<T>() where T : class;

        IDataContextTransaction BeginTransaction(DbTransaction useTransaction = null);

        void SaveChanges();
    }
}
