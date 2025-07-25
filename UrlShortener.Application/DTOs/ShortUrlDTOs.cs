using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Application.DTOs
{
    public abstract record AbstractShortUrlDto
    {
        public string OriginalUrl { get; init; } = null!;
        public string ShortCode { get; init; } = null!;
        public string CreatedByUsername { get; init; } = null!;
        public DateTime CreatedAt { get; init; }
    }

    public sealed record ShortUrlDto : AbstractShortUrlDto
    {
        public Guid Id { get; init; }
        public string CreatedByUserId { get; init; } = null!;
    }

    public sealed record SafeShortUrlDto : AbstractShortUrlDto;

    public record CreateShortUrlDto(string OriginalUrl);
}
