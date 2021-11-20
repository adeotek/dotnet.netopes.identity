using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Netopes.Identity.Abstract
{
    /// <summary>
    ///     Abstraction for interacting with AspNetUserRoles table.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
    /// <typeparam name="TRole">The type representing a role.</typeparam>
    /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
    public interface IUserRolesRecord<TRole, TKey, TUserRole>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        ///     Gets a list of roles to be belonging to the specified user.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        Task<IEnumerable<TRole>> GetRolesAsync(TKey userId);

        /// <summary>
        ///     Find the specified role for a certain user.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        /// <param name="roleId">The id of the role.</param>
        Task<TUserRole> FindUserRoleAsync(TKey userId, TKey roleId);
    }
}