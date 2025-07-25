using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs;

namespace UrlShortener.API.Controllers
{
    [Route("api/about")]
    [ApiController]
    public class AboutController(IAboutService aboutService) : ControllerBase
    {
        /// <summary>
        /// Gets the URL shortening algorithm description.
        /// Accessible to all users, including anonymous.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var aboutInfo = await aboutService.GetAboutInfoAsync();
            return Ok(aboutInfo);
        }

        /// <summary>
        /// Updates the URL shortening algorithm description.
        /// Accessible only to administrators.
        /// </summary>
        /// <param name="dto">Update data</param>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateAboutDto dto)
        {
            var userId = User.Identity!.GetUserId();
            await aboutService.UpdateAboutInfoAsync(dto, userId);
            return Ok();
        }
    }
}
