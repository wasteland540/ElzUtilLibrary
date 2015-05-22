using System;
using System.Collections.Generic;

namespace ElzUtilLibary.Database
{
    public interface IDatabaseConnector
    {
        List<T> GetData<T>(string sqlStatement) where T : new();

        void InsertData(string sqlStatement);

        [Obsolete("Please use 'InsertData<T>(T dataObject)' method and 'Tablename' attribute instead.")]
        void InsertData<T>(T dataObject, string tableName = "");

        void InsertData<T>(T dataObject);

        void Update<T>(T dataObject, bool saveUpdate = false);

        void Delete<T>(T dataObject, bool saveDelete = false);
    }
}