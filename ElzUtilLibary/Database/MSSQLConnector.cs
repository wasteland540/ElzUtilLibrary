﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using ElzUtilLibary.Database.Attributes;
using ElzUtilLibary.Mapping;

namespace ElzUtilLibary.Database
{
    public class MsSqlConnector
    {
        private const string SqlInsertTemplate = "INSERT INTO {0} ({1}) VALUES ({2})";
        private readonly string _connectionString = string.Empty;

        public MsSqlConnector(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<T> GetData<T>(string sqlStatement) where T : new()
        {
            DataTable table = ReadData<T>(sqlStatement);

            return MappingHelper.MapDataTableToObjectList<T>(table);
        }

        public void InsertData(string sqlStatement)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand())
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
            if (tableName == "")
            {
                tableName = dataObject.GetType().Name;
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand())
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

        private DataTable ReadData<T>(string sqlStatement)
        {
            DataTable dataTable;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    dataTable = MappingHelper.CreateTypedDataTable<T>();
                    command.CommandText = sqlStatement;

                    SqlDataReader reader = command.ExecuteReader();
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

        private static void AddToColumnNamesAndValueList<T>(T dataObject, ref string columnNames,
            PropertyInfo propertyInfo,
            ref string valueList)
        {
            columnNames += propertyInfo.Name;
            columnNames += " ,";

            valueList += Escape(propertyInfo.GetValue(dataObject));
            valueList += " ,";
        }

        private static string Escape(object dataValue)
        {
            if (dataValue is Guid || dataValue is DateTime || dataValue is string)
            {
                return string.Format("'{0}'", dataValue);
            }

            if (dataValue is bool)
            {
                var val = (bool) dataValue;

                return val ? string.Format("{0}", 1) : string.Format("{0}", 0);
            }

            if (dataValue != null)
            {
                return dataValue.ToString();
            }

            return "NULL";
        }
    }
}