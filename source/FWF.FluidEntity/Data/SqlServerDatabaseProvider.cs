using System;
using System.Data;
using System.Data.Common;

namespace FWF.FluidEntity.Data
{
    public class SqlServerDatabaseProvider : IDataProvider
    {

        private const string ClientFactoryTypeName = "System.Data.SqlClient.SqlClientFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

        public DbProviderFactory GetProviderFactory()
        {
            var ft = Type.GetType(ClientFactoryTypeName);

            if (ft == null)
            {
                throw new ArgumentException("Could not load the " + GetType().Name + " DbProviderFactory.");
            }

            return (DbProviderFactory)ft.GetField("Instance").GetValue(null);
        }

        public virtual object MapParameterValue(object value)
        {
            if (value is bool)
                return ((bool)value) ? 1 : 0;

            return value;
        }

    }
}
