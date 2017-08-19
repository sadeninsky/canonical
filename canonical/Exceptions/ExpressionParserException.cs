using System;

namespace canonical.Exceptions
{
    public class ExpressionParserException : CanonicalException
    {
        public ExpressionParserException(string message) : base(message)
        {
        }

        public ExpressionParserException(Exception innerException) : base(innerException.Message, innerException)
        {
        }
    }
}