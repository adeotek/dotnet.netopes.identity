using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Netopes.Core.Helpers.Services;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Netopes.Identity
{
    public class UserSession<TIdentityUser, TKey> : AppScopedSession 
        where TIdentityUser : IdentityUser<TKey> 
        where TKey : IEquatable<TKey>
    {
        protected readonly AuthenticationStateProvider _authenticationStateProvider;
        protected readonly UserManager<TIdentityUser> _userManager;
        protected readonly ILogger<UserSession<TIdentityUser, TKey>> _logger;
        protected bool _dbContextLoading = false;

        public UserSession(
            AuthenticationStateProvider authenticationStateProvider,
            UserManager<TIdentityUser> userManager,
            ILogger<UserSession<TIdentityUser, TKey>> logger)
        {
            _authenticationStateProvider = authenticationStateProvider;
            _userManager = userManager;
            _logger = logger;
        }

        public TIdentityUser CurrentIdentityUser { get; protected set; }

        public override async Task LoadStateAsync()
        {
            await GetAuthenticationStateAsync();
            await LoadUserContextAsync();
            IsInitialized = true;
        }

        public virtual async Task<bool> GetAuthenticationStateAsync()
        {
            if (_dbContextLoading)
            {
                var timer = new Stopwatch();
                timer.Start();
                while (_dbContextLoading && timer.Elapsed.TotalMilliseconds < 3000)
                {
                    Thread.Sleep(200);
                }
                timer.Stop();

                if (_dbContextLoading)
                {
                    _logger!.LogWarning("GetAuthenticationStateAsync exited because DbContext is in use!!!");
                    return false;
                }
            }

            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            if ((authState?.User?.Identity?.IsAuthenticated ?? false) == false)
            {
                CurrentIdentityUser = default;
                return false;
            }

            try
            {
                _dbContextLoading = true;
                CurrentIdentityUser = await _userManager.GetUserAsync(authState.User);
                return true;
            }
            finally
            {
                _dbContextLoading = false;
            }
        }

        public virtual async Task LoadUserContextAsync()
        {
            await Task.CompletedTask;
        }
    }

    public class UserSession : UserSession<AppIdentityUser, Guid>
    {
        public UserSession(AuthenticationStateProvider authenticationStateProvider, UserManager<AppIdentityUser> userManager, ILogger<UserSession> logger) 
            : base(authenticationStateProvider, userManager, logger)
        {
        }
    }
}
