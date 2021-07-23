using ChatApp.Database.Entities;
using ChatApp.WebAPI.DTOs.Request;
using ChatApp.WebAPI.DTOs.Response;
using ChatApp.WebAPI.Managers;
using ChatApp.WebAPI.Models;

using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Threading.Tasks;

namespace ChatApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly ILogger<AccountsController> _logger;
        private UserManager<User> _userManager;
        private SignInManager _signInManager;

        public AccountsController(
            ILogger<AccountsController> logger,
            UserManager<User> userManager,
            SignInManager signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user is null) return Unauthorized();

            var result = await _signInManager.SignInAsync(request.UserName, request.Password);

            if (!result.Succeeded) return Unauthorized();

            _logger.LogInformation($"User [{request.UserName}] logged in the system.");

            return Ok(new LoginResponse
            {
                UserName = result.User.UserName,
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken,
                Succeded = true
            });
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<RefreshTokenResponse>> Refresh([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _signInManager.RefreshToken(request);

            if (!result.Succeeded) return Unauthorized();

            return Ok(new RefreshTokenResponse
            {
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken
            });
        }

        [HttpPost("register")]
        public async Task<bool> Register([FromBody] RegisterRequest request)
        {
            var user = new User
            {
                UserName = request.UserName
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(request.UserName, request.Password);

                return true;
            }

            return false;
        }

        [Attriputes.Authorize]
        [HttpGet("logout")]
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation($"User logged out the system.");
        }
    }
}