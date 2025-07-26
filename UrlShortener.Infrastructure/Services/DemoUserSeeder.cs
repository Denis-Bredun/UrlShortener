using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.Abstractions;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Services
{
    public class DemoUserSeeder(
            UrlShortenerDbContext dbContext,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IShortCodeGenerator codeGenerator) : IDemoUserSeeder
    {
        public async Task SeedAsync()
        {
            var baseEmail = "userr";
            var password = "1234TJFKDSKJfds$jjfifd&*!";

            for (int i = 1; i <= 4; i++)
            {
                var email = $"{baseEmail}{i}@gmail.com";
                var user = await userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    user = new IdentityUser
                    {
                        UserName = $"user{i}",
                        Email = email,
                        EmailConfirmed = true
                    };

                    await userManager.CreateAsync(user, password);
                    await userManager.AddToRoleAsync(user, "User");
                }

                if (!dbContext.ShortUrls.Any(s => s.CreatedByUserId == user.Id))
                {
                    var urls = new List<ShortUrl>();

                    for (int j = 1; j <= 4; j++)
                    {
                        var originalUrl = $"https://starlink.com/user{i}/link{j}";
                        var shortCode = await codeGenerator.GenerateUniqueCodeAsync();

                        urls.Add(new ShortUrl
                        {
                            Id = Guid.NewGuid(),
                            OriginalUrl = originalUrl,
                            ShortCode = shortCode,
                            CreatedAt = DateTime.UtcNow,
                            CreatedByUserId = user.Id
                        });
                    }

                    dbContext.ShortUrls.AddRange(urls);
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }

}
