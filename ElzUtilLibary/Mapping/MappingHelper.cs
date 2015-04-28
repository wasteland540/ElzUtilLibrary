using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace ElzUtilLibary.Mapping
{
    public class MappingHelper
    {
        public static List<T> MapDataTableToObjectList<T>(DataTable dataTable) where T : new()
        {
            var resultList = new List<T>();

            Type type = typeof (T);
            var properties = type.GetProperties();

            foreach (DataRow row in dataTable.Rows)
            {
                var dataObject = new T();

                foreach (PropertyInfo propertyInfo in properties)
                {
                    var name = propertyInfo.Name;
                    var rowValue = row[name];

                    type.GetProperty(name).SetValue(dataObject, rowValue);
                }

                resultList.Add(dataObject);
            }

            return resultList;
        }

        public static DataTable CreateTypedDataTable<T>()
        {
            var dataTable = new DataTable();

            Type type = typeof (T);
            var properties = type.GetProperties();

            foreach (PropertyInfo propertyInfo in properties)
            {
                dataTable.Columns.Add(propertyInfo.Name, propertyInfo.PropertyType);
            }

            return dataTable;
        }
    }
}