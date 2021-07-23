
using ChatApp.DataAccess.Interfaces;
using ChatApp.Database.Entities;
using ChatApp.Extensions;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;


namespace ChatApp.WebAPI.Managers
{
    public class UserManager : UserManager<User>
    {
        private readonly IUnitOfWork _uow;
        private readonly IJwtAuthenticationManager _jwtAuthManager;

        public UserManager(
            IUserStore<User> store,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager> logger,
            IOptions<IdentityOptions> optionsAccessor,
            IUnitOfWork uow,
            IJwtAuthenticationManager jwtAuthManager
            ) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _uow = uow;
            _jwtAuthManager = jwtAuthManager;
        }

        public async Task<User> GetUserFromToken(string accessToken)
        {
            ClaimsPrincipal claimsPrincipal = _jwtAuthManager.GetPrincipalFromToken(accessToken);
            if (claimsPrincipal == null) return null;
            var userId = Convert.ToInt32(claimsPrincipal.GetUserId());
            return await _uow.Users.FindAsync(userId);
        }

        public ClaimsPrincipal GetPrincipalFromUser(User user)
        {
            if (user is null) return null;
            ClaimsIdentity identity = new();
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            return new ClaimsPrincipal(identity);
        }
    }
}
