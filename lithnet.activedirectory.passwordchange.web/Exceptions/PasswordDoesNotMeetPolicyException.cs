using System;
using System.Runtime.Serialization;

namespace lithnet.activedirectory.passwordchange.web.Exceptions
{
    [Serializable]
    internal class PasswordDoesNotMeetPolicyException : Exception
    {
        public PasswordDoesNotMeetPolicyException()
        {
        }

        public PasswordDoesNotMeetPolicyException(string message) : base(message)
        {
        }

        public PasswordDoesNotMeetPolicyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PasswordDoesNotMeetPolicyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}