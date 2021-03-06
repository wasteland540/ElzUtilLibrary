﻿using System;

namespace ElzUtilLibary.Database
{
    public class MySqlConnector : AbstractDatabaseConnector
    {
        public MySqlConnector(string connectionString) : base(Database.MySql, connectionString)
        {
        }

        protected override string Escape(object dataValue)
        {
            if (dataValue is Guid || dataValue is string)
            {
                return string.Format("'{0}'", dataValue);
            }

            if (dataValue is bool)
            {
                var val = (bool) dataValue;

                return val ? string.Format("{0}", 1) : string.Format("{0}", 0);
            }

            if (dataValue is DateTime)
            {
                var date = ((DateTime) dataValue);

                return string.Format("'{0}-{1}-{2}'", date.Year, date.Month, date.Day);
            }

            if (dataValue != null)
            {
                return dataValue.ToString();
            }

            return "NULL";
        }
    }
}