using System.Collections.Generic;

namespace FWF.FluidEntity.Data.Local
{
    public sealed class LocalDataSet<T> : IWriteDataSet<T> where T : class 
    {
        private readonly IDataContext _localReadDataContext;
        private readonly HashSet<DataSetItem> _items = new HashSet<DataSetItem>();

        public LocalDataSet(IDataContext localReadDataContext)
        {
            _localReadDataContext = localReadDataContext;
        }

        public void Clear()
        {
            _items.Clear();
        }

        public IEnumerable<DataSetItem> PendingChanges
        {
            get { return _items; }
        }
        
        public void Add(T item)
        {
            var dataSetItem = new DataSetItem
            {
                Operation = CrudOperation.Add,
                Item = item
            };

            _items.Add(dataSetItem);
        }

        public void Remove(T item)
        {
            var dataSetItem = new DataSetItem
            {
                Operation = CrudOperation.Delete,
                Item = item
            };

            _items.Add(dataSetItem);
        }

        public void Update(T item)
        {
            var dataSetItem = new DataSetItem
            {
                Operation = CrudOperation.Update,
                Item = item
            };

            _items.Add(dataSetItem);
        }
        
    }
}
