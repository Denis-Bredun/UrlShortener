using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs;
using UrlShortener.Application.Mapping;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Exceptions;

namespace UrlShortener.Infrastructure.Services
{
    public class AboutService(UrlShortenerDbContext context) : IAboutService
    {
        public async Task<AboutDto> GetAboutInfoAsync()
        {
            var about = await context.AboutInfos
                .Include(a => a.UpdatedByUser)
                .FirstOrDefaultAsync();

            if (IsAboutEmpty(about))
                throw new AboutInfoNotFoundException();

            return Mapper.AboutEntityToDto(about);
        }

        public async Task UpdateAboutInfoAsync(UpdateAboutDto dto, string userId)
        {
            var about = await context.AboutInfos.FirstOrDefaultAsync();

            if (IsAboutEmpty(about))
            {
                about = new AboutInfo
                {
                    Description = dto.Description,
                    LastUpdated = DateTime.UtcNow,
                    UpdatedById = userId
                };
                context.AboutInfos.Add(about);
            }
            else
            {
                about.Description = dto.Description;
                about.LastUpdated = DateTime.UtcNow;
                about.UpdatedById = userId;
            }

            await context.SaveChangesAsync();
        }

        private bool IsAboutEmpty(AboutInfo? about)
        {
            return about == null || string.IsNullOrWhiteSpace(about.UpdatedById);
        }
    }
}
