using Dapper;
using Microsoft.AspNetCore.Identity;
using Netopes.Identity.Abstract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Netopes.Core.Helpers.Database;

namespace Netopes.Identity.Data
{
    /// <summary>
    ///     The default implementation of <see cref="IRolesRecord{TRole,TKey,TRoleClaim}" />.
    /// </summary>
    /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
    /// <typeparam name="TRoleClaim">The type of the class representing a role claim.</typeparam>
    public class RolesRecord<TRole, TKey, TRoleClaim> :
        IdentityRecord,
        IRolesRecord<TRole, TKey, TRoleClaim>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        /// <summary>
        ///     Creates a new instance of <see cref="RolesRecord{TRole, TKey, TRoleClaim}" />.
        /// </summary>
        /// <param name="dbConnectionFactory">A factory for creating instances of <see cref="IDbConnection" />.</param>
        public RolesRecord(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }

        /// <inheritdoc />
        public virtual async Task<bool> CreateAsync(TRole role)
        {
            var sql = $"insert into {TN("Roles")} ({CN("Id")}, {CN("Name")}, {CN("NormalizedName")}, {CN("ConcurrencyStamp")}) " +
                               $"values ({GID("Id")}, @Name, @NormalizedName, @ConcurrencyStamp);";
            var rowsInserted = await DbConnection.ExecuteAsync(sql, new
            {
                role.Id,
                role.Name,
                role.NormalizedName,
                role.ConcurrencyStamp
            });
            return rowsInserted == 1;
        }

        /// <inheritdoc />
        public virtual async Task<bool> DeleteAsync(TKey roleId)
        {
            var sql = "delete " +
                               $"from {TN("Roles")} " +
                               $"where {CN("Id")} = {GID("Id")};";
            var rowsDeleted = await DbConnection.ExecuteAsync(sql, new { Id = roleId });
            return rowsDeleted == 1;
        }

        /// <inheritdoc />
        public virtual async Task<TRole> FindByIdAsync(TKey roleId)
        {
            var sql = "select * " +
                               $"from {TN("Roles")} " +
                               $"where {CN("Id")} = {GID("Id")};";
            var role = await DbConnection.QuerySingleOrDefaultAsync<TRole>(sql, new { Id = roleId });
            return role;
        }

        /// <inheritdoc />
        public virtual async Task<TRole> FindByNameAsync(string normalizedName)
        {
            var sql = "select * " +
                               $"from {TN("Roles")} " +
                               $"where {CN("NormalizedName")} = @NormalizedName;";
            var role = await DbConnection.QuerySingleOrDefaultAsync<TRole>(sql, new { NormalizedName = normalizedName });
            return role;
        }

        /// <inheritdoc />
        public virtual async Task<bool> UpdateAsync(TRole role, IList<TRoleClaim> claims = null)
        {
            var updateRoleSql = $"update {TN("Roles")} " +
                                         $"set {CN("Name")} = @Name, " +
                                         $"{CN("NormalizedName")} = @NormalizedName, " +
                                         $"{CN("ConcurrencyStamp")} = @ConcurrencyStamp " +
                                         $"where {CN("Id")} = {GID("Id")};";
            using (var transaction = DbConnection.BeginTransaction())
            {
                await DbConnection.ExecuteAsync(updateRoleSql, new
                {
                    role.Name,
                    role.NormalizedName,
                    role.ConcurrencyStamp,
                    role.Id
                }, transaction);
                if (claims?.Count() > 0)
                {
                    var deleteClaimsSql = "delete " +
                                                   $"from {TN("RoleClaims")} " +
                                                   $"where {CN("RoleId")} = {GID("RoleId")};";
                    await DbConnection.ExecuteAsync(deleteClaimsSql, new
                    {
                        RoleId = role.Id
                    }, transaction);
                    var insertClaimsSql =
                        $"insert into {TN("RoleClaims")} ({CN("RoleId")}, {CN("ClaimType")}, {CN("ClaimValue")}) " +
                        $"values ({GID("RoleId")}, @ClaimType, @ClaimValue);";
                    await DbConnection.ExecuteAsync(insertClaimsSql, claims.Select(x => new
                    {
                        RoleId = role.Id,
                        x.ClaimType,
                        x.ClaimValue
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
            }

            return true;
        }
    }
}