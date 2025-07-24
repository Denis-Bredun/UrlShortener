using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Domain.Entities
{
    public class AboutInfo
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public DateTime LastUpdated { get; set; }
        public string UpdatedById { get; set; } = null!;
    }
}
