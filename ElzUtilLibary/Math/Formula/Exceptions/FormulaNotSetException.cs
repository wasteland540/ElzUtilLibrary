using System;

namespace ElzUtilLibary.Math.Formula.Exceptions
{
    public class FormulaNotSetException : Exception
    {
        public FormulaNotSetException()
        {
        }

        public FormulaNotSetException(string message) : base(message)
        {
        }
    }
}