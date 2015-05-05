using System.Collections.Generic;

namespace ElzUtilLibary.Database
{
    public interface IDatabaseConnector
    {
        List<T> GetData<T>(string sqlStatement) where T : new();

        void InsertData(string sqlStatement);

        void InsertData<T>(T dataObject, string tableName = "");
    }
}