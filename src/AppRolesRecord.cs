using Microsoft.AspNetCore.Identity;
using Netopes.Core.Helpers.Database;
using Netopes.Identity.Data;
using System;

namespace Netopes.Identity
{
    public class AppRolesRecord : RolesRecord<AppIdentityRole, Guid, IdentityRoleClaim<Guid>>
    {
        public AppRolesRecord(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory) { }
    }
}
