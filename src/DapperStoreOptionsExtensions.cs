using System;
using Netopes.Identity;
using Netopes.Identity.Abstract;
using Netopes.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Extensions on <see cref="DapperStoreOptions" /> type.
    /// </summary>
    public static class DapperStoreOptionsExtensions
    {
        /// <summary>
        ///     Add a custom implementation for <see cref="RoleClaimsRecord{TKey,TRoleClaim}" />.
        /// </summary>
        /// <typeparam name="TRoleClaimsRecord">The type of the table to register.</typeparam>
        /// <typeparam name="TRoleClaim">The type of the class representing a role claim.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddRoleClaimsRecord<TRoleClaimsRecord, TRoleClaim>(this DapperStoreOptions options)
            where TRoleClaimsRecord : RoleClaimsRecord<string, TRoleClaim>
            where TRoleClaim : IdentityRoleClaim<string>, new()
        {
            options.AddRoleClaimsRecord<TRoleClaimsRecord, string, TRoleClaim>();
        }

        /// <summary>
        ///     Add a custom implementation for <see cref="RoleClaimsRecord{TKey, TRoleClaim}" />.
        /// </summary>
        /// <typeparam name="TRoleClaimsRecord">The type of the table to register.</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
        /// <typeparam name="TRoleClaim">The type of the class representing a role claim.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddRoleClaimsRecord<TRoleClaimsRecord, TKey, TRoleClaim>(this DapperStoreOptions options)
            where TRoleClaimsRecord : RoleClaimsRecord<TKey, TRoleClaim>
            where TKey : IEquatable<TKey>
            where TRoleClaim : IdentityRoleClaim<TKey>, new()
        {
            options.Services.AddScoped(typeof(IRoleClaimsRecord<,>).MakeGenericType(typeof(TKey), typeof(TRoleClaim)),
                typeof(TRoleClaimsRecord));
        }

        /// <summary>
        ///     Add a custom implementation for <see cref="RolesRecord{TRole, TKey, TRoleClaim}" />.
        /// </summary>
        /// <typeparam name="TRolesRecord">The type of the table to register.</typeparam>
        /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddRolesRecord<TRolesRecord, TRole>(this DapperStoreOptions options)
            where TRolesRecord : RolesRecord<TRole, string, IdentityRoleClaim<string>>
            where TRole : IdentityRole<string>
        {
            options.AddRolesRecord<TRolesRecord, TRole, string, IdentityRoleClaim<string>>();
        }

        /// <summary>
        ///     Add a custom implementation for <see cref="RolesRecord{TRole, TKey, TRoleClaim}" />.
        /// </summary>
        /// <typeparam name="TRolesRecord">The type of the table to register.</typeparam>
        /// <typeparam name="TRole">The type of the class representing a role.</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a role.</typeparam>
        /// <typeparam name="TRoleClaim">The type of the class representing a role claim.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddRolesRecord<TRolesRecord, TRole, TKey, TRoleClaim>(this DapperStoreOptions options)
            where TRolesRecord : RolesRecord<TRole, TKey, TRoleClaim>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
            where TRoleClaim : IdentityRoleClaim<TKey>, new()
        {
            options.Services.AddScoped(
                typeof(IRolesRecord<,,>).MakeGenericType(typeof(TRole), typeof(TKey), typeof(TRoleClaim)),
                typeof(TRolesRecord));
        }

        /// <summary>
        ///     Add a custom implementation for <see cref="UserClaimsRecord{TKey, TUserClaim}" />.
        /// </summary>
        /// <typeparam name="TUserClaimsRecord">The type of the table to register.</typeparam>
        /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUserClaimsRecord<TUserClaimsRecord, TUserClaim>(this DapperStoreOptions options)
            where TUserClaimsRecord : UserClaimsRecord<string, TUserClaim>
            where TUserClaim : IdentityUserClaim<string>, new()
        {
            options.AddUserClaimsRecord<TUserClaimsRecord, string, TUserClaim>();
        }

        /// <summary>
        ///     Add a custom implementation for <see cref="UserClaimsRecord{TKey, TUserClaim}" />.
        /// </summary>
        /// <typeparam name="TUserClaimsRecord">The type of the table to register.</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
        /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUserClaimsRecord<TUserClaimsRecord, TKey, TUserClaim>(this DapperStoreOptions options)
            where TUserClaimsRecord : UserClaimsRecord<TKey, TUserClaim>
            where TKey : IEquatable<TKey>
            where TUserClaim : IdentityUserClaim<TKey>, new()
        {
            options.Services.AddScoped(typeof(IUserClaimsRecord<,>).MakeGenericType(typeof(TKey), typeof(TUserClaim)),
                typeof(TUserClaimsRecord));
        }

        /// <summary>
        ///     Add a custom implementation for <see cref="UserLoginsRecord{TUser, TKey, TUserLogin}" />.
        /// </summary>
        /// <typeparam name="TUserLoginsRecord">The type of the table to register.</typeparam>
        /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUserLoginsRecord<TUserLoginsRecord, TUserLogin>(this DapperStoreOptions options)
            where TUserLoginsRecord : UserLoginsRecord<IdentityUser, string, TUserLogin>
            where TUserLogin : IdentityUserLogin<string>, new()
        {
            options.AddUserLoginsRecord<TUserLoginsRecord, IdentityUser, string, TUserLogin>();
        }

        /// <summary>
        ///     Add a custom implementation for <see cref="UserLoginsRecord{TUser, TKey, TUserLogin}" />.
        /// </summary>
        /// <typeparam name="TUserLoginsRecord">The type of the table to register.</typeparam>
        /// <typeparam name="TUser">The type representing a user.</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
        /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUserLoginsRecord<TUserLoginsRecord, TUser, TKey, TUserLogin>(
            this DapperStoreOptions options)
            where TUserLoginsRecord : UserLoginsRecord<TUser, TKey, TUserLogin>
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
            where TUserLogin : IdentityUserLogin<TKey>, new()
        {
            options.Services.AddScoped(
                typeof(IUserLoginsRecord<,,>).MakeGenericType(typeof(TUser), typeof(TKey), typeof(TUserLogin)),
                typeof(TUserLoginsRecord));
        }

        /// <summary>
        ///     Add a custom implementation for <see cref="UserRolesRecord{TRole, TKey, TUserRole}" />.
        /// </summary>
        /// <typeparam name="TUserRolesRecord">The type of the table to register.</typeparam>
        /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUserRolesRecord<TUserRolesRecord, TUserRole>(this DapperStoreOptions options)
            where TUserRolesRecord : UserRolesRecord<IdentityRole, string, TUserRole>
            where TUserRole : IdentityUserRole<string>, new()
        {
            options.AddUserRolesRecord<TUserRolesRecord, IdentityRole, string, TUserRole>();
        }

        /// <summary>
        ///     Add a custom implementation for <see cref="UserRolesRecord{TRole, TKey, TUserRole}" />.
        /// </summary>
        /// <typeparam name="TUserRolesRecord">The type of the table to register.</typeparam>
        /// <typeparam name="TRole">The type representing a role.</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a user.</typeparam>
        /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUserRolesRecord<TUserRolesRecord, TRole, TKey, TUserRole>(this DapperStoreOptions options)
            where TUserRolesRecord : UserRolesRecord<TRole, TKey, TUserRole>
            where TRole : IdentityRole<TKey>
            where TKey : IEquatable<TKey>
            where TUserRole : IdentityUserRole<TKey>, new()
        {
            options.Services.AddScoped(
                typeof(IUserRolesRecord<,,>).MakeGenericType(typeof(TRole), typeof(TKey), typeof(TUserRole)),
                typeof(TUserRolesRecord));
        }

        /// <summary>
        ///     Add a custom implementation for
        ///     <see cref="UsersRecord{TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken}" />.
        /// </summary>
        /// <typeparam name="TUsersRecord">The type of the table to register.</typeparam>
        /// <typeparam name="TUser">The type representing a user.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUsersRecord<TUsersRecord, TUser>(this DapperStoreOptions options)
            where TUsersRecord : UsersRecord<TUser, string, IdentityUserClaim<string>, IdentityUserRole<string>,
                IdentityUserLogin<string>, IdentityUserToken<string>>
            where TUser : IdentityUser<string>
        {
            options.AddUsersRecord<TUsersRecord, TUser, string>();
        }

        /// <summary>
        ///     Add a custom implementation for
        ///     <see cref="UsersRecord{TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken}" />.
        /// </summary>
        /// <typeparam name="TUsersRecord">The type of the table to register.</typeparam>
        /// <typeparam name="TUser">The type representing a user.</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a role and user.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUsersRecord<TUsersRecord, TUser, TKey>(this DapperStoreOptions options)
            where TUsersRecord : UsersRecord<TUser, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>,
                IdentityUserLogin<TKey>, IdentityUserToken<TKey>>
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
        {
            options
                .AddUsersRecord<TUsersRecord, TUser, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>,
                    IdentityUserLogin<TKey>, IdentityUserToken<TKey>>();
        }

        /// <summary>
        ///     Add a custom implementation for
        ///     <see cref="UsersRecord{TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken}" />.
        /// </summary>
        /// <typeparam name="TUsersRecord">The type of the table to register.</typeparam>
        /// <typeparam name="TUser">The type representing a user.</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a role and user.</typeparam>
        /// <typeparam name="TUserClaim">The type representing a claim.</typeparam>
        /// <typeparam name="TUserRole">The type representing a user role.</typeparam>
        /// <typeparam name="TUserLogin">The type representing a user external login.</typeparam>
        /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUsersRecord<TUsersRecord, TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken>(
            this DapperStoreOptions options)
            where TUsersRecord : UsersRecord<TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken>
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
            where TUserClaim : IdentityUserClaim<TKey>, new()
            where TUserRole : IdentityUserRole<TKey>, new()
            where TUserLogin : IdentityUserLogin<TKey>, new()
            where TUserToken : IdentityUserToken<TKey>, new()
        {
            options.Services.AddScoped(
                typeof(IUsersRecord<,,,,,>).MakeGenericType(typeof(TUser), typeof(TKey), typeof(TUserClaim),
                    typeof(TUserRole), typeof(TUserLogin), typeof(TUserToken)), typeof(TUsersRecord));
        }

        /// <summary>
        ///     Add a custom implementation for <see cref="UserTokensRecord{TKey, TUserToken}" />.
        /// </summary>
        /// <typeparam name="TUserTokensRecord">The type of the table to register.</typeparam>
        /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUserTokensRecord<TUserTokensRecord, TUserToken>(this DapperStoreOptions options)
            where TUserTokensRecord : UserTokensRecord<string, TUserToken>
            where TUserToken : IdentityUserToken<string>, new()
        {
            options.AddUserTokensRecord<TUserTokensRecord, string, TUserToken>();
        }

        /// <summary>
        ///     Add a custom implementation for <see cref="UserTokensRecord{TKey, TUserToken}" />.
        /// </summary>
        /// <typeparam name="TUserTokensRecord">The type of the table to register.</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a role and user.</typeparam>
        /// <typeparam name="TUserToken">The type representing a user token.</typeparam>
        /// <param name="options">Options for configuring Dapper stores.</param>
        public static void AddUserTokensRecord<TUserTokensRecord, TKey, TUserToken>(this DapperStoreOptions options)
            where TUserTokensRecord : UserTokensRecord<TKey, TUserToken>
            where TKey : IEquatable<TKey>
            where TUserToken : IdentityUserToken<TKey>, new()
        {
            options.Services.AddScoped(typeof(IUserTokensRecord<,>).MakeGenericType(typeof(TKey), typeof(TUserToken)),
                typeof(TUserTokensRecord));
        }
    }
}