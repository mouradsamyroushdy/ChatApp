using ChatApp.WebAPI.Managers;
using ChatApp.WebAPI.Models;

using Microsoft.AspNetCore.Http;

using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.WebAPI.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtConfig _jwtConfig;

        public JwtMiddleware(RequestDelegate next, JwtConfig jwtConfig)
        {
            _next = next;
            _jwtConfig = jwtConfig;
        }

        public async Task Invoke(HttpContext context, IJwtAuthenticationManager jwtAuthManager)
        {
            var token = context.Request.Headers[_jwtConfig.AuthTokenKey].FirstOrDefault()?.Split(" ").Last();
            var user = await jwtAuthManager.ValidateAccessToken(token);
            jwtAuthManager.AttachUserToContext(user);
            await _next(context);
        }
    }
}
