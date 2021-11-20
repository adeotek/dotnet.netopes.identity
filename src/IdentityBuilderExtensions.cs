using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Netopes.Core.Helpers.Database;
using Netopes.Identity;
using Netopes.Identity.Abstract;
using Netopes.Identity.Data;
using Netopes.Identity.Stores;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Extension methods on <see cref="IdentityBuilder" /> class.
    /// </summary>
    public static class IdentityBuilderExtensions
    {
        /// <summary>
        ///     Adds a Dapper implementation of ASP.NET Core Identity stores.
        /// </summary>
        /// <param name="builder">Helper functions for configuring identity services.</param>
        /// <param name="configureAction">Delegate for configuring options </param>
        /// <returns>The <see cref="IdentityBuilder" /> instance this method extends.</returns>
        public static IdentityBuilder AddDapperStores(this IdentityBuilder builder, IDbConnectionFactory dbConnectionFactory,
            Action<DapperStoreOptions> configureAction = null)
        {
            AddStores(builder.Services, builder.UserType, builder.RoleType, dbConnectionFactory, configureAction);
            return builder;
        }

        private static void AddStores(IServiceCollection services, Type userType, Type roleType, IDbConnectionFactory dbConnectionFactory,
            Action<DapperStoreOptions> configureAction = null)
        {
            var identityUserType = FindGenericBaseType(userType, typeof(IdentityUser<>));
            if (identityUserType == null)
                throw new InvalidOperationException(
                    $"Method {nameof(AddDapperStores)} can only be called with a user that derives from IdentityUser<TKey>.");

            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var dbConnectionContextOptions = new DapperStoreOptions
            {
                ConnectionString = configuration.GetConnectionString("DefaultConnection"),
                DbConnectionFactory = dbConnectionFactory,
                Services = services
            };
            configureAction?.Invoke(dbConnectionContextOptions);
            dbConnectionContextOptions.Services = null;
            var keyType = identityUserType.GenericTypeArguments[0];
            services.TryAddScoped(typeof(IDbConnectionFactory), x =>
            {
                var dbConnectionFactoryInstance =
                    (IDbConnectionFactory)Activator.CreateInstance(dbConnectionContextOptions.DbConnectionFactory
                        .GetType());
                dbConnectionFactoryInstance.ConnectionString = dbConnectionContextOptions.ConnectionString;
                return dbConnectionFactoryInstance;
            });
            Type userStoreType;
            var userClaimType = typeof(IdentityUserClaim<>).MakeGenericType(keyType);
            var userRoleType = typeof(IdentityUserRole<>).MakeGenericType(keyType);
            var userLoginType = typeof(IdentityUserLogin<>).MakeGenericType(keyType);
            var roleClaimType = typeof(IdentityRoleClaim<>).MakeGenericType(keyType);
            var userTokenType = typeof(IdentityUserToken<>).MakeGenericType(keyType);
            if (roleType != null)
            {
                var identityRoleType = FindGenericBaseType(roleType, typeof(IdentityRole<>));
                if (identityRoleType == null)
                    throw new InvalidOperationException(
                        $"Method {nameof(AddDapperStores)} can only be called with a role that derives from IdentityRole<TKey>.");

                services.TryAddScoped(
                    typeof(IUsersRecord<,,,,,>).MakeGenericType(userType, keyType, userClaimType, userRoleType,
                        userLoginType, userTokenType),
                    typeof(UsersRecord<,,,,,>).MakeGenericType(userType, keyType, userClaimType, userRoleType,
                        userLoginType, userTokenType)
                );
                services.TryAddScoped(typeof(IRolesRecord<,,>).MakeGenericType(roleType, keyType, roleClaimType),
                    typeof(RolesRecord<,,>).MakeGenericType(roleType, keyType, roleClaimType));
                services.TryAddScoped(typeof(IUserRolesRecord<,,>).MakeGenericType(roleType, keyType, userRoleType),
                    typeof(UserRolesRecord<,,>).MakeGenericType(roleType, keyType, userRoleType));
                services.TryAddScoped(typeof(IRoleClaimsRecord<,>).MakeGenericType(keyType, roleClaimType),
                    typeof(RoleClaimsRecord<,>).MakeGenericType(keyType, roleClaimType));
                services.TryAddScoped(typeof(IRoleStore<>).MakeGenericType(roleType),
                    typeof(RoleStore<,,,>).MakeGenericType(roleType, keyType, userRoleType, roleClaimType));
                userStoreType = typeof(UserStore<,,,,,,,>).MakeGenericType(userType, roleType, keyType,
                    userClaimType, userRoleType, userLoginType, userTokenType, roleClaimType);
            }
            else
            {
                services.TryAddScoped(
                    typeof(IUsersOnlyRecord<,,,,>).MakeGenericType(userType, keyType, userClaimType, userLoginType,
                        userTokenType),
                    typeof(UsersRecord<,,,,,>).MakeGenericType(userType, keyType, userClaimType, roleType,
                        userLoginType, userTokenType)
                );
                userStoreType = typeof(UserOnlyStore<,,,,>).MakeGenericType(userType, keyType, userClaimType,
                    userLoginType, userTokenType);
            }

            services.TryAddScoped(typeof(IUserClaimsRecord<,>).MakeGenericType(keyType, userClaimType),
                typeof(UserClaimsRecord<,>).MakeGenericType(keyType, userClaimType));
            services.TryAddScoped(typeof(IUserLoginsRecord<,,>).MakeGenericType(userType, keyType, userLoginType),
                typeof(UserLoginsRecord<,,>).MakeGenericType(userType, keyType, userLoginType));
            services.TryAddScoped(typeof(IUserTokensRecord<,>).MakeGenericType(keyType, userTokenType),
                typeof(UserTokensRecord<,>).MakeGenericType(keyType, userTokenType));
            services.TryAddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
        }

        private static TypeInfo FindGenericBaseType(Type currentType, Type genericBaseType)
        {
            var type = currentType;
            while (type != null)
            {
                var typeInfo = type.GetTypeInfo();
                var genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
                if (genericType != null && genericType == genericBaseType) return typeInfo;

                type = type.BaseType;
            }

            return null;
        }
    }
}