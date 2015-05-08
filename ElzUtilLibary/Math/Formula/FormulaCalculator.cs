using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ElzUtilLibary.Math.Formula.Exceptions;

namespace ElzUtilLibary.Math.Formula
{
    public class FormulaCalculator
    {
        //TODO: implement support for 'x^y' 
        private const string ParameterPattern = "[A-Za-z]+\\.?([A-Za-z]+)?";
        private readonly Regex _regex;
        private Dictionary<string, double?> _parameters;
        private string _originalFormula;

        public FormulaCalculator()
        {
            _regex = new Regex(ParameterPattern);
        }

        public FormulaCalculator(string formula)
        {
            _regex = new Regex(ParameterPattern);
            Formula = formula;
        }

        // ReSharper disable once ValueParameterNotUsed
        public bool HasParameters
        {
            get { return Parameters != null && Parameters.Keys.Count > 0; }
            private set { }
        }

        public double Result { get; set; }
        public string Formula { get; set; }

        public Dictionary<string, double?> Parameters
        {
            get { return _parameters ?? (_parameters = new Dictionary<string, double?>()); }
            private set { _parameters = value; }
        }

        public void Clean()
        {
            Formula = string.Empty;
            Parameters = null;
            Result = double.NaN;
            HasParameters = false;
        }

        public void Parse()
        {
            if (!string.IsNullOrEmpty(Formula))
            {
                //remove whitespace
                Formula = Formula.Replace(" ", "");
                _originalFormula = Formula;

                //determine parameters
                MatchCollection matches = _regex.Matches(Formula);

                if (matches.Count > 0)
                {
                    _parameters = new Dictionary<string, double?>();

                    foreach (Match match in matches)
                    {
                        //check if key already exists!
                        if (!_parameters.ContainsKey(match.Value))
                        {
                            _parameters.Add(match.Value, null);
                        }
                    }
                }
            }
            else
            {
                throw new FormulaNotSetException("Formula is null or empty!");
            }
        }

        public void Calculate()
        {
            if (HasParameters)
            {
                if (_parameters.Any(keyVa => keyVa.Value == null))
                {
                    throw new FormulaParametersNotSetException("Parameters aren't all set!");
                }

                Formula = _originalFormula;

                //replace parameters with values
                foreach (string parameterKey in _parameters.Keys)
                {
                    double? param = _parameters[parameterKey];

                    
                    if (param != null)
                    {
                        Formula = Formula.Replace(parameterKey, param.Value.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }

            Result = Evaluate(Formula);
        }

        private static double Evaluate(string formula)
        {
            var table = new DataTable();
            table.Columns.Add("formula", string.Empty.GetType(), formula);
            DataRow row = table.NewRow();
            table.Rows.Add(row);

            return double.Parse((string) row["formula"]);
        }
    }
}