using System.Collections.Generic;
using System.Linq;
using System.Text;
using canonical.Exceptions;
using canonical.Extensions;
using canonical.Parsers;

namespace canonical
{
    public class Expression
    {
        private readonly List<Operand> _operands;

        public Expression(IEnumerable<Operand> operands)
        {
            _operands = operands?.ToList() ?? new List<Operand>();
        }

        public override string ToString()
        {
            if (_operands.Count == 0)
                return "0";
            var sb = new StringBuilder(_operands[0].ToString());
            foreach (var operand in _operands.Skip(1))
                sb
                    .Append(" ")
                    .Append(operand.SignToString())
                    .Append(" ")
                    .Append(operand.ForcePositiveToString())
                    ;
            sb.Append(" = 0");
            return sb.ToString();
        }

        /// <exception cref="EquationParserException"></exception>
        public static Expression Parse(string input)
        {
            return new Expression(EquationParser.Parse(input));
        }

        /// <exception cref="OperandException"></exception>
        public Expression Simplify()
        {
            var result = new List<Operand>();
            foreach (var grouping in _operands.GroupBy(x => new {x.Indeterminate, x.Exponent}))
            {
                var amount = grouping.Sum(x => x.Amount);
                if (amount != 0)
                    result.Add(new Operand(grouping.Key.Indeterminate.ToCharArray(), amount, grouping.Key.Exponent));
            }
            return new Expression(result.OrderByDescending(x => x.Exponent).ThenBy(x => x.Indeterminate));
        }
    }
}