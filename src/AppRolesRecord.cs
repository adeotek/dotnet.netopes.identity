using System;
using Microsoft.AspNetCore.Identity;
using Netopes.Core.Helpers.Database;
using Netopes.Identity.Data;

namespace Netopes.Identity
{
    public class AppRolesRecord : RolesRecord<AppIdentityRole, Guid, IdentityRoleClaim<Guid>>
    {
        public AppRolesRecord(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }
    }
}
