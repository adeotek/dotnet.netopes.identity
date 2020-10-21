using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Netopes.Core.Helpers.Database;
using Netopes.Identity.Data;

namespace Netopes.Identity
{
    public class AppUsersRecord : UsersRecord<AppIdentityUser, Guid, IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityUserToken<Guid>>
    {
        private readonly ILogger<AppUsersRecord> _logger;
        private readonly string _selectSql;

        public AppUsersRecord(IDbConnectionFactory dbConnectionFactory, ILogger<AppUsersRecord> logger) : base(dbConnectionFactory)
        {
            _logger = logger;
            _selectSql = $"select u.*, " +
                    $"e.{CN("CountryId")}, " +
                    $"e.{CN("CompanyName")}, " +
                    $"e.{CN("TaxCode")}, " +
                    $"e.{CN("City")}, " +
                    $"e.{CN("StreetAddress")}, " +
                    $"e.{CN("AddressDetails")}, " +
                    $"e.{CN("PostalCode")}, " +
                    $"e.{CN("PhoneNumber")}, " +
                    $"e.{CN("State")} as EntityState " +
                    $"from {TN("Users")} u " +
                    $"inner join {TN("Entities")} e on u.{CN("EntityId")} = e.{CN("Id")} ";
        }

        public override async Task<bool> CreateAsync(AppIdentityUser user)
        {
            var createEntitySql = $"insert into {TN("Entities")} (" +
                $"{CN("Id")}, " +
                $"{CN("CountryId")}, " +
                $"{CN("Email")}," +
                $"{CN("CompanyName")}, " +
                $"{CN("TaxCode")}, " +
                $"{CN("Region")}, " +
                $"{CN("City")}," +
                $"{CN("StreetAddress")}, " +
                $"{CN("AddressDetails")}, " +
                $"{CN("PostalCode")}, " +
                $"{CN("PhoneNumber")}, " +
                $"{CN("State")}) " +
                $"values ({GID("Id")}, {GID("CountryId")}, " +
                "@Email, @CompanyName, @TaxCode, @Region, @City, @StreetAddress, @AddressDetails, @PostalCode, @PhoneNumber, @State);";
            var createEntityParams = new
            {
                Id = user.EntityId.ToString(),
                CountryId = user.CountryId.ToString(),
                user.Email,
                user.CompanyName,
                user.TaxCode,
                user.Region,
                user.City,
                user.StreetAddress,
                user.AddressDetails,
                user.PostalCode,
                user.PhoneNumber,
                user.State
            };

            var createUserSql = $"insert into {TN("Users")} ( " +
                $"{CN("Id")}, " +
                $"{CN("EntityId")}, " +
                $"{CN("Email")}, " +
                $"{CN("FirstName")}, " +
                $"{CN("LastName")}, " +
                $"{CN("Username")}, " +
                $"{CN("NormalizedUsername")}, " +
                $"{CN("PasswordHash")}, " +
                $"{CN("CultureInfo")}, " +
                $"{CN("State")}, " +
                $"{CN("EmailConfirmed")}, " +
                $"{CN("SecurityStamp")}, " +
                $"{CN("ConcurrencyStamp")}, " +
                $"{CN("PhoneNumber")}, " +
                $"{CN("PhoneNumberConfirmed")}, " +
                $"{CN("TwoFactorEnabled")}, " +
                $"{CN("LockoutEnd")}, " +
                $"{CN("LockoutEnabled")}, " +
                $"{CN("AccessFailedCount")}) " +
                $"select first 1 {GID("Id")}, {GID("EntityId")}, @Email, @FirstName, @LastName, @UserName, @NormalizedUserName, @PasswordHash, " +
                $"coalesce(@CultureInfo,(case when c.{CN("Code2")} = 'RO' then 'ro' else 'en' end)), " +
                "@State, @EmailConfirmed, @SecurityStamp, @ConcurrencyStamp, @PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled, @LockoutEnd, @LockoutEnabled, @AccessFailedCount " +
                $"from {TN("Countries")} c where c.{CN("Id")} = {GID("CountryId")};";
            var createUserParams = new
            {
                Id = user.Id.ToString(),
                EntityId = user.EntityId.ToString(),
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
                LockoutEnd = user.LockoutEndForDb,
                user.LockoutEnabled,
                user.AccessFailedCount,
                CountryId = user.CountryId.ToString()
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

        /// <inheritdoc />
        public override async Task<AppIdentityUser> FindByIdAsync(Guid userId)
        {
            var sql = _selectSql + 
                $"where u.{CN("Id")} = {GID("Id")};";
            var user = await DbConnection.QuerySingleOrDefaultAsync<AppIdentityUser>(sql, new { Id = userId.ToString() });
            return user;
        }

        /// <inheritdoc />
        public override async Task<AppIdentityUser> FindByNameAsync(string normalizedUserName)
        {
            var sql = _selectSql + 
                $"where u.{CN("NormalizedUsername")} = @NormalizedUserName;";
            var user = await DbConnection.QuerySingleOrDefaultAsync<AppIdentityUser>(sql,
                new { NormalizedUserName = normalizedUserName });
            return user;
        }

        /// <inheritdoc />
        public override async Task<AppIdentityUser> FindByEmailAsync(string normalizedEmail)
        {
            var sql = _selectSql +
                $"where u.{CN("Email")} = @NormalizedEmail;";
            var user = await DbConnection.QuerySingleOrDefaultAsync<AppIdentityUser>(sql,
                new { NormalizedEmail = normalizedEmail });
            return user;
        }

        /// <inheritdoc />
        public override async Task<bool> UpdateAsync(AppIdentityUser user, IList<IdentityUserClaim<Guid>> claims, IList<IdentityUserRole<Guid>> roles, IList<IdentityUserLogin<Guid>> logins, IList<IdentityUserToken<Guid>> tokens)
        {
            var updateUserSql =
                $"update {TN("Users")} set " +
                $"{CN("Email")} = @Email, " +
                $"{CN("FirstName")} = @FirstName, " +
                $"{CN("LastName")} = @LastName, " +
                $"{CN("UserName")} = @UserName, " +
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
                Id = user.Id.ToString()
            }, transaction);

            if (claims?.Count > 0)
            {
                var deleteClaimsSql = "delete " +
                                               $"from {TN("UserClaims")} " +
                                               $"where {CN("UserId")} = {GID("UserId")};";
                await DbConnection.ExecuteAsync(deleteClaimsSql, new { UserId = user.Id }, transaction);
                var insertClaimsSql =
                    $"insert into {TN("UserClaims")} ({CN("UserId")}, {CN("ClaimType")}, {CN("ClaimValue")}) " +
                    $"values ({GID("UserId")}, @ClaimType, @ClaimValue);";
                await DbConnection.ExecuteAsync(insertClaimsSql, claims.Select(x => new
                {
                    UserId = user.Id.ToString(),
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
                    UserId = user.Id.ToString(),
                    RoleId = x.RoleId.ToString()
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
                    UserId = user.Id.ToString()
                }), transaction);
            }

            if (tokens?.Count > 0)
            {
                var deleteTokensSql = "delete " +
                                               $"from {TN("UserTokens")} " +
                                               $"where {CN("UserId")} = {GID("UserId")};";
                await DbConnection.ExecuteAsync(deleteTokensSql, new { UserId = user.Id.ToString() }, transaction);
                var insertTokensSql =
                    $"insert into {TN("UserTokens")} ({CN("UserId")}, {CN("LoginProvider")}, {CN("Name")}, {CN("Value")}) " +
                    $"values ({GID("UserId")}, @LoginProvider, @Name, @Value);";
                await DbConnection.ExecuteAsync(insertTokensSql, tokens.Select(x => new
                {
                    UserId = x.UserId.ToString(),
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
        public override async Task<IEnumerable<AppIdentityUser>> GetUsersInRoleAsync(string roleName)
        {
            var sql = _selectSql +
                $"inner join {TN("UserRoles")} ur on u.{CN("Id")} = ur.{CN("UserId")} " +
                $"inner join {TN("Roles")} r on ur.{CN("RoleId")} = r.{CN("Id")} " +
                $"where r.{CN("Name")} = @RoleName;";
            var users = await DbConnection.QueryAsync<AppIdentityUser>(sql, new { RoleName = roleName });
            return users;
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<AppIdentityUser>> GetUsersForClaimAsync(Claim claim)
        {
            var sql = _selectSql +
                $"inner join {TN("UserClaims")} uc on u.{CN("Id")} = uc.{CN("UserId")} " +
                $"where uc.{CN("ClaimType")} = @ClaimType and uc.{CN("ClaimValue")} = @ClaimValue;";
            var users = await DbConnection.QueryAsync<AppIdentityUser>(sql, new
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            });
            return users;
        }
    }
}
