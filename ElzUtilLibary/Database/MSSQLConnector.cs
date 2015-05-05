using System;

namespace ElzUtilLibary.Database
{
    public class MsSqlConnector : AbstractDatabaseConnector
    {
        public MsSqlConnector(string connectionString) : base(Database.MsSql, connectionString)
        {
        }

        protected override string Escape(object dataValue)
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