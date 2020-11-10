using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Netopes.Core.Helpers.Services;

namespace Netopes.Identity
{
    public class UserSession : AppScopedSession
    {
        protected readonly AuthenticationStateProvider _authenticationStateProvider;
        protected readonly UserManager<AppIdentityUser> _userManager;

        public UserSession(
            AuthenticationStateProvider authenticationStateProvider,
            UserManager<AppIdentityUser> userManager)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _userManager = userManager;
        }
        
        public AppIdentityUser CurrentIdentityUser { get; protected set; }
        
        public override async Task LoadStateAsync()
        {
            await GetAuthenticationStateAsync();
            await LoadUserContextAsync();
            IsInitialized = true;
        }

        public virtual async Task<bool> GetAuthenticationStateAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            if ((authState?.User?.Identity?.IsAuthenticated ?? false) == false)
            {
                CurrentIdentityUser = null;
                return false;
            }

            CurrentIdentityUser = await _userManager.GetUserAsync(authState.User);
            return true;
        }

        public virtual async Task LoadUserContextAsync()
        {
            await Task.CompletedTask;
        }
    }
}
