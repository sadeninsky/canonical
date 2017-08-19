using System.Globalization;
using System.Linq;
using canonical.Exceptions;
using canonical.Extensions;

namespace canonical
{
    public class Operand
    {
        private decimal _amount;

        private Operand(Operand other)
        {
            Indeterminate = other.Indeterminate;
            Amount = other.Amount;
            Exponent = other.Exponent;
        }

        /// <exception cref="OperandException"></exception>
        public Operand(char[] indeterminate, decimal amount = 1, int exponent = 1)
        {
            if (indeterminate == null || indeterminate.Length == 0)
                throw new OperandException("Переменных не задано");

            var duplicate = indeterminate.GroupBy(x => x).FirstOrDefault(x => x.Count() > 1);
            if (duplicate != null)
                throw new OperandException($"Переменная {duplicate.Key} у одного операнда встречается более одного раза");

            if (indeterminate.Any(x => x < 'a' || x > 'z'))
                throw new OperandException("Список переменных должен состоять из маленьких латинских букв");


            if (amount == 0)
                throw new OperandException("Числовая часть должна быть неравна нулю");

            Amount = amount;
            Indeterminate = string.Join(string.Empty, indeterminate.OrderBy(x => x).Select(x => x.ToString()));
            Exponent = exponent;
        }

        public decimal Amount
        {
            get { return _amount; }
            private set { _amount = value.TrimDecimal(); }
        }

        public string Indeterminate { get; }
        public int Exponent { get; }

        public bool IsPositive => Amount > 0;

        public override string ToString()
        {
            var strAmount =
                $"{(Amount == -1 ? "-" : string.Empty)}{(Amount != 1 && Amount != -1 ? Amount.ToString(CultureInfo.InvariantCulture) : string.Empty)}";
            return $"{strAmount}{Indeterminate}{(Exponent != 1 ? "^" + Exponent : string.Empty)}";
        }

        private Operand Clone()
        {
            return new Operand(this);
        }

        public Operand Multiply(decimal multiplier)
        {
            var result = Clone();
            result.Amount *= multiplier;
            return result;
        }
    }
}