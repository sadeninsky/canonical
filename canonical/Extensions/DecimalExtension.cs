using System;

namespace canonical.Extensions
{
    public static class DecimalExtension
    {
        public static decimal TrimDecimal(this decimal val)
        {
            if (val == 0)
                return decimal.Zero;

            try
            {
                return decimal.Parse(val.ToString("#.###"));
            }
            catch (FormatException)
            {
                return decimal.Zero;
            }
        }
    }
}