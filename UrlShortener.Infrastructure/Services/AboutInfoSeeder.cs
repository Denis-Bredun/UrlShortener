using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.Abstractions;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Exceptions;

namespace UrlShortener.Infrastructure.Services
{
    public class AboutInfoSeeder(UrlShortenerDbContext context, UserManager<IdentityUser> userManager) : IAboutInfoSeeder
    {

        public async Task SeedAsync(string adminEmail)
        {
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
                throw new AdminNotFoundException();

            if (!context.AboutInfos.Any())
            {
                context.AboutInfos.Add(new AboutInfo
                {
                    Description = """
                        The short URL code is generated using a cryptographically secure random byte generator (RandomNumberGenerator).
                        Each code consists of uppercase letters, lowercase letters, and digits, with a default length of 8 characters.

                        For every attempt, the system checks the database to ensure the generated code is unique.
                        If a unique code is found, it is returned immediately.
                        If the code already exists, the generator retries—up to 10 attempts in total.
                        If all 10 attempts fail to produce a unique code, a ShortCodeGenerationException is thrown.
                    """,
                    LastUpdated = DateTime.UtcNow,
                    UpdatedById = adminUser.Id
                });

                await context.SaveChangesAsync();
            }
        }
    }

}
