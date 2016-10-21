using System;
using System.Text;

namespace FWF.FluidEntity.Data
{
    public enum DataAuthenticationType
    {
        Windows = 0,
        Sql = 1
    }

    public class DataConnection
    {
        private string _sqlServerInstance = string.Empty;
        private string _database = string.Empty;

        private DataAuthenticationType _authType = DataAuthenticationType.Windows;
        private string _userName = string.Empty;
        private string _password = string.Empty;
        private string _applicationName = string.Empty;
        private string _workstationId = string.Empty;

        private const string ConnectionStringPrefix = "Persist Security Info=True;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True;";

        public string DatabaseName
        {
            get
            {
                return _database;
            }
            set
            {
                _database = value;
            }
        }

        public string DataCenter { get; set; }

        public string SqlServerInstance
        {
            get
            {
                return _sqlServerInstance;
            }
            set
            {
                _sqlServerInstance = value;
            }
        }

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        public DataAuthenticationType AuthenticationType
        {
            get
            {
                return _authType;
            }
            set
            {
                _authType = value;
            }
        }

        public string WorkstationId
        {
            get
            {
                return _workstationId;
            }
            set
            {
                _workstationId = value;
            }
        }

        public string ApplicationName
        {
            get
            {
                return _applicationName;
            }
            set
            {
                _applicationName = value;
            }
        }

        public string Render()
        {
            return RenderInternal(true);
        }

        public string RenderWithoutCredentials()
        {
            return RenderInternal(false);
        }

        private string RenderInternal(bool outputCredentials)
        {
            // Must have SqlServerInstance/DatabaseName to Render
            if (string.IsNullOrEmpty(_sqlServerInstance) || string.IsNullOrEmpty(_database))
            {
                throw new InvalidOperationException("Must populate SqlServerInstance and DatabaseName before calling this method.");
            }

            var connectionString = new StringBuilder();

            connectionString.Append(ConnectionStringPrefix);

            // TODO: Add Data Access Option Paramters to connection string
            /*
            connectionString.Append( "Connection Timeout=" + _DataAccessOptions.ConnectionTimeOut + ";" );
            connectionString.Append( "Packet Size=" + _DataAccessOptions.PacketSize + ";" );
            connectionString.Append( "Min Pool Size=" + _DataAccessOptions.MinPoolSize + ";" );
            connectionString.Append( "Max Pool Size=" + _DataAccessOptions.MaxPoolSize + ";" );
            */

            connectionString.AppendFormat("Data Source={0};", _sqlServerInstance);
            connectionString.AppendFormat("Initial Catalog={0};", _database);

            if (_authType == DataAuthenticationType.Windows)
            {
                connectionString.Append("Integrated Security=True;");
            }
            else
            {
                if (outputCredentials)
                {
                    connectionString.AppendFormat("User ID={0};Password={1};", _userName, _password);
                }
            }

            if (!string.IsNullOrEmpty(_applicationName))
            {
                connectionString.AppendFormat("Application Name={0};", _applicationName);
            }

            if (!string.IsNullOrEmpty(_workstationId))
            {
                connectionString.AppendFormat("Workstation ID={0};", _workstationId);
            }

            return connectionString.ToString();

        }

        public static DataConnection CreateFromString(string connectionString)
        {
            var dc = new DataConnection
            {
                SqlServerInstance = FindNamedValue(connectionString, "server", "Data Source"),
                DatabaseName = FindNamedValue(connectionString, "database", "Initial Catalog"),

                UserName = FindNamedValue(connectionString, "user id", "user"),
                Password = FindNamedValue(connectionString, "password", "pwd"),

                ApplicationName = FindNamedValue(connectionString, "Application Name"),
                WorkstationId = FindNamedValue(connectionString, "Workstation Id"),
            };

            if (!string.IsNullOrEmpty(dc.UserName))
            {
                dc.AuthenticationType = DataAuthenticationType.Sql;
            }

            return dc;
        }

        private static string FindNamedValue(string str, params string[] listNames)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            if (listNames == null)
            {
                return null;
            }

            const string delimiter = ";";

            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (var name in listNames)
            // ReSharper restore LoopCanBeConvertedToQuery
            {
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                string namePrefix = name + "=";

                if (str.IndexOf(namePrefix, StringComparison.OrdinalIgnoreCase) == -1)
                {
                    continue;
                }

                var startOfValue = str.IndexOf(namePrefix, StringComparison.OrdinalIgnoreCase) + name.Length + 1;
                var endOfValue = str.IndexOf(delimiter, startOfValue, StringComparison.OrdinalIgnoreCase);
                var length = endOfValue - startOfValue;

                if (length > 0)
                {
                    return str.Substring(startOfValue, length);
                }
            }

            return null;
        }

        public override string ToString()
        {
            return RenderWithoutCredentials();
        }

    }
}