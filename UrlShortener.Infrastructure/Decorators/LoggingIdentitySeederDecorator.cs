using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.Infrastructure.Decorators
{
    public class LoggingIdentitySeederDecorator(
        IIdentitySeeder inner, 
        ILogger<LoggingIdentitySeederDecorator> logger) : IIdentitySeeder
    {
        public async Task SeedRolesAsync()
        {
            logger.LogInformation("Starting roles seeding...");
            try
            {
                await inner.SeedRolesAsync();
                logger.LogInformation("Roles seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Roles seeding failed.");
                throw;
            }
        }

        public async Task SeedAdminUserAsync(string email, string password, string username)
        {
            logger.LogInformation("Starting admin user seeding for {Email}...", email);
            try
            {
                await inner.SeedAdminUserAsync(email, password, username);
                logger.LogInformation("Admin user seeding for {Email} completed successfully.", email);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Admin user seeding for {Email} failed.", email);
                throw;
            }
        }
    }
}
