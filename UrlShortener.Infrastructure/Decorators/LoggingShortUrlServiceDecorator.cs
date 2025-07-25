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
    public class LoggingShortUrlServiceDecorator : IShortUrlService
    {
        private readonly IShortUrlService _inner;
        private readonly ILogger<LoggingShortUrlServiceDecorator> _logger;

        public LoggingShortUrlServiceDecorator(IShortUrlService inner, ILogger<LoggingShortUrlServiceDecorator> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task<List<AbstractShortUrlDto>> GetAllAsync(bool? isAuthenticated)
        {
            _logger.LogInformation("Starting GetAllAsync...");
            try
            {
                return await _inner.GetAllAsync(isAuthenticated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllAsync failed.");
                throw;
            }
        }

        public async Task<ShortUrlDto?> GetByIdAsync(Guid id)
        {
            _logger.LogInformation("Starting GetByIdAsync for Id: {Id}...", id);
            try
            {
                return await _inner.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetByIdAsync failed for Id: {Id}.", id);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id, string requestedByUserId, bool isAdmin)
        {
            _logger.LogInformation("Starting DeleteAsync for Id: {Id}, requestedByUserId: {UserId}, isAdmin: {IsAdmin}...", id, requestedByUserId, isAdmin);
            try
            {
                await _inner.DeleteAsync(id, requestedByUserId, isAdmin);
                _logger.LogInformation("DeleteAsync completed successfully for Id: {Id}.", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAsync failed for Id: {Id}.", id);
                throw;
            }
        }

        public async Task<Guid> CreateShortUrlAsync(CreateShortUrlDto dto, string createdByUserId)
        {
            _logger.LogInformation("Starting CreateShortUrlAsync for OriginalUrl: {OriginalUrl} by User: {UserId}...", dto.OriginalUrl, createdByUserId);
            try
            {
                return await _inner.CreateShortUrlAsync(dto, createdByUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateShortUrlAsync failed for OriginalUrl: {OriginalUrl}.", dto.OriginalUrl);
                throw;
            }
        }

        public async Task<string> GetOriginalUrlByShortCodeAsync(string shortCode)
        {
            _logger.LogInformation("Starting GetOriginalUrlByShortCodeAsync for ShortCode: {shortCode}...", shortCode);
            try
            {
                return await _inner.GetOriginalUrlByShortCodeAsync(shortCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetOriginalUrlByShortCodeAsync failed for ShortCode: {shortCode}.", shortCode);
                throw;
            }
        }
    }

}
