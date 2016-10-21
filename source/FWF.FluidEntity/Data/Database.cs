using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;

namespace FWF.FluidEntity.Data
{
    public class Database
    {

        private readonly IDbConnection _dbConnection;
        private readonly IDataProvider _dataProvider;
        private readonly DbProviderFactory _dbProviderFactory;
        

        public Database(IDbConnection dbConnection)
        {
            if (dbConnection == null)
            {
                throw new ArgumentNullException("dbConnection");
            }

            _dbConnection = dbConnection;

            _dataProvider = Resolve(_dbConnection.GetType());

            _dbProviderFactory = _dataProvider.GetProviderFactory();
        }

        private IDataProvider Resolve(Type dbConnectionType)
        {
            var typeName = dbConnectionType.Name;
            
            // Assume SQL Server
            return new SqlServerDatabaseProvider();
        }





        public IDbCommand CreateCommand(IDbConnection connection, string sql, params object[] args)
        {
            // Create the command and add parameters
            var cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = sql;

            foreach (var item in args)
            {
                AddParam(cmd, item);
            }
            
            return cmd;
        }

        private void AddParam(IDbCommand cmd, object value)
        {
            // Support passed in parameters
            var idbParam = value as IDbDataParameter;
            if (idbParam != null)
            {
                idbParam.ParameterName = string.Format("{0}{1}", "@", cmd.Parameters.Count);
                cmd.Parameters.Add(idbParam);
                return;
            }

            // Create the parameter
            var p = cmd.CreateParameter();
            p.ParameterName = string.Format("{0}{1}", "@", cmd.Parameters.Count);

            // Assign the parmeter value
            if (value == null)
            {
                p.Value = DBNull.Value;
            }
            else
            {
                // Give the database type first crack at converting to DB required type
                value = _dataProvider.MapParameterValue(value);

                var t = value.GetType();
                if (t.IsEnum) // PostgreSQL .NET driver wont cast enum to int
                {
                    p.Value = Convert.ChangeType(value, ((Enum)value).GetTypeCode());
                }
                else if (t == typeof(string))
                {
                    p.Size = Math.Max((value as string).Length + 1, 4000); // Help query plan caching by using common size
                    p.Value = value;
                }
                else if (value.GetType().Name == "SqlGeography") //SqlGeography is a CLR Type
                {
                    p.GetType().GetProperty("UdtTypeName").SetValue(p, "geography", null); //geography is the equivalent SQL Server Type
                    p.Value = value;
                }
                else if (value.GetType().Name == "SqlGeometry") //SqlGeometry is a CLR Type
                {
                    p.GetType().GetProperty("UdtTypeName").SetValue(p, "geometry", null); //geography is the equivalent SQL Server Type
                    p.Value = value;
                }
                else
                {
                    p.Value = value;
                }
            }

            // Add to the collection
            cmd.Parameters.Add(p);
        }






        public IEnumerable<T> Query<T>(string sql, params object[] args)
        {
            using (var cmd = CreateCommand(_dbConnection, sql, args))
            {
                IDataReader r;
                try
                {
                    r = cmd.ExecuteReader();
                }
                catch (Exception ex)
                {
                    throw;
                    //yield break;
                }

                //var pd = PocoData.ForType(typeof(T), _defaultMapper);

                //var factory = pd.GetFactory(
                //    cmd.CommandText, 
                //    _sharedConnection.ConnectionString, 
                //    0, 
                //    r.FieldCount, 
                //    r, 
                //    _defaultMapper
                //    ) as Func<IDataReader, T>;

                using (r)
                {
                    while (true)
                    {
                        T poco = default(T);
                        try
                        {
                            if (!r.Read())
                            {
                                yield break;
                            }

                            //poco = factory(r);
                        }
                        catch (Exception ex)
                        {
                            throw;
                            //yield break;
                        }

                        yield return poco;
                    }
                }

            }
            
        }


    }
}
