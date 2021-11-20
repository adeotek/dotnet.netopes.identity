using Microsoft.AspNetCore.Identity;
using Netopes.Core.Helpers.Extensions;
using Netopes.Identity.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Netopes.Identity.Stores
{
    /// <summary>
    ///     The persistence store for roles.
    /// </summary>
    /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
    public class RoleStore<TRole> : RoleStore<TRole, string>
        where TRole : IdentityRole<string>
    {
        /// <summary>
        ///     Constructs a new instance of <see cref="RoleStore{TRole}" />.
        /// </summary>
        /// <param name="rolesRecord">Abstraction for interacting with Roles table.</param>
        /// <param name="roleClaimsRecord">Abstraction for interacting with RoleClaims table.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber" />.</param>
        public RoleStore(IRolesRecord<TRole, string, IdentityRoleClaim<string>> rolesRecord,
            IRoleClaimsRecord<string, IdentityRoleClaim<string>> roleClaimsRecord,
            IdentityErrorDescriber describer = null)
            : base(rolesRecord, roleClaimsRecord, describer)
        {
        }
    }

    /// <summary>
    ///     The persistence store for roles.
    /// </summary>
    /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
    /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
    public class RoleStore<TRole, TKey> :
        RoleStore<TRole, TKey, IdentityUserRole<TKey>, IdentityRoleClaim<TKey>>,
        IRoleClaimStore<TRole>
        where TKey : IEquatable<TKey>
        where TRole : IdentityRole<TKey>
    {
        /// <summary>
        ///     Constructs a new instance of <see cref="RoleStore{TKey, TRole}" />.
        /// </summary>
        /// <param name="rolesRecord">Abstraction for interacting with Roles table.</param>
        /// <param name="roleClaimsRecord">Abstraction for interacting with RoleClaims table.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber" />.</param>
        public RoleStore(IRolesRecord<TRole, TKey, IdentityRoleClaim<TKey>> rolesRecord,
            IRoleClaimsRecord<TKey, IdentityRoleClaim<TKey>> roleClaimsRecord, IdentityErrorDescriber describer = null)
            : base(rolesRecord, roleClaimsRecord, describer)
        {
        }
    }

    /// <summary>
    ///     The persistence store for roles.
    /// </summary>
    /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
    /// <typeparam name="TUserRole">The type of the class representing a user role.</typeparam>
    /// <typeparam name="TRoleClaim">The type of the class representing a role claim.</typeparam>
    public class RoleStore<TRole, TKey, TUserRole, TRoleClaim> : RoleStoreBase<TRole, TKey, TUserRole, TRoleClaim>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserRole : IdentityUserRole<TKey>, new()
        where TRoleClaim : IdentityRoleClaim<TKey>, new()
    {
        /// <summary>
        ///     Constructs a new instance of <see cref="RoleStore{TKey, TRole, TUserRole, TRoleClaim}" />.
        /// </summary>
        /// <param name="rolesRecord">Abstraction for interacting with Roles table.</param>
        /// <param name="roleClaimsRecord">Abstraction for interacting with RoleClaims table.</param>
        /// <param name="describer">The <see cref="IdentityErrorDescriber" />.</param>
        public RoleStore(IRolesRecord<TRole, TKey, TRoleClaim> rolesRecord,
            IRoleClaimsRecord<TKey, TRoleClaim> roleClaimsRecord, IdentityErrorDescriber describer) : base(describer)
        {
            RolesRecord = rolesRecord ?? throw new ArgumentNullException(nameof(rolesRecord));
            RoleClaimsRecord = roleClaimsRecord ?? throw new ArgumentNullException(nameof(roleClaimsRecord));
            ErrorDescriber = describer ?? new IdentityErrorDescriber();
        }

        /// <inheritdoc />
        public override IQueryable<TRole> Roles => throw new NotSupportedException();

        /// <summary>
        ///     Abstraction for interacting with Roles table.
        /// </summary>
        private IRolesRecord<TRole, TKey, TRoleClaim> RolesRecord { get; }

        /// <summary>
        ///     Abstraction for interacting with RoleClaims table.
        /// </summary>
        private IRoleClaimsRecord<TKey, TRoleClaim> RoleClaimsRecord { get; }

        /// <summary>
        ///     Internally keeps the claims of a role.
        /// </summary>
        private IList<TRoleClaim> RoleClaims { get; set; }

        /// <inheritdoc />
        public override async Task AddClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            claim.ThrowIfNull(nameof(claim));
            RoleClaims ??= (await RoleClaimsRecord.GetClaimsAsync(role.Id)).ToList();
            RoleClaims.Add(CreateRoleClaim(role, claim));
        }

        /// <inheritdoc />
        public override async Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            var created = await RolesRecord.CreateAsync(role);
            return created
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError
                {
                    Code = string.Empty,
                    Description = $"Role '{role.Name}' could not be created."
                });
        }

        /// <inheritdoc />
        public override async Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            var deleted = await RolesRecord.DeleteAsync(role.Id);
            return deleted
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError
                {
                    Code = string.Empty,
                    Description = $"Role '{role.Name}' could not be deleted."
                });
        }

        /// <inheritdoc />
        public override async Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var id = ConvertIdFromString(roleId);
            var role = await RolesRecord.FindByIdAsync(id);
            return role;
        }

        /// <inheritdoc />
        public override async Task<TRole> FindByNameAsync(string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var role = await RolesRecord.FindByNameAsync(normalizedName);
            return role;
        }

        /// <inheritdoc />
        public override async Task<IList<Claim>> GetClaimsAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            var roleClaims = await RoleClaimsRecord.GetClaimsAsync(role.Id);
            return roleClaims.Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();
        }

        /// <inheritdoc />
        public override Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            return Task.FromResult(role.NormalizedName);
        }

        /// <inheritdoc />
        public override Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            return Task.FromResult(ConvertIdToString(role.Id));
        }

        /// <inheritdoc />
        public override Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            return Task.FromResult(role.Name);
        }

        /// <inheritdoc />
        public override async Task RemoveClaimAsync(TRole role, Claim claim, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            claim.ThrowIfNull(nameof(role));
            RoleClaims ??= (await RoleClaimsRecord.GetClaimsAsync(role.Id)).ToList();
            var roleClaims = RoleClaims.Where(x =>
                x.RoleId.Equals(role.Id) && x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
            foreach (var roleClaim in RoleClaims) RoleClaims.Remove(roleClaim);
        }

        /// <inheritdoc />
        public override Task SetNormalizedRoleNameAsync(TRole role, string normalizedName,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            role.Name = roleName;
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override async Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            role.ThrowIfNull(nameof(role));
            role.ConcurrencyStamp = Guid.NewGuid().ToString();
            var updated = await RolesRecord.UpdateAsync(role, RoleClaims);
            return updated
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError
                {
                    Code = string.Empty,
                    Description = $"Role '{role.Name}' could not be updated."
                });
        }
    }
}