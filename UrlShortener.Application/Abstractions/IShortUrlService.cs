using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs;

namespace UrlShortener.Application.Abstractions
{
    public interface IShortUrlService
    {
        Task<Guid> CreateShortUrlAsync(CreateShortUrlDto dto, string createdByUserId);
        Task DeleteAsync(Guid id, string requestedByUserId, bool isAdmin);
        Task<List<AbstractShortUrlDto>> GetAllAsync(bool? isAuthenticated);
        Task<ShortUrlDto?> GetByIdAsync(Guid id);
        Task<string> GetOriginalUrlByShortCodeAsync(string shortCode);
    }
}
