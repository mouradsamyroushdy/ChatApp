
using ChatApp.DataAccess.Interfaces;
using ChatApp.Database.Entities;
using ChatApp.Extensions;
using ChatApp.WebAPI.DTOs.Request;
using ChatApp.WebAPI.DTOs.Result;
using ChatApp.WebAPI.Models;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatApp.WebAPI.Managers
{
    public class SignInManager : SignInManager<User>
    {
        private readonly ILogger<SignInManager> _logger;
        private readonly IJwtAuthenticationManager _jwtAuthManager;
        private readonly JwtConfig _jwtTokenConfig;
        private readonly UserManager _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IDistributedCache _cache;
        private readonly IUnitOfWork _uow;

        public SignInManager(
            ILogger<SignInManager> logger,
            IJwtAuthenticationManager jwtAuthManager,
            JwtConfig jwtTokenConfig,
            UserManager userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<User> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<User> confirmation,
            IDistributedCache cache,
            IUnitOfWork uow) : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
            _logger = logger;
            _jwtAuthManager = jwtAuthManager;
            _jwtTokenConfig = jwtTokenConfig;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _cache = cache;
            _uow = uow;
        }

        public async Task<LoginResult> SignInAsync(string userName, string password)
        {
            _logger.LogInformation($"Validating user [{userName}]", userName);

            var result = new LoginResult();

            if (string.IsNullOrWhiteSpace(userName)) return result;
            if (string.IsNullOrWhiteSpace(password)) return result;

            var signInResult = await PasswordSignInAsync(userName, password, false, false);
            if (signInResult != null && signInResult.Succeeded)

            {
                var user = await _uow.Users.GetUserByNameAsync(userName);

                var claimsPrincipal = _userManager.GetPrincipalFromUser(user);
                result.User = await _userManager.GetUserAsync(claimsPrincipal);
                result.AccessToken = _jwtAuthManager.BuildToken(claimsPrincipal.Claims);
                result.RefreshToken = _jwtAuthManager.BuildRefreshToken();
                _uow.RefreshTokens.Insert(new RefreshToken { UserId = user.Id, Token = result.RefreshToken, IssuedAt = DateTime.Now, ExpiresAt = DateTime.Now.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration) });
                await _uow.SaveChangesAsync();

                result.Succeeded = true;
            };

            return result;
        }

        public async Task<RefreshTokenResult> RefreshToken(RefreshTokenRequest request)
        {
            RefreshTokenResult result = new();

            ClaimsPrincipal claimsPrincipal = _jwtAuthManager.GetPrincipalFromToken(request.AccessToken);
            if (claimsPrincipal == null) return result;
            var userId = Convert.ToInt32(claimsPrincipal.GetUserId());
            var user = await _uow.Users.FindAsync(userId);
            if (user == null) return result;

            var oldToken = await _uow.RefreshTokens.GetRefreshToken(user, request.RefreshToken, DateTime.Now);
            if (oldToken == null) return result;

            result.AccessToken = _jwtAuthManager.BuildToken(claimsPrincipal.Claims);
            result.RefreshToken = _jwtAuthManager.BuildRefreshToken();

            var newToken = new RefreshToken
            {
                UserId = user.Id,
                Token = result.RefreshToken,
                IssuedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(_jwtTokenConfig.RefreshTokenExpiration)
            };

            _uow.RefreshTokens.Delete(oldToken);
            _uow.RefreshTokens.Insert(newToken);
            await _uow.SaveChangesAsync();

            result.Succeeded = true;
            return result;
        }

        public override async Task SignOutAsync()
        {
            var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);

            var refreshToken = await _uow.RefreshTokens.GetRefreshTokenByUser(user.Id);
            if (refreshToken is not null)
            {
                _uow.RefreshTokens.Delete(refreshToken);
                await _uow.SaveChangesAsync();
            }

            var accessToken = _jwtAuthManager.GetAccessToken();
            await _jwtAuthManager.BlacklistTokenAsync(user.Id, accessToken);

            await SignOutAsync();
        }
    }
}
