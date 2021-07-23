using ChatApp.Database.Entities;

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatApp.WebAPI.Managers
{
    public interface IJwtAuthenticationManager
    {
        string BuildToken(IEnumerable<Claim> claims);
        string BuildRefreshToken();
        ClaimsPrincipal GetPrincipalFromToken(string token);
        string GetAccessToken();
        Task<bool> IsTokenBlacklistedAsync(int userId, string token);
        Task BlacklistTokenAsync(int userId, string token);
        Task<User> ValidateAccessToken(string token);
        void AttachUserToContext(User user);
    }
}