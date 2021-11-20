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
    ///     The default implementation of <see cref="IUserClaimsRecord{TKey,TUserClaim}" />.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
    public class UserClaimsRecord<TKey, TUserClaim> :
        IdentityRecord,
        IUserClaimsRecord<TKey, TUserClaim>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
    {
        /// <summary>
        ///     Creates a new instance of <see cref="UserClaimsRecord{TKey, TUserClaim}" />.
        /// </summary>
        /// <param name="dbConnectionFactory">A factory for creating instances of <see cref="IDbConnection" />.</param>
        public UserClaimsRecord(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TUserClaim>> GetClaimsAsync(TKey userId)
        {
            var sql = "select * " +
                               $"from {TN("UserClaims")} " +
                               $"where {CN("UserId")} = {GID("UserId")};";
            var userClaims = await DbConnection.QueryAsync<TUserClaim>(sql, new { UserId = userId });
            return userClaims;
        }
    }
}