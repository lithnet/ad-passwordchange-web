using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    [Serializable]
    public class RateLimitExceededException : Exception
    {
        public RateLimitExceededException()
        {
        }

        public RateLimitExceededException(string message)
            : base(message)
        {
        }

        public RateLimitExceededException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected RateLimitExceededException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}