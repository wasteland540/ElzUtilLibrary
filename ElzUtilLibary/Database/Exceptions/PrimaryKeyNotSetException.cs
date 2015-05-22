using System;

namespace ElzUtilLibary.Database.Exceptions
{
    public class PrimaryKeyNotSetException : Exception
    {
        public PrimaryKeyNotSetException()
        {
        }

        public PrimaryKeyNotSetException(string message)
            : base(message)
        {
        }
    }
}