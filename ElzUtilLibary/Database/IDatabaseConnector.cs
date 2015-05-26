using System;
using System.Collections.Generic;
using ElzUtilLibary.Database.BaseClasses;

namespace ElzUtilLibary.Database
{
    public interface IDatabaseConnector
    {
        List<T> GetData<T>(string sqlStatement) where T : Entity, new();

        void InsertData(string sqlStatement);

        [Obsolete("Please use 'InsertData<T>(T dataObject)' method and 'Tablename' attribute instead.")]
        void InsertData<T>(T dataObject, string tableName = "");

        void InsertData<T>(T dataObject) where T : Entity;

        void Update<T>(T dataObject, bool saveUpdate = false) where T : Entity;

        void Delete<T>(T dataObject, bool saveDelete = false) where T : Entity;
    }
}