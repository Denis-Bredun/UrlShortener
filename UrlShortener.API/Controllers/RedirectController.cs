using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.API.Controllers
{
    [ApiController]
    [Route("/{shortCode}")]
    public class RedirectController : ControllerBase
    {
        private readonly IShortUrlService _shortUrlService;

        public RedirectController(IShortUrlService shortUrlService)
        {
            _shortUrlService = shortUrlService;
        }

        /// <summary>
        /// Redirects to the original URL based on a short code.
        /// Accessible to everyone (even anonymous).
        /// </summary>
        /// <param name="shortCode">The short code of the URL</param>
        /// <returns>301 redirect to the original URL if found, 404 otherwise</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> RedirectToOriginal([FromRoute] string shortCode)
        {
            var originalUrl = await _shortUrlService.GetOriginalUrlByShortCodeAsync(shortCode);
            return RedirectPermanent(originalUrl);
        }
    }
}
