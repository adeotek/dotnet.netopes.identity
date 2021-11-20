using Dapper;
using Microsoft.AspNetCore.Identity;
using Netopes.Core.Helpers.Database;
using Netopes.Identity.Abstract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Netopes.Identity.Data
{
    /// <summary>
    ///     The default implementation of <see cref="IRoleClaimsRecord{TKey,TRoleClaim}" />.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
    /// <typeparam name="TRoleClaim">The type of the class representing a role claim.</typeparam>
    public class RoleClaimsRecord<TKey, TRoleClaim> :
        IdentityRecord,
        IRoleClaimsRecord<TKey, TRoleClaim>
        where TKey : IEquatable<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        /// <summary>
        ///     Creates a new instance of <see cref="RoleClaimsRecord{TKey, TRoleClaim}" />.
        /// </summary>
        /// <param name="dbConnectionFactory">A factory for creating instances of <see cref="IDbConnection" />.</param>
        public RoleClaimsRecord(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TRoleClaim>> GetClaimsAsync(TKey roleId)
        {
            var sql = "select * " +
                        $"from {TN("RoleClaims")} " +
                        $"where {CN("RoleId")} = {GID("RoleId")};";
            var roleClaims = await DbConnection.QueryAsync<TRoleClaim>(sql, new { RoleId = roleId });
            return roleClaims;
        }
    }
}