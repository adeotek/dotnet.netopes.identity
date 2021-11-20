using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Netopes.Core.Helpers.Database;
using Netopes.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Netopes.Identity
{
    public class AppUsersWithAccountRecord : UsersRecord<AppIdentityUserWithAccount, Guid, IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityUserToken<Guid>>
    {
        private readonly ILogger<AppUsersWithAccountRecord> _logger;

        public AppUsersWithAccountRecord(IDbConnectionFactory dbConnectionFactory, ILogger<AppUsersWithAccountRecord> logger) : base(dbConnectionFactory)
        {
            _logger = logger;
        }

        public override async Task<bool> CreateAsync(AppIdentityUserWithAccount user)
        {
            var createEntitySql = $"insert into {TN("Accounts")} (" +
                $"{CN("Id")}, " +
                $"{CN("Email")}," +
                $"{CN("Region")}, " +
                $"{CN("City")}," +
                $"{CN("StreetAddress")}, " +
                $"{CN("AddressDetails")}, " +
                $"{CN("PostalCode")}, " +
                $"{CN("PhoneNumber")}, " +
                $"{CN("State")}) " +
                $"{CN("CreatedAt")}) " +
                $"values ({GID("Id")}, " +
                "@Email, @Region, @City, @StreetAddress, @AddressDetails, @PostalCode, @PhoneNumber, @State, @CreatedAt);";
            var createEntityParams = new
            {
                Id = user.AccountId.ToString(),
                user.Email,
                user.Account.Region,
                user.Account.City,
                user.Account.StreetAddress,
                user.Account.AddressDetails,
                user.Account.PostalCode,
                user.PhoneNumber,
                State = 1,
                user.CreatedAt
            };

            var createUserSql = $"insert into {TN("Users")} ( " +
                $"{CN("Id")}, " +
                $"{CN("AccountId")}, " +
                $"{CN("Email")}, " +
                $"{CN("FirstName")}, " +
                $"{CN("LastName")}, " +
                $"{CN("Username")}, " +
                $"{CN("NormalizedUsername")}, " +
                $"{CN("PasswordHash")}, " +
                $"{CN("CultureInfo")}, " +
                $"{CN("State")}, " +
                $"{CN("DebugMode")}, " +
                $"{CN("EmailConfirmed")}, " +
                $"{CN("SecurityStamp")}, " +
                $"{CN("ConcurrencyStamp")}, " +
                $"{CN("PhoneNumber")}, " +
                $"{CN("PhoneNumberConfirmed")}, " +
                $"{CN("TwoFactorEnabled")}, " +
                $"{CN("LockoutEnd")}, " +
                $"{CN("LockoutEnabled")}, " +
                $"{CN("AccessFailedCount")}) " +
                $"values ({GID("Id")}, {GID("AccountId")}, @Email, @FirstName, @LastName, @UserName, @NormalizedUserName, @PasswordHash, " +
                $"coalesce(@CultureInfo,'en'), " +
                "@State, @EmailConfirmed, @SecurityStamp, @ConcurrencyStamp, @PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled, @LockoutEnd, @LockoutEnabled, @AccessFailedCount);";
            var createUserParams = new
            {
                Id = user.Id.ToString(),
                EntityId = user.AccountId.ToString(),
                user.Email,
                user.FirstName,
                user.LastName,
                user.UserName,
                user.NormalizedUserName,
                user.PasswordHash,
                user.CultureInfo,
                user.State,
                user.DebugMode,
                user.EmailConfirmed,
                user.SecurityStamp,
                user.ConcurrencyStamp,
                user.PhoneNumber,
                user.PhoneNumberConfirmed,
                user.TwoFactorEnabled,
                LockoutEnd = user.LockoutEnd?.DateTime,
                user.LockoutEnabled,
                user.AccessFailedCount
            };

            using var transaction = DbConnection.BeginTransaction();
            try
            {
                await DbConnection.ExecuteAsync(createEntitySql, createEntityParams, transaction);
                await DbConnection.ExecuteAsync(createUserSql, createUserParams, transaction);

                transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "Unable to create new user");
                transaction.Rollback();
                return false;
            }
        }

        /// <inheritdoc />
        public override async Task<bool> DeleteAsync(Guid userId)
        {
            var sql = "delete " +
                $"from {TN("Users")} " +
                $"where {CN("Id")} = {GID("Id")};";
            var rowsDeleted = await DbConnection.ExecuteAsync(sql, new { Id = userId.ToString() });
            return rowsDeleted == 1;
        }

        public async Task<AppAccount> GetAppAccountById(Guid id)
        {
            var sql = $"select a.* from {TN("Accounts")} a where a.{CN("Id")} = {GID("Id")};";
            return await DbConnection.QuerySingleOrDefaultAsync<AppAccount>(sql, new { Id = id.ToString() });
        }

        /// <inheritdoc />
        public override async Task<AppIdentityUserWithAccount> FindByIdAsync(Guid userId)
        {
            var sql = $"select u.* from {TN("Users")} u where u.{CN("Id")} = {GID("Id")};";
            var user = await DbConnection.QuerySingleOrDefaultAsync<AppIdentityUserWithAccount>(sql, new { Id = userId.ToString() });
            user.Account = await GetAppAccountById(user.AccountId);
            return user;
        }

        /// <inheritdoc />
        public override async Task<AppIdentityUserWithAccount> FindByNameAsync(string normalizedUserName)
        {
            var sql = $"select u.* from {TN("Users")} u where u.{CN("NormalizedUsername")} = @NormalizedUserName;";
            var user = await DbConnection.QuerySingleOrDefaultAsync<AppIdentityUserWithAccount>(sql, new { NormalizedUserName = normalizedUserName });
            user.Account = await GetAppAccountById(user.AccountId);
            return user;
        }

        /// <inheritdoc />
        public override async Task<AppIdentityUserWithAccount> FindByEmailAsync(string normalizedEmail)
        {
            var sql = $"select u.* from {TN("Users")} u where u.{CN("Email")} = @NormalizedEmail;";
            var user = await DbConnection.QuerySingleOrDefaultAsync<AppIdentityUserWithAccount>(sql, new { NormalizedEmail = normalizedEmail });
            user.Account = await GetAppAccountById(user.AccountId);
            return user;
        }

        /// <inheritdoc />
        public override async Task<bool> UpdateAsync(AppIdentityUserWithAccount user, IList<IdentityUserClaim<Guid>> claims, IList<IdentityUserRole<Guid>> roles, IList<IdentityUserLogin<Guid>> logins, IList<IdentityUserToken<Guid>> tokens)
        {
            var updateUserSql =
                $"update {TN("Users")} set " +
                $"{CN("Email")} = @Email, " +
                $"{CN("FirstName")} = @FirstName, " +
                $"{CN("LastName")} = @LastName, " +
                $"{CN("Username")} = @UserName, " +
                $"{CN("NormalizedUsername")} = @NormalizedUserName, " +
                $"{CN("PasswordHash")} = @PasswordHash, " +
                $"{CN("CultureInfo")} = @CultureInfo, " +
                $"{CN("State")} = @State, " +
                $"{CN("EmailConfirmed")} = @EmailConfirmed, " +
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
                user.Email,
                user.FirstName,
                user.LastName,
                user.UserName,
                user.NormalizedUserName,
                user.PasswordHash,
                user.CultureInfo,
                user.State,
                user.EmailConfirmed,
                user.SecurityStamp,
                user.ConcurrencyStamp,
                user.PhoneNumber,
                user.PhoneNumberConfirmed,
                user.TwoFactorEnabled,
                LockoutEnd = user.LockoutEnd?.DateTime,
                user.LockoutEnabled,
                user.AccessFailedCount,
                user.Id
            }, transaction);

            if (claims?.Count > 0)
            {
                var deleteClaimsSql = "delete " +
                                               $"from {TN("UserClaims")} " +
                                               $"where {CN("UserId")} = {GID("UserId")};";
                await DbConnection.ExecuteAsync(deleteClaimsSql, new { UserId = user.Id.ToString() }, transaction);
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
                await DbConnection.ExecuteAsync(deleteRolesSql, new { UserId = user.Id }, transaction);
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
                await DbConnection.ExecuteAsync(deleteLoginsSql, new { UserId = user.Id }, transaction);
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
                await DbConnection.ExecuteAsync(deleteTokensSql, new { UserId = user.Id }, transaction);
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
        public override async Task<IEnumerable<AppIdentityUserWithAccount>> GetUsersInRoleAsync(string roleName)
        {
            var sql = $"select u.* from {TN("Users")} u " +
                $"inner join {TN("UserRoles")} ur on u.{CN("Id")} = ur.{CN("UserId")} " +
                $"inner join {TN("Roles")} r on ur.{CN("RoleId")} = r.{CN("Id")} " +
                $"where r.{CN("Name")} = @RoleName;";
            var users = await DbConnection.QueryAsync<AppIdentityUserWithAccount>(sql, new { RoleName = roleName });
            if(!users.Any())
            {
                return users;
            }

            foreach (var user in users)
            {
                user.Account = await GetAppAccountById(user.AccountId);
            }
            return users;
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<AppIdentityUserWithAccount>> GetUsersForClaimAsync(Claim claim)
        {
            var sql = $"select u.* from {TN("Users")} u " +
                $"inner join {TN("UserClaims")} uc on u.{CN("Id")} = uc.{CN("UserId")} " +
                $"where uc.{CN("ClaimType")} = @ClaimType and uc.{CN("ClaimValue")} = @ClaimValue;";
            var users = await DbConnection.QueryAsync<AppIdentityUserWithAccount>(sql, new
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            });

            if (!users.Any())
            {
                return users;
            }

            foreach (var user in users)
            {
                user.Account = await GetAppAccountById(user.AccountId);
            }
            return users;
        }
    }
}
