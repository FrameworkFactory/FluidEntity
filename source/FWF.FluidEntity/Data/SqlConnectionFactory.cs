using System.Collections.Concurrent;
using System.Configuration;
using FWF.FluidEntity.Logging;

namespace FWF.FluidEntity.Data
{
    public class SqlConnectionFactory
    {
        private readonly ConcurrentDictionary<string, DataConnection> _dataConnections = new ConcurrentDictionary<string, DataConnection>();

        private readonly ILog _log;

        public SqlConnectionFactory(
            ILogFactory logFactory
            )
        {
            _log = logFactory.CreateForType(this);
            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
            {
                _dataConnections.TryAdd(
                    connectionString.Name,
                    DataConnection.CreateFromString(connectionString.ConnectionString)
                    );
            }
        }

        public void Add(string connectionName, DataConnection dataConnection)
        {
            _dataConnections.TryAdd(connectionName, dataConnection);
        }

        public void Remove(string connectionName)
        {
            DataConnection dataConnection;
            _dataConnections.TryRemove(connectionName, out dataConnection);
        }

        public DataConnection Get(string connectionName)
        {
            DataConnection dataConnection;
            _dataConnections.TryGetValue(connectionName, out dataConnection);
            return dataConnection;
        }
    }
}