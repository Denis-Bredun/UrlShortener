using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.Infrastructure.Decorators
{
    public class LoggingIdentitySeederDecorator : IIdentitySeeder
    {
        private readonly IIdentitySeeder _inner;
        private readonly ILogger<LoggingIdentitySeederDecorator> _logger;

        public LoggingIdentitySeederDecorator(IIdentitySeeder inner, ILogger<LoggingIdentitySeederDecorator> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task SeedRolesAsync()
        {
            _logger.LogInformation("Starting roles seeding...");
            try
            {
                await _inner.SeedRolesAsync();
                _logger.LogInformation("Roles seeding completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Roles seeding failed.");
                throw;
            }
        }

        public async Task SeedAdminUserAsync(string email, string password)
        {
            _logger.LogInformation("Starting admin user seeding for {Email}...", email);
            try
            {
                await _inner.SeedAdminUserAsync(email, password);
                _logger.LogInformation("Admin user seeding for {Email} completed successfully.", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin user seeding for {Email} failed.", email);
                throw;
            }
        }
    }
}
