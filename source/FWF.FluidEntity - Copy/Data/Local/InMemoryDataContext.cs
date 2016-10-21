using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace FWF.FluidEntity.Data.Local
{
    public class InMemoryDataContext : IDataContext
    {
        private readonly IDictionary<Type, ArrayList> _items = new ConcurrentDictionary<Type, ArrayList>();

        public bool IsRunning
        {
            get { return true; }
        }

        public void Start()
        {
        }

        public void Stop()
        {
            _items.Clear();
        }

        public void Dispose()
        {
            // NOTE: Calling Dispose should not clear data.  It is only meant to dispose this instance
            // and release it to be used elsewhere, much like a sql connection pool.
        }

        public IQueryable<T> Get<T>() where T : class
        {
            if (!_items.ContainsKey(typeof(T)))
            {
                _items.Add(typeof(T), new ArrayList());
            }

            var typedItems = _items[typeof(T)];

            return typedItems.Cast<T>().AsQueryable();
        }

        public IList GetListForType(Type itemType)
        {
            if (!_items.ContainsKey(itemType))
            {
                _items.Add(itemType, new ArrayList());
            }

            var typedItems = _items[itemType];

            return typedItems;
        }

        public IWriteDataContext StartWriteSession()
        {
            return new LocalWriteDataContext(this);
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
