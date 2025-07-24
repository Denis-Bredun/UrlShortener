using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Domain.Entities
{
    public class ShortUrl
    {
        public Guid Id { get; set; }
        public string OriginalUrl { get; set; } = null!;
        public string ShortCode { get; set; } = null!;
        public string CreatedByUserId { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
