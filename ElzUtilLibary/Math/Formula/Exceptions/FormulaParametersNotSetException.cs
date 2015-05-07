using System;

namespace ElzUtilLibary.Math.Formula.Exceptions
{
    public class FormulaParametersNotSetException : Exception
    {
        public FormulaParametersNotSetException(string message)
            : base(message)
        {
        }

        public FormulaParametersNotSetException(params string[] parmeters)
        {
            ParametersNotSet = parmeters;
        }

        public FormulaParametersNotSetException(string message, params string[] parmeters) : base(message)
        {
            ParametersNotSet = parmeters;
        }

        public string[] ParametersNotSet { get; set; }
    }
}