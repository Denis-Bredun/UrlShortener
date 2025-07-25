using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs;

namespace UrlShortener.Infrastructure.Decorators
{
    public class LoggingShortUrlServiceDecorator(
        IShortUrlService inner, 
        ILogger<LoggingShortUrlServiceDecorator> logger) : IShortUrlService
    {
        public async Task<List<AbstractShortUrlDto>> GetAllAsync(bool? isAuthenticated)
        {
            logger.LogInformation("Starting GetAllAsync...");
            try
            {
                return await inner.GetAllAsync(isAuthenticated);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAllAsync failed.");
                throw;
            }
        }

        public async Task<ShortUrlDto?> GetByIdAsync(Guid id)
        {
            logger.LogInformation("Starting GetByIdAsync for Id: {Id}...", id);
            try
            {
                return await inner.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetByIdAsync failed for Id: {Id}.", id);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id, string requestedByUserId, bool isAdmin)
        {
            logger.LogInformation("Starting DeleteAsync for Id: {Id}, requestedByUserId: {UserId}, isAdmin: {IsAdmin}...", id, requestedByUserId, isAdmin);
            try
            {
                await inner.DeleteAsync(id, requestedByUserId, isAdmin);
                logger.LogInformation("DeleteAsync completed successfully for Id: {Id}.", id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "DeleteAsync failed for Id: {Id}.", id);
                throw;
            }
        }

        public async Task<Guid> CreateShortUrlAsync(CreateShortUrlDto dto, string createdByUserId)
        {
            logger.LogInformation("Starting CreateShortUrlAsync for OriginalUrl: {OriginalUrl} by User: {UserId}...", dto.OriginalUrl, createdByUserId);
            try
            {
                return await inner.CreateShortUrlAsync(dto, createdByUserId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "CreateShortUrlAsync failed for OriginalUrl: {OriginalUrl}.", dto.OriginalUrl);
                throw;
            }
        }

        public async Task<string> GetOriginalUrlByShortCodeAsync(string shortCode)
        {
            logger.LogInformation("Starting GetOriginalUrlByShortCodeAsync for ShortCode: {shortCode}...", shortCode);
            try
            {
                return await inner.GetOriginalUrlByShortCodeAsync(shortCode);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetOriginalUrlByShortCodeAsync failed for ShortCode: {shortCode}.", shortCode);
                throw;
            }
        }
    }

}
