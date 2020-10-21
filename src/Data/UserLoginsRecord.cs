using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Netopes.Identity.Abstract;
using Microsoft.AspNetCore.Identity;
using Netopes.Core.Helpers.Database;

namespace Netopes.Identity.Data
{
    /// <summary>
    ///     The default implementation of <see cref="IUserLoginsRecord{TUser,TKey,TUserLogin}" />.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    public class UserLoginsRecord<TUser, TKey, TUserLogin> :
        IdentityRecord,
        IUserLoginsRecord<TUser, TKey, TUserLogin>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserLogin : IdentityUserLogin<TKey>, new()
    {
        /// <summary>
        ///     Creates a new instance of <see cref="UserLoginsRecord{TUser, TKey, TUserLogin}" />.
        /// </summary>
        /// <param name="dbConnectionFactory">A factory for creating instances of <see cref="IDbConnection" />.</param>
        public UserLoginsRecord(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TUserLogin>> GetLoginsAsync(TKey userId)
        {
            var sql = "select * " +
                               $"from {TN("UserLogins")} " +
                               $"where {CN("UserId")} = {GID("UserId")};";
            var userLogins = await DbConnection.QueryAsync<TUserLogin>(sql, new {UserId = userId});
            return userLogins;
        }

        /// <inheritdoc />
        public virtual async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey)
        {
            var sql = "select u.* " +
                               $"from {TN("Users")} u " +
                               $"inner join {TN("UserLogins")} ul on ul.{CN("UserId")} = u.{CN("Id")} " +
                               $"where ul.{CN("LoginProvider")} = @LoginProvider AND ul.{CN("ProviderKey")} = @ProviderKey;";
            var user = await DbConnection.QuerySingleOrDefaultAsync<TUser>(sql, new
            {
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            });
            return user;
        }

        /// <inheritdoc />
        public virtual async Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey)
        {
            var sql = "select * " +
                               $"from {TN("UserLogins")} " +
                               $"where {CN("LoginProvider")} = @LoginProvider AND {CN("ProviderKey")} = @ProviderKey;";
            var userLogin = await DbConnection.QuerySingleOrDefaultAsync<TUserLogin>(sql, new
            {
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            });
            return userLogin;
        }

        /// <inheritdoc />
        public virtual async Task<TUserLogin> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey)
        {
            var sql = "select * " +
                               $"from {TN("UserLogins")} " +
                               $"where {CN("UserId")} = {GID("UserId")} and {CN("LoginProvider")} = @LoginProvider AND {CN("ProviderKey")} = @ProviderKey;";
            var userLogin = await DbConnection.QuerySingleOrDefaultAsync<TUserLogin>(sql, new
            {
                UserId = userId,
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            });
            return userLogin;
        }
    }
}