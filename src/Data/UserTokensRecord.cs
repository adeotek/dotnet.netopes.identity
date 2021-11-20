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
    ///     The default implementation of <see cref="IUserTokensRecord{TKey,TUserToken}" />.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    public class UserTokensRecord<TKey, TUserToken> :
        IdentityRecord,
        IUserTokensRecord<TKey, TUserToken>
        where TKey : IEquatable<TKey>
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        /// <summary>
        ///     Creates a new instance of <see cref="UserTokensRecord{TKey, TUserToken}" />.
        /// </summary>
        /// <param name="dbConnectionFactory">A factory for creating instances of <see cref="IDbConnection" />.</param>
        public UserTokensRecord(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TUserToken>> GetTokensAsync(TKey userId)
        {
            var sql = "select * " +
                               $"from {TN("UserTokens")} " +
                               $"where {CN("UserId")} = {GID("RoleId")};";
            var userTokens = await DbConnection.QueryAsync<TUserToken>(sql, new { UserId = userId });
            return userTokens;
        }

        /// <inheritdoc />
        public virtual async Task<TUserToken> FindTokenAsync(TKey userId, string loginProvider, string name)
        {
            var sql = "select * " +
                               $"from {TN("UserTokens")} " +
                               $"where {CN("UserId")} = {GID("RoleId")} AND {CN("LoginProvider")} = @LoginProvider AND {CN("Name")} = @Name;";
            var token = await DbConnection.QuerySingleOrDefaultAsync<TUserToken>(sql, new
            {
                UserId = userId,
                LoginProvider = loginProvider,
                Name = name
            });
            return token;
        }
    }
}