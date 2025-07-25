using Microsoft.AspNetCore.Identity;

namespace UrlShortener.Domain.Entities
{
    public class ShortUrl
    {
        public Guid Id { get; set; }
        public string OriginalUrl { get; set; } = null!;
        public string ShortCode { get; set; } = null!;
        public string CreatedByUserId { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public IdentityUser? CreatedByUser { get; set; }
    }
}
