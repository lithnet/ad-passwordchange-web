using System;
using System.Runtime.Serialization;

namespace Lithnet.ActiveDirectory.PasswordChange.Web.Exceptions
{
    [Serializable]
    internal class PasswordIncorrectException : Exception
    {
        public PasswordIncorrectException()
        {
        }

        public PasswordIncorrectException(string message) : base(message)
        {
        }

        public PasswordIncorrectException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PasswordIncorrectException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}