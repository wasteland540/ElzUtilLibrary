using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Reflection;
using ElzUtilLibary.Database.Attributes;
using ElzUtilLibary.Database.Exceptions;
using ElzUtilLibary.Mapping;
using MySql.Data.MySqlClient;

namespace ElzUtilLibary.Database
{
    public abstract class AbstractDatabaseConnector : IDatabaseConnector
    {
        public enum Database
        {
            MsSql,
            MySql,
            Sqlite,
        }

        private const string SqlInsertTemplate = "INSERT INTO {0} ({1}) VALUES ({2})";
        private const string SqlUpdateTemplate = "UPDATE {0} SET {1} WHERE {2}";
        private const string SqlDeleteTemplate = "DELETE FROM {0} WHERE {1}";

        private const string UnsafePrimaryKeyNotSetExecptionMessage =
            "Primary Key not set for entity '{0}'. If you using the unsafe delete/update method you have to declare a primary key!";
        protected readonly string ConnectionString = string.Empty;
        protected Database DatabaseType;

        internal AbstractDatabaseConnector(Database database, string connectionString)
        {
            DatabaseType = database;
            ConnectionString = connectionString;
        }

        public List<T> GetData<T>(string sqlStatement) where T : new()
        {
            DataTable table = ReadData<T>(sqlStatement);

            return MappingHelper.MapDataTableToObjectList<T>(table);
        }

        public void InsertData(string sqlStatement)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();

                using (IDbCommand command = GetDbCommand())
                {
                    command.Connection = connection;
                    command.CommandText = sqlStatement;

                    command.ExecuteNonQuery();

                    command.Dispose();
                }

                connection.Close();
                connection.Dispose();
            }
        }

        public void InsertData<T>(T dataObject, string tableName = "")
        {
            if (tableName == string.Empty)
            {
                tableName = dataObject.GetType().Name;
            }

            Insert(dataObject, tableName);
        }

        public void InsertData<T>(T dataObject)
        {
            var tableName = ResolveTablename(dataObject);

            Insert(dataObject, tableName);
        }

        public void Update<T>(T dataObject, bool saveUpdate = false)
        {
            if (saveUpdate)
            {
                throw new NotSupportedException("Only unsafe update method implemented yet.");
            }

            var tableName = ResolveTablename(dataObject);

            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();

                using (IDbCommand command = GetDbCommand())
                {
                    command.Connection = connection;
                    command.CommandText = BuildSqlUpdateStatement(dataObject, tableName, saveUpdate);

                    command.ExecuteNonQuery();

                    command.Dispose();
                }

                connection.Close();
                connection.Dispose();
            }
        }

        public void Delete<T>(T dataObject, bool saveDelete = false)
        {
            var tableName = ResolveTablename(dataObject);

            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();

                using (IDbCommand command = GetDbCommand())
                {
                    command.Connection = connection;
                    command.CommandText = BuildSqlDeleteStatement(dataObject, tableName, saveDelete);

                    command.ExecuteNonQuery();

                    command.Dispose();
                }

                connection.Close();
                connection.Dispose();
            }
        }

        private DataTable ReadData<T>(string sqlStatement)
        {
            DataTable dataTable;

            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();

                using (IDbCommand command = GetDbCommand())
                {
                    command.Connection = connection;

                    dataTable = MappingHelper.CreateTypedDataTable<T>();
                    command.CommandText = sqlStatement;

                    IDataReader reader = command.ExecuteReader();
                    dataTable.Load(reader);

                    command.Dispose();
                }

                connection.Close();
                connection.Dispose();
            }

            return dataTable;
        }

        private string BuildSqlInsertStatement<T>(T dataObject, string tableName)
        {
            string columnNames = string.Empty;
            string valueList = string.Empty;

            Type type = typeof (T);
            PropertyInfo[] properties = type.GetProperties();

            foreach (PropertyInfo propertyInfo in properties)
            {
                if (IsPrimaryKey(propertyInfo))
                {
                    if (!IsPrimaryKeyAutogenerated(propertyInfo))
                    {
                        AddToColumnNamesAndValueList(dataObject, ref columnNames, propertyInfo, ref valueList);
                    }
                }
                else
                {
                    AddToColumnNamesAndValueList(dataObject, ref columnNames, propertyInfo, ref valueList);
                }
            }

            columnNames = columnNames.Substring(0, columnNames.Length - 2);
            valueList = valueList.Substring(0, valueList.Length - 2);

            return string.Format(SqlInsertTemplate, tableName, columnNames, valueList);
        }

        private string BuildSqlUpdateStatement<T>(T dataObject, string tableName, bool saveUpdate)
        {
            string whereClause = BuildWhereClause(dataObject, saveUpdate);
            string setClause = BuildSetClause(dataObject);

            return string.Format(SqlUpdateTemplate, tableName, setClause, whereClause);
        }

        private string BuildSqlDeleteStatement<T>(T dataObject, string tableName, bool saveDelete)
        {
            string whereClause = BuildWhereClause(dataObject, saveDelete);

            return string.Format(SqlDeleteTemplate, tableName, whereClause);
        }

        private string BuildWhereClause<T>(T dataObject, bool save)
        {
            string whereClause = string.Empty;

            Type type = typeof (T);
            PropertyInfo[] properties = type.GetProperties();

            if (save)
            {
                foreach (PropertyInfo propertyInfo in properties)
                {
                    var columnName = propertyInfo.Name;
                    var columnValue = Escape(propertyInfo.GetValue(dataObject));

                    if (columnValue == "NULL")
                    {
                        whereClause += string.Format("{0} IS {1} AND ", columnName, columnValue);
                    }
                    else
                    {
                        whereClause += string.Format("{0} = {1} AND ", columnName, columnValue);
                    }
                }

                whereClause = whereClause.Substring(0, whereClause.Length - 5);
            }
            else
            {
                foreach (PropertyInfo propertyInfo in properties)
                {
                    if (IsPrimaryKey(propertyInfo))
                    {
                        var columnName = propertyInfo.Name;
                        var columnValue = Escape(propertyInfo.GetValue(dataObject));

                        whereClause = string.Format("{0} = {1}", columnName, columnValue);
                        break;
                    }
                }

                if (string.IsNullOrEmpty(whereClause))
                {
                    throw new PrimaryKeyNotSetException(string.Format(UnsafePrimaryKeyNotSetExecptionMessage,
                        dataObject.GetType().FullName));
                }
            }
            return whereClause;
        }

        private string BuildSetClause<T>(T dataObject)
        {
            string setClause = string.Empty;

            Type type = typeof (T);
            PropertyInfo[] properties = type.GetProperties();
            
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (!IsPrimaryKey(propertyInfo))
                {
                    var columnName = propertyInfo.Name;
                    var columnValue = Escape(propertyInfo.GetValue(dataObject));

                    setClause += string.Format("{0} = {1}, ", columnName, columnValue);    
                }
            }

            setClause = setClause.Substring(0, setClause.Length - 2);

            return setClause;
        }

        private bool IsPrimaryKeyAutogenerated(PropertyInfo propertyInfo)
        {
            if (Attribute.IsDefined(propertyInfo, typeof (PrimaryKeyAutogeneratedByDatabase)))
            {
                var autoGenerated =
                    (PrimaryKeyAutogeneratedByDatabase)
                        Attribute.GetCustomAttribute(propertyInfo, typeof (PrimaryKeyAutogeneratedByDatabase));

                return autoGenerated.IsAutogenerated;
            }

            return true;
        }

        private bool IsPrimaryKey(PropertyInfo propertyInfo)
        {
            return Attribute.IsDefined(propertyInfo, typeof (PrimaryKey));
        }

        private void AddToColumnNamesAndValueList<T>(T dataObject, ref string columnNames,
            PropertyInfo propertyInfo,
            ref string valueList)
        {
            columnNames += propertyInfo.Name;
            columnNames += " ,";

            valueList += Escape(propertyInfo.GetValue(dataObject));
            valueList += " ,";
        }

        private IDbConnection GetDbConnection()
        {
            switch (DatabaseType)
            {
                case Database.MsSql:
                    return new SqlConnection(ConnectionString);
                case Database.MySql:
                    return new MySqlConnection(ConnectionString);
                case Database.Sqlite:
                    return new SQLiteConnection(ConnectionString);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IDbCommand GetDbCommand()
        {
            switch (DatabaseType)
            {
                case Database.MsSql:
                    return new SqlCommand();
                case Database.MySql:
                    return new MySqlCommand();
                case Database.Sqlite:
                    return new SQLiteCommand();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Insert<T>(T dataObject, string tableName)
        {
            using (IDbConnection connection = GetDbConnection())
            {
                connection.Open();

                using (IDbCommand command = GetDbCommand())
                {
                    command.Connection = connection;
                    command.CommandText = BuildSqlInsertStatement(dataObject, tableName);

                    command.ExecuteNonQuery();

                    command.Dispose();
                }

                connection.Close();
                connection.Dispose();
            }
        }

        private string ResolveTablename(object dataObject)
        {
            string tableName;

            var tablenameAttribute = (Tablename) dataObject.GetType().GetCustomAttribute(typeof(Tablename));

            if (tablenameAttribute != null)
            {
                tableName = tablenameAttribute.Name == string.Empty
                    ? dataObject.GetType().Name
                    : tablenameAttribute.Name;
            }
            else
            {
                tableName = dataObject.GetType().Name;
            }

            return tableName;
        }

        protected abstract string Escape(object dataValue);
    }
}