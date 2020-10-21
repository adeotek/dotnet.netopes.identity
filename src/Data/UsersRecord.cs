using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Netopes.Identity.Abstract;
using Microsoft.AspNetCore.Identity;
using Netopes.Core.Helpers.Database;

namespace Netopes.Identity.Data
{
    /// <summary>
    ///     The default implementation of <see cref="IUsersRecord{TUser,TKey,TUserClaim,TUserRole,TUserLogin,TUserToken}" />.
    /// </summary>
    /// <typeparam name="TUser">The type representing a user.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a role and user.</typeparam>
    /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
    /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
    /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
    /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
    public class UsersRecord<TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken> :
        IdentityRecord,
        IUsersRecord<TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken>
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserToken : IdentityUserToken<TKey>, new()
    {
        /// <summary>
        ///     Creates a new instance of <see cref="UsersRecord{TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken}" />.
        /// </summary>
        /// <param name="dbConnectionFactory">A factory for creating instances of <see cref="IDbConnection" />.</param>
        public UsersRecord(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }

        /// <inheritdoc />
        public virtual async Task<bool> CreateAsync(TUser user)
        {
            var sql = $"insert into {TN("Users")} values " +
                               $"({GID("Id")}, @UserName, @NormalizedUserName, @Email, @NormalizedEmail, @EmailConfirmed, @PasswordHash, @SecurityStamp, @ConcurrencyStamp, " +
                               "@PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled, @LockoutEnd, @LockoutEnabled, @AccessFailedCount);";
            var rowsInserted = await DbConnection.ExecuteAsync(sql, new
            {
                user.Id,
                user.UserName,
                user.NormalizedUserName,
                user.Email,
                user.NormalizedEmail,
                user.EmailConfirmed,
                user.PasswordHash,
                user.SecurityStamp,
                user.ConcurrencyStamp,
                user.PhoneNumber,
                user.PhoneNumberConfirmed,
                user.TwoFactorEnabled,
                user.LockoutEnd,
                user.LockoutEnabled,
                user.AccessFailedCount
            });
            return rowsInserted == 1;
        }

        /// <inheritdoc />
        public virtual async Task<bool> DeleteAsync(TKey userId)
        {
            var sql = "delete " +
                $"from {TN("Users")} " +
                $"where {CN("Id")} = {GID("Id")};";
            var rowsDeleted = await DbConnection.ExecuteAsync(sql, new {Id = userId});
            return rowsDeleted == 1;
        }

        /// <inheritdoc />
        public virtual async Task<TUser> FindByIdAsync(TKey userId)
        {
            var sql = "select * " +
                $"from {TN("Users")} " +
                $"where {CN("Id")} = {GID("Id")};";
            var user = await DbConnection.QuerySingleOrDefaultAsync<TUser>(sql, new {Id = userId});
            return user;
        }

        /// <inheritdoc />
        public virtual async Task<TUser> FindByNameAsync(string normalizedUserName)
        {
            var sql = "select * " +
                $"from {TN("Users")} " +
                $"where {CN("NormalizedUserName")} = @NormalizedUserName;";
            var user = await DbConnection.QuerySingleOrDefaultAsync<TUser>(sql,
                new {NormalizedUserName = normalizedUserName});
            return user;
        }

        /// <inheritdoc />
        public virtual async Task<TUser> FindByEmailAsync(string normalizedEmail)
        {
            var command = "select * " +
                                   $"from {TN("Users")} " +
                                   $"where {CN("NormalizedEmail")} = @NormalizedEmail;";
            var user = await DbConnection.QuerySingleOrDefaultAsync<TUser>(command,
                new {NormalizedEmail = normalizedEmail});
            return user;
        }

        /// <inheritdoc />
        public virtual Task<bool> UpdateAsync(TUser user, IList<TUserClaim> claims, IList<TUserLogin> logins,
            IList<TUserToken> tokens)
        {
            return UpdateAsync(user, claims, null, logins, tokens);
        }

        /// <inheritdoc />
        public virtual async Task<bool> UpdateAsync(TUser user, IList<TUserClaim> claims, IList<TUserRole> roles,
            IList<TUserLogin> logins, IList<TUserToken> tokens)
        {
            var updateUserSql =
                $"update {TN("Users")} set " +
                $"{CN("UserName")} = @UserName, " +
                $"{CN("NormalizedUserName")} = @NormalizedUserName, " +
                $"{CN("Email")} = @Email, " +
                $"{CN("NormalizedEmail")} = @NormalizedEmail, " +
                $"{CN("EmailConfirmed")} = @EmailConfirmed, " +
                $"{CN("PasswordHash")} = @PasswordHash, " +
                $"{CN("SecurityStamp")} = @SecurityStamp, " +
                $"{CN("ConcurrencyStamp")} = @ConcurrencyStamp, " +
                $"{CN("PhoneNumber")} = @PhoneNumber, " +
                $"{CN("PhoneNumberConfirmed")} = @PhoneNumberConfirmed, " +
                $"{CN("TwoFactorEnabled")} = @TwoFactorEnabled, " +
                $"{CN("LockoutEnd")} = @LockoutEnd, " +
                $"{CN("LockoutEnabled")} = @LockoutEnabled, " +
                $"{CN("AccessFailedCount")} = @AccessFailedCount " +
                $"where {CN("Id")} = {GID("Id")};";

            using var transaction = DbConnection.BeginTransaction();
            await DbConnection.ExecuteAsync(updateUserSql, new
            {
                user.UserName,
                user.NormalizedUserName,
                user.Email,
                user.NormalizedEmail,
                user.EmailConfirmed,
                user.PasswordHash,
                user.SecurityStamp,
                user.ConcurrencyStamp,
                user.PhoneNumber,
                user.PhoneNumberConfirmed,
                user.TwoFactorEnabled,
                user.LockoutEnd,
                user.LockoutEnabled,
                user.AccessFailedCount,
                user.Id
            }, transaction);

            if (claims?.Count > 0)
            {
                var deleteClaimsSql = "delete " +
                                               $"from {TN("UserClaims")} " +
                                               $"where {CN("UserId")} = {GID("UserId")};";
                await DbConnection.ExecuteAsync(deleteClaimsSql, new {UserId = user.Id}, transaction);
                var insertClaimsSql =
                    $"insert into {TN("UserClaims")} ({CN("UserId")}, {CN("ClaimType")}, {CN("ClaimValue")}) " +
                    $"values ({GID("UserId")}, @ClaimType, @ClaimValue);";
                await DbConnection.ExecuteAsync(insertClaimsSql, claims.Select(x => new
                {
                    UserId = user.Id,
                    x.ClaimType,
                    x.ClaimValue
                }), transaction);
            }

            if (roles?.Count > 0)
            {
                var deleteRolesSql = "delete " +
                                              $"from {TN("UserRoles")} " +
                                              $"where {CN("UserId")} = {GID("UserId")};";
                await DbConnection.ExecuteAsync(deleteRolesSql, new {UserId = user.Id}, transaction);
                var insertRolesSql = $"insert into {TN("UserRoles")} ({CN("UserId")}, {CN("RoleId")}) " +
                                              $"values ({GID("UserId")}, {GID("RoleId")});";
                await DbConnection.ExecuteAsync(insertRolesSql, roles.Select(x => new
                {
                    UserId = user.Id,
                    x.RoleId
                }), transaction);
            }

            if (logins?.Count > 0)
            {
                var deleteLoginsSql = "delete " +
                                               $"from {TN("UserLogins")} " +
                                               $"where {CN("UserId")} = {GID("UserId")};";
                await DbConnection.ExecuteAsync(deleteLoginsSql, new {UserId = user.Id}, transaction);
                var insertLoginsSql =
                    $"insert into {TN("UserLogins")} ({CN("LoginProvider")}, {CN("ProviderKey")}, {CN("ProviderDisplayName")}, {CN("UserId")}) " +
                    $"values (@LoginProvider, @ProviderKey, @ProviderDisplayName, {GID("UserId")});";
                await DbConnection.ExecuteAsync(insertLoginsSql, logins.Select(x => new
                {
                    x.LoginProvider,
                    x.ProviderKey,
                    x.ProviderDisplayName,
                    UserId = user.Id
                }), transaction);
            }

            if (tokens?.Count > 0)
            {
                var deleteTokensSql = "delete " +
                                               $"from {TN("UserTokens")} " +
                                               $"where {CN("UserId")} = {GID("UserId")};";
                await DbConnection.ExecuteAsync(deleteTokensSql, new {UserId = user.Id}, transaction);
                var insertTokensSql =
                    $"insert into {TN("UserTokens")} ({CN("UserId")}, {CN("LoginProvider")}, {CN("Name")}, {CN("Value")}) " +
                    $"values ({GID("UserId")}, @LoginProvider, @Name, @Value);";
                await DbConnection.ExecuteAsync(insertTokensSql, tokens.Select(x => new
                {
                    x.UserId,
                    x.LoginProvider,
                    x.Name,
                    x.Value
                }), transaction);
            }

            try
            {
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TUser>> GetUsersInRoleAsync(string roleName)
        {
            var sql = "select * " +
                               $"from {TN("Users")} u " +
                               $"inner join {TN("UserRoles")} ur on u.{CN("Id")} = ur.{CN("UserId")} " +
                               $"inner join {TN("Roles")} r on ur.{CN("RoleId")} = r.{CN("Id")} " +
                               $"where r.{CN("Name")} = @RoleName;";
            var users = await DbConnection.QueryAsync<TUser>(sql, new {RoleName = roleName});
            return users;
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TUser>> GetUsersForClaimAsync(Claim claim)
        {
            var sql = "select * " +
                               $"from {TN("Users")} u " +
                               $"inner join {TN("UserClaims")} uc on u.{CN("Id")} = uc.{CN("UserId")} " +
                               $"where uc.{CN("ClaimType")} = @ClaimType AND uc.{CN("ClaimValue")} = @ClaimValue;";
            var users = await DbConnection.QueryAsync<TUser>(sql, new
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            });
            return users;
        }
    }
}