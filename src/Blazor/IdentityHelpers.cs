using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Netopes.Identity.Blazor
{
    public static class IdentityHelpers
    {
        public static async Task PasswordSignInForBalzor(string username,
            string password,
            SignInManager<AppIdentityUser> signInManager,
            IHostEnvironmentAuthenticationStateProvider hostAuthentication)
        {
            AppIdentityUser user;
            try
            {
                user = await signInManager.UserManager.FindByNameAsync(username);
                if (user == null)
                {
                    throw new UserAuthenticationException($"User [{username}] not fond!", "UsernameNotFound");
                }
            }
            catch (Exception e)
            {
                throw new UserAuthenticationException(e.Message, "UserNotFound");
            }

            if (!await signInManager.UserManager.CheckPasswordAsync(user, password))
            {
                throw new UserAuthenticationException("Invalid user password!", "InvalidPassword");
            }

            var principal = await signInManager.CreateUserPrincipalAsync(user);
            var identity = new ClaimsIdentity(
                principal.Claims,
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme
            );
            principal = new ClaimsPrincipal(identity);
            signInManager.Context.User = principal;
            hostAuthentication.SetAuthenticationState(Task.FromResult(new AuthenticationState(principal)));
        }
    }
}