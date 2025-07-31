using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.Infrastructure.Decorators
{
    // Recommended: decorators might be superfluous
    public class LoggingAboutInfoSeederDecorator(
            IAboutInfoSeeder inner,
            ILogger<LoggingAboutInfoSeederDecorator> logger) : IAboutInfoSeeder
    {
        public async Task SeedAsync(string adminEmail)
        {
            logger.LogInformation("Starting AboutInfo seeding for admin {Email}...", adminEmail);
            try
            {
                await inner.SeedAsync(adminEmail);
                logger.LogInformation("AboutInfo seeding for admin {Email} completed successfully.", adminEmail);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "AboutInfo seeding for admin {Email} failed.", adminEmail);
                throw;
            }
        }
    }
}
