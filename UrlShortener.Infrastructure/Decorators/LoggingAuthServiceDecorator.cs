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
    public class LoggingAuthServiceDecorator : IAuthService
    {
        private readonly IAuthService _inner;
        private readonly ILogger<LoggingAuthServiceDecorator> _logger;

        public LoggingAuthServiceDecorator(IAuthService inner, ILogger<LoggingAuthServiceDecorator> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            _logger.LogInformation("Register attempt for {Email}", dto.Email);
            try
            {
                return await _inner.RegisterAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for {Email}", dto.Email);
                throw;
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            _logger.LogInformation("Login attempt for {Email}", dto.Email);
            try
            {
                return await _inner.LoginAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for {Email}", dto.Email);
                throw;
            }
        }

        public async Task<UserInfoDto> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
        {
            _logger.LogInformation("Fetching current user info");
            try
            {
                return await _inner.GetCurrentUserAsync(userPrincipal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch current user info");
                throw;
            }
        }
    }
}
