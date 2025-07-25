using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Application.DTOs
{
    public record class AboutDto(string Description, DateTime LastUpdated, string UpdatedByUserName);
    public record class UpdateAboutDto(string Description);
}
