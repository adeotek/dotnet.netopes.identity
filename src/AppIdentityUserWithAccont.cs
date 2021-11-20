using System;

namespace Netopes.Identity
{
    public class AppIdentityUserWithAccount<TAccount> : AppIdentityUser
    {
        public Guid AccountId { get; set; }

        public TAccount Account { get; set; }
    }

    public class AppIdentityUserWithAccount : AppIdentityUserWithAccount<AppAccount>
    {
    }
}
