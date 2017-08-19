using System.Collections.Generic;
using System.Text.RegularExpressions;
using canonical.Exceptions;

namespace canonical.Parsers
{
    public static class EquationParser
    {
        private static readonly Regex IsThisEquationRegex = new Regex("^((?:(?<=\\s*)[^=]+(?=\\s*)))=((?:(?<=\\s*)[^=]+(?=\\s*)))$");

        public static bool IsThisEquation(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            return IsThisEquationRegex.IsMatch(str);
        }

        public static bool IsThisEquation(string str, out string left, out string right)
        {
            left = right = null;

            if (string.IsNullOrWhiteSpace(str))
                return false;

            var match = IsThisEquationRegex.Match(str);
            if (match.Success)
            {
                left = match.Groups[1].Value.Trim();
                right = match.Groups[2].Value.Trim();
            }
            return match.Success;
        }

        /// <exception cref="EquationParserException"></exception>
        public static IEnumerable<Operand> Parse(string input)
        {
            string leftExpr, rightExpr;
            if (!IsThisEquation(input, out leftExpr, out rightExpr))
                throw new EquationParserException($"Строка \"{input}\" не является уравнением");

            try
            {
                var result = new List<Operand>();
                foreach (var operand in ExpressionParser.Parse(leftExpr))
                    result.Add(operand);
                foreach (var operand in ExpressionParser.Parse(rightExpr))
                    result.Add(operand.Multiply(-1));
                return result;
            }
            catch (ExpressionParserException ex)
            {
                throw new EquationParserException(ex);
            }
        }
    }
}