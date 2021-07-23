using ChatApp.DataAccess.Interfaces;
using ChatApp.Database.Entities;
using ChatApp.WebAPI.Extensions;
using ChatApp.WebAPI.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.WebAPI.Managers
{
    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {
        private readonly IDistributedCache _cache;
        private readonly JwtConfig _jwtConfig;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<JwtAuthenticationManager> _logger;

        public JwtAuthenticationManager(
            IDistributedCache cache,
            JwtConfig jwtConfig,
            IHttpContextAccessor contextAccessor,
            IUnitOfWork uow,
            ILogger<JwtAuthenticationManager> logger)
        {
            _cache = cache;
            _jwtConfig = jwtConfig;
            _contextAccessor = contextAccessor;
            _uow = uow;
            _logger = logger;
        }

        public string BuildToken(IEnumerable<Claim> claims)
        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                    issuer: _jwtConfig.Issuer,
                    audience: _jwtConfig.Audience,
                    notBefore: DateTime.Now,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(_jwtConfig.AccessTokenExpiration),
                    signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string BuildRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            JwtSecurityTokenHandler tokenValidator = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var parameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = false
            };

            try
            {
                var principal = tokenValidator.ValidateToken(token, parameters, out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogError($"Token validation failed");
                    return null;
                }

                return principal;
            }
            catch (Exception e)
            {
                _logger.LogError($"Token validation failed: {e.Message}");
                return null;
            }
        }

        public async Task BlacklistTokenAsync(int userId, string token)
        {
            var blacklist = await _cache.GetRecordAsync<List<string>>(userId.ToString());
            blacklist = blacklist is null ? new List<string>() : blacklist;
            if (!blacklist.Contains(token))
            {
                blacklist.Add(token);
                await _cache.SetRecordAsync(userId.ToString(), blacklist);
            }
        }

        public async Task<bool> IsTokenBlacklistedAsync(int userId, string token)
        {
            var blacklist = await _cache.GetRecordAsync<List<string>>(userId.ToString());
            return blacklist is not null && blacklist.Contains(token);
        }

        public string GetAccessToken()
        {
            return _contextAccessor.HttpContext.Request.Headers[_jwtConfig.AuthTokenKey].FirstOrDefault()?.Split(" ").Last();
        }

        public async Task<User> ValidateAccessToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            try
            {
                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtConfig.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = _jwtConfig.Issuer,
                    ValidAudience = _jwtConfig.Audience,
                };
                new JwtSecurityTokenHandler().ValidateToken(token, validationParams, out SecurityToken validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
                if (!await IsTokenBlacklistedAsync(userId, token))
                {
                    return await _uow.Users.FindAsync(userId);
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public void AttachUserToContext(User user)
        {
            if (user is null) return;
            _contextAccessor.HttpContext.Items["User"] = user;
        }
    }
}
