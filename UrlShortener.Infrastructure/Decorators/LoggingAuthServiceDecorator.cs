using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs;

namespace UrlShortener.Infrastructure.Decorators
{
    public class LoggingAuthServiceDecorator(
        IAuthService inner, 
        ILogger<LoggingAuthServiceDecorator> logger) : IAuthService
    {
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            logger.LogInformation("Register attempt for {Email}", dto.Email);
            try
            {
                return await inner.RegisterAsync(dto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Registration failed for {Email}", dto.Email);
                throw;
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            logger.LogInformation("Login attempt for {Email}", dto.Email);
            try
            {
                return await inner.LoginAsync(dto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Login failed for {Email}", dto.Email);
                throw;
            }
        }

        public async Task<UserInfoDto> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
        {
            logger.LogInformation("Fetching current user info");
            try
            {
                return await inner.GetCurrentUserAsync(userPrincipal);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch current user info");
                throw;
            }
        }
    }
}
