using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs;

namespace UrlShortener.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="dto">User registration data.</param>
        /// <returns>JWT token and user info.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await authService.RegisterAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Authenticates a user with email and password.
        /// </summary>
        /// <param name="dto">User login data.</param>
        /// <returns>JWT token and user info.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await authService.LoginAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Returns information about the currently authenticated user.
        /// </summary>
        /// <returns>User ID, email, username, and role.</returns>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userInfo = await authService.GetCurrentUserAsync(User);
            return Ok(userInfo);
        }
    }
}
