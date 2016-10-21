using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using FWF.FluidEntity.Threading;

namespace FWF.FluidEntity.Data.Local
{
    public sealed class LocalWriteDataContext : DisposableObject, IWriteDataContext, IDataContextTransaction
    {

        private readonly List<IPendingChanges> _sessionDataSets = new List<IPendingChanges>();
        private readonly InMemoryDataContext _inMemoryDataContext;

        private readonly object _lockObject = new object();

        private readonly IdProvider _idProvider = IdProvider.Current;

        public LocalWriteDataContext(InMemoryDataContext localdataContext)
        {
            _inMemoryDataContext = localdataContext;
        }
        
        public IWriteDataSet<T> GetDataSet<T>() where T : class
        {
            var dataSet = new LocalDataSet<T>(_inMemoryDataContext);
            _sessionDataSets.Add(dataSet);
            return dataSet;
        }

        public override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Commit();
            }
        }

        public void SaveChanges()
        {
            //
            using (new TimeoutLock(_lockObject))
            {
                if (!_sessionDataSets.Any())
                {
                    return;
                }

                foreach (var sessionDataSet in _sessionDataSets)
                {
                    Type setType = sessionDataSet.GetType();
                    Type itemType = setType.GetGenericArguments()[0];

                    IList list = _inMemoryDataContext.GetListForType(itemType);

                    foreach (var pendingItemWrapper in sessionDataSet.PendingChanges)
                    {
                        object pendingItem = pendingItemWrapper.Item;
                        object existingItem = list.Cast<object>().FirstOrDefault(item => item.Equals(pendingItem));

                        switch (pendingItemWrapper.Operation)
                        {
                            case CrudOperation.Add:
                                if (existingItem != null)
                                {
                                    throw new InvalidOperationException(); //ItemAlreadyExistsException();
                                }
                                UpdateDatabaseGeneratedFields(itemType, pendingItem);
                                list.Add(pendingItem);
                                break;

                            case CrudOperation.Update:
                                if (existingItem == null)
                                {
                                    throw new InvalidOperationException(); //ItemDoesNotExistsException();
                                }
                                list.Remove(existingItem);
                                list.Add(pendingItem);
                                break;

                            case CrudOperation.Delete:
                                if (existingItem != null)
                                {
                                    list.Remove(pendingItem);
                                }
                                break;
                        }

                    }
                }

                foreach (var sessionDataSet in _sessionDataSets)
                {
                    sessionDataSet.Clear();
                }
            }
        }

        private void UpdateDatabaseGeneratedFields(Type itemType, object item)
        {
            PropertyInfo[] listPropertyInfo = itemType.GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);

            foreach (var propertyInfo in listPropertyInfo)
            {
                var databaseGeneratedAttribute = propertyInfo.GetCustomAttribute<DatabaseGeneratedAttribute>();

                if (databaseGeneratedAttribute == null)
                {
                    continue;
                }

                if (databaseGeneratedAttribute.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                {
                    if (propertyInfo.PropertyType == typeof (int))
                    {
                        var id = (int) propertyInfo.GetValue(item);

                        if (id <= 0)
                        {
                            string idProviderName = string.Concat(itemType.FullName, "-", propertyInfo.Name);

                            propertyInfo.SetValue(
                                item,
                                _idProvider.GetNextId(idProviderName)
                                );
                        }
                    }
                }

                if (databaseGeneratedAttribute.DatabaseGeneratedOption == DatabaseGeneratedOption.Computed)
                {
                    if (propertyInfo.PropertyType == typeof (Guid))
                    {
                        propertyInfo.SetValue(
                            item,
                            Guid.NewGuid()
                            );
                    }
                    if (propertyInfo.PropertyType == typeof(DateTime))
                    {
                        propertyInfo.SetValue(
                            item,
                            DateTime.UtcNow
                            );
                    }
                    if (propertyInfo.PropertyType == typeof(byte[]))
                    {
                        string idProviderName = string.Concat(itemType.FullName, "-", propertyInfo.Name);

                        var id = _idProvider.GetNextId(idProviderName);
                        byte[] timestamp = BitConverter.GetBytes(id);

                        propertyInfo.SetValue(
                            item,
                            timestamp
                            );
                    }
                }
            }

        }

        public IDataContextTransaction BeginTransaction(DbTransaction useTransaction = null)
        {
            return this;
        }

        public void RollBack()
        {
            using (new TimeoutLock(_lockObject))
            {
                foreach (var sessionDataSet in _sessionDataSets)
                {
                    sessionDataSet.Clear();
                }
            }
        }

        public void Commit()
        {
            using (new TimeoutLock(_lockObject))
            {
                SaveChanges();

                // Commit, then clean
                RollBack();
            }
        }

    }

}
