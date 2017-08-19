using System;

namespace canonical.Exceptions
{
    public class EquationParserException : CanonicalException
    {
        public EquationParserException(string message) : base(message)
        {
        }

        public EquationParserException(Exception innerException) : base(innerException.Message, innerException)
        {
        }
    }
}