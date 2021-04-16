using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Netopes.Core.Helpers.Email;

namespace Netopes.Identity
{
    public class IdentityEmailSender : EmailSender, IEmailSender
    {
        public IdentityEmailSender(EmailSettings emailSettings, ILogger<IdentityEmailSender> logger) 
            : base(emailSettings, logger)
        {
        }
    }
}
