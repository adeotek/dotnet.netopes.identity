using System;

namespace Netopes.Identity
{
    public class UserAuthenticationException : Exception
    {
        public string UserMessage { get; }

        public UserAuthenticationException(string message, string userMessage = null) : base(message)
        {
            UserMessage = userMessage;
        }
    }
}