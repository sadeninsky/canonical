namespace canonical.Extensions
{
    public static class OperandExtension
    {
        public static string SignToString(this Operand thisOperand)
        {
            if (thisOperand == null)
                return string.Empty;

            return thisOperand.IsPositive ? "+" : "-";
        }

        public static string ForcePositiveToString(this Operand thisOperand)
        {
            if (thisOperand == null)
                return string.Empty;

            var operand = thisOperand.IsPositive ? thisOperand : thisOperand.Multiply(-1);

            return operand.ToString();
        }
    }
}