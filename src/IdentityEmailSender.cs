using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Netopes.Core.Abstraction.Settings;
using Netopes.Core.Helpers.Services;

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
