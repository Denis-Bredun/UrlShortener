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
    public class LoggingAboutServiceDecorator(
        IAboutService inner, 
        ILogger<LoggingAboutServiceDecorator> logger) : IAboutService
    {

        public async Task<AboutDto> GetAboutInfoAsync()
        {
            logger.LogInformation("Starting GetAboutInfoAsync...");
            try
            {
                return await inner.GetAboutInfoAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAboutInfoAsync failed.");
                throw;
            }
        }

        public async Task UpdateAboutInfoAsync(UpdateAboutDto dto, string userId)
        {
            logger.LogInformation("Starting UpdateAboutInfoAsync by UserId: {UserId}...", userId);
            try
            {
                await inner.UpdateAboutInfoAsync(dto, userId);
                logger.LogInformation("UpdateAboutInfoAsync completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "UpdateAboutInfoAsync failed.");
                throw;
            }
        }
    }
}
