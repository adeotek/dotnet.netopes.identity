using System;
using Microsoft.AspNetCore.Identity;

namespace Netopes.Identity
{
    public class AppIdentityRole : IdentityRole<Guid>
    {
        public AppIdentityRole()
        {
        }
    }
}
