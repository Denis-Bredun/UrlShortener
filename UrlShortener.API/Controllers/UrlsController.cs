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
    public class UrlsController : ControllerBase
    {
        private readonly IShortUrlService _shortUrlService;

        public UrlsController(IShortUrlService shortUrlService)
        {
            _shortUrlService = shortUrlService;
        }

        /// <summary>
        /// Gets all short URLs.
        /// Accessible to all users, including anonymous.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var urls = await _shortUrlService.GetAllAsync(User.Identity?.IsAuthenticated);
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
            var url = await _shortUrlService.GetByIdAsync(id);
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
            var newUrlId = await _shortUrlService.CreateShortUrlAsync(dto, userId);
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
            var userId = User.Identity!.GetUserId();
            var isAdmin = User.IsInRole("Admin");
            await _shortUrlService.DeleteAsync(id, userId, isAdmin);
            return Ok();
        }
    }
}
