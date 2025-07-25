using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs;
using UrlShortener.Application.Mapping;
using UrlShortener.Domain.Entities;
using UrlShortener.Infrastructure.Exceptions;

namespace UrlShortener.Infrastructure.Services
{
    public class ShortUrlService(
            UrlShortenerDbContext context,
            IShortCodeGenerator shortCodeGenerator) : IShortUrlService
    {
        public async Task<List<AbstractShortUrlDto>> GetAllAsync(bool? isAuthenticated)
        {
            var entities = await context.ShortUrls
                .AsNoTracking()
                .Include(u => u.CreatedByUser)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            IEnumerable<AbstractShortUrlDto> urls = isAuthenticated == true
                ? entities.Select(Mapper.EntityToDto)
                : entities.Select(Mapper.EntityToSafeDto);

            return urls.ToList();
        }

        public async Task<ShortUrlDto?> GetByIdAsync(Guid id)
        {
            var entity = await context.ShortUrls
                .AsNoTracking()
                .Include(u => u.CreatedByUser)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (IsEntityEmpty(entity))
                throw new ShortUrlNotFoundException();

            return Mapper.EntityToDto(entity);
        }

        public async Task DeleteAsync(Guid id, string requestedByUserId, bool isAdmin)
        {
            var entity = await context.ShortUrls.FindAsync(id);
            if (IsEntityEmpty(entity))
                throw new ShortUrlNotFoundException();

            if (IsDeletingWhileNotAdmin(requestedByUserId, isAdmin, entity))
                throw new ShortUrlForbiddenDeletionException();

            context.ShortUrls.Remove(entity);
            await context.SaveChangesAsync();
        }

        private bool IsDeletingWhileNotAdmin(string requestedByUserId, bool isAdmin, ShortUrl entity)
        {
            return !isAdmin && entity.CreatedByUserId != requestedByUserId;
        }

        public async Task<Guid> CreateShortUrlAsync(CreateShortUrlDto dto, string createdByUserId)
        {
            var exists = await context.ShortUrls.AnyAsync(u => u.OriginalUrl == dto.OriginalUrl);
            if (exists)
                throw new ShortUrlCreationConflictException();

            var shortCode = await shortCodeGenerator.GenerateUniqueCodeAsync();

            var entity = new ShortUrl
            {
                Id = Guid.NewGuid(),
                OriginalUrl = dto.OriginalUrl,
                ShortCode = shortCode,
                CreatedByUserId = createdByUserId,
                CreatedAt = DateTime.UtcNow
            };

            context.ShortUrls.Add(entity);
            await context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<string> GetOriginalUrlByShortCodeAsync(string shortCode)
        {
            if (IsShortCodeEmpty(shortCode))
                throw new ShortCodeEmptyException();

            var entity = await context.ShortUrls
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ShortCode == shortCode);

            if (IsEntityEmpty(entity))
                throw new ShortUrlNotFoundException();

            return entity.OriginalUrl;
        }

        private bool IsShortCodeEmpty(string shortCode)
        {
            return string.IsNullOrWhiteSpace(shortCode);
        }

        private bool IsEntityEmpty(ShortUrl? entity)
        {
            return entity == null || string.IsNullOrWhiteSpace(entity.OriginalUrl);
        }
    }
}
