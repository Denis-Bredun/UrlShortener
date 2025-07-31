using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs;

namespace UrlShortener.API.Controllers
{
    [Route("api/urls")]
    [ApiController]
    public class UrlsController(IShortUrlService shortUrlService) : ControllerBase
    {
        /// <summary>
        /// Gets all short URLs.
        /// Accessible to all users, including anonymous.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        // Recommended: we may remove it considering the fact that we don't assign Authorize for the whole class
        // Recommended: pagination
        public async Task<IActionResult> GetAll()
        {
            var urls = await shortUrlService.GetAllAsync(User.Identity?.IsAuthenticated);
            return Ok(urls);
        }

        /// <summary>
        /// Gets details of a specific short URL by Id.
        /// Accessible only to authorized users.
        /// </summary>
        /// <param name="id">Short URL identifier</param>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(Guid id)
        {
            var url = await shortUrlService.GetByIdAsync(id);
            return Ok(url);
        }

        /// <summary>
        /// Creates a new short URL.
        /// Accessible only to authorized users.
        /// </summary>
        /// <param name="dto">Data for creation</param>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateShortUrlDto dto)
        {
            var userId = User.Identity!.GetUserId();
            var newUrlId = await shortUrlService.CreateShortUrlAsync(dto, userId);
            return Ok(newUrlId);
        }

        /// <summary>
        /// Deletes a short URL.
        /// Regular users can delete only their own URLs.
        /// Admins can delete any URL.
        /// </summary>
        /// <param name="id">Short URL identifier</param>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.Identity!.GetUserId(); // Recommended: to have a separate service for getting Identity, Id, User's role, e.t.c.
            var isAdmin = User.IsInRole("Admin"); // Recommended: [Authorize(Roles = "Admin")]
            await shortUrlService.DeleteAsync(id, userId, isAdmin);
            return Ok();
        }
    }
}
