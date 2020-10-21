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
    ///     The default implementation of <see cref="IUserRolesRecord{TRole,TKey,TUserRole}" />.
    /// </summary>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
    public class UserRolesRecord<TRole, TKey, TUserRole> :
        IdentityRecord,
        IUserRolesRecord<TRole, TKey, TUserRole>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserRole : IdentityUserRole<TKey>, new()
    {
        /// <summary>
        ///     Creates a new instance of <see cref="UserRolesRecord{TRole, TKey, TUserRole}" />.
        /// </summary>
        /// <param name="dbConnectionFactory">A factory for creating instances of <see cref="IDbConnection" />.</param>
        public UserRolesRecord(IDbConnectionFactory dbConnectionFactory) : base(dbConnectionFactory)
        {
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TRole>> GetRolesAsync(TKey userId)
        {
            var sql = "select r.* " +
                               $"from {TN("Roles")} r " +
                               $"inner join {TN("UserRoles")} ur ON ur.{CN("RoleId")} = r.{CN("Id")} " +
                               $"where ur.{CN("UserId")} = {GID("UserId")};";
            var userRoles = await DbConnection.QueryAsync<TRole>(sql, new { UserId = userId });
            return userRoles;
        }

        /// <inheritdoc />
        public virtual async Task<TUserRole> FindUserRoleAsync(TKey userId, TKey roleId)
        {
            var sql = "select * " +
                               $"from {TN("UserRoles")} " +
                               $"where {CN("UserId")} = {GID("UserId")} AND {CN("RoleId")} = {GID("RoleId")};";
            var userRole = await DbConnection.QuerySingleOrDefaultAsync<TUserRole>(sql, new
            {
                UserId = userId,
                RoleId = roleId
            });
            return userRole;
        }
    }
}