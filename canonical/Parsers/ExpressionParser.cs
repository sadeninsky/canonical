using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using canonical.Exceptions;

namespace canonical.Parsers
{
    public enum Operation
    {
        Plus,
        Minus
    }

    public static class ExpressionParser
    {
        private static readonly Regex OperandSymsRegex = new Regex("^[-\\da-z\\.\\^]+");

        // 2.3xyz^45
        // 2.3xyz
        // xy^2
        // x
        private static readonly Regex OperandRegex = new Regex("^(-?)((?:\\d+(?:\\.\\d+)?)?)([a-z]+)(\\^\\d+)?$");

        private static readonly Regex OperationRegex = new Regex("^(\\+|-)");
        private static readonly Regex BracedExpressionRegex = new Regex("^(-?)((?:\\d+(?:\\.\\d+)?)?)\\((.+?)\\)((?:/\\d+(?:\\.\\d+)?)?)$");

        /// <exception cref="ExpressionParserException"></exception>
        public static Operation? ParseOperation(string input, out string right)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ExpressionParserException("Выражение не задано");

            input = input.TrimStart();
            right = input;

            var match = OperationRegex.Match(input);
            if (!match.Success)
                throw new ExpressionParserException($"Ожидается операция +/-: {input}");

            Operation? operation;

            var sign = match.Groups[1].Value;
            switch (sign)
            {
                case "+":
                    operation = Operation.Plus;
                    break;
                case "-":
                    operation = Operation.Minus;
                    break;
                default:
                    throw new InvalidOperationException($"invalid sign: {sign}");
            }

            right = input.Substring(match.Value.Length);

            return operation;
        }

        public static bool TryParseOperation(string input, out Operation? operation, out string right)
        {
            operation = null;
            right = input;
            try
            {
                operation = ParseOperation(input, out right);
                return true;
            }
            catch (ExpressionParserException)
            {
                return false;
            }
        }

        private static decimal ParseDecimal(string value, bool isNegative)
        {
            var result = value.Length > 0 ? decimal.Parse(value, NumberStyles.AllowDecimalPoint, new NumberFormatInfo()) : 1m;
            if (isNegative)
                result = -result;
            return result;
        }

        /// <exception cref="ExpressionParserException"></exception>
        public static Operand ParseOperand(string input, out string right)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ExpressionParserException("Выражение не задано");

            input = input.TrimStart();
            right = input;

            var match = OperandSymsRegex.Match(input);
            if (!match.Success)
                throw new ExpressionParserException($"Ожидается операнд: {input}");

            var operandString = match.Value;

            match = OperandRegex.Match(operandString);
            if (!match.Success)
                throw new ExpressionParserException($"Ожидается операнд: {input}");

            var isNegative = match.Groups[1].Value.Length > 0;
            var strAmount = match.Groups[2].Value;
            var indeterminate = match.Groups[3].Value;
            var strExponent = match.Groups[4].Value;

            var amount = ParseDecimal(strAmount, isNegative);

            var exponent = strExponent.Length > 0 ? int.Parse(strExponent.Substring(1)) : 1;

            try
            {
                var operand = new Operand(indeterminate.ToCharArray(), amount, exponent);
                right = input.Substring(operandString.Length);
                return operand;
            }
            catch (OperandException ex)
            {
                throw new ExpressionParserException(ex);
            }
        }

        public static bool TryParseOperand(string input, out Operand operand, out string right)
        {
            operand = null;
            right = input;
            try
            {
                operand = ParseOperand(input, out right);
                return true;
            }
            catch (ExpressionParserException)
            {
                return false;
            }
        }

        public static bool TryParseBracedExpression(string input, out decimal multiplier, out string innerExpression, out decimal divider)
        {
            multiplier = divider = 1m;
            innerExpression = null;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            input = input.Trim();

            var match = BracedExpressionRegex.Match(input);
            if (match.Success)
            {
                var isNegative = match.Groups[1].Value.Length > 0;
                var strMultiplier = match.Groups[2].Value;
                multiplier = ParseDecimal(strMultiplier, isNegative);

                innerExpression = match.Groups[3].Value;

                var valueGroup4 = match.Groups[4].Value;
                var strDivider = valueGroup4.Length > 0 ? valueGroup4.Substring(1) : string.Empty;
                if (strDivider.Length > 0)
                    divider = ParseDecimal(strDivider, false);
            }

            return match.Success;
        }

        /// <exception cref="ExpressionParserException"></exception>
        public static IEnumerable<Operand> Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ExpressionParserException("Выражение не задано");

            decimal multiplier, divider;
            string innerExpression;

            if (TryParseBracedExpression(input, out multiplier, out innerExpression, out divider))
                input = innerExpression;

            multiplier = multiplier / divider;

            var needOperaion = false;
            do
            {
                string right;
                var kt = 1;
                if (needOperaion)
                {
                    var operation = ParseOperation(input, out right);
                    Debug.Assert(operation.HasValue);
                    if (operation.Value == Operation.Minus)
                        kt = -1;
                    input = right;
                }

                var operand = ParseOperand(input, out right).Multiply(multiplier * kt);
                input = right;
                yield return operand;
                needOperaion = true;
            } while (!string.IsNullOrWhiteSpace(input));
        }
    }
}