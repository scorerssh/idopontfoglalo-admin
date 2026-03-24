using ApartManBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApartManBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] RequestModels.Auth.LoginRequest request, CancellationToken ct)
        {
            var resault = await _authService.ValidateUserCredentialsAsync(request, ct);
            if (!resault.Susscess)
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var userclaims = await _authService.GenerateClaimsForTheUserAsync(resault.User!, ct);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userclaims);
            return Ok(new { message = "Login successful." });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Unauthorized();
            }


            await _authService.InvalidateUserSessionAsync(userId, ct);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Logout successful." });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                email = User.FindFirstValue(ClaimTypes.Email),
                role = User.FindFirstValue(ClaimTypes.Role)
            });
        }
    }
}
