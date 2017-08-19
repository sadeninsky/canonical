using System;

namespace canonical.Exceptions
{
    public class CanonicalException : Exception
    {
        protected CanonicalException(string message) : base(message)
        {
        }

        protected CanonicalException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CanonicalException(Exception innerException) : base(innerException.Message, innerException)
        {
        }
    }
}