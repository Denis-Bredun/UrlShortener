using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.DTOs;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application.Mapping
{
    public static class Mapper
    {
        public static ShortUrlDto EntityToDto(ShortUrl entity)
        {
            return new ShortUrlDto
            {
                Id = entity.Id,
                OriginalUrl = entity.OriginalUrl,
                ShortCode = entity.ShortCode,
                CreatedByUserId = entity.CreatedByUserId,
                CreatedByUsername = entity.CreatedByUser?.UserName ?? "Unknown",
                CreatedAt = entity.CreatedAt
            };
        }

        public static SafeShortUrlDto EntityToSafeDto(ShortUrl entity)
        {
            return new SafeShortUrlDto
            {
                OriginalUrl = entity.OriginalUrl,
                ShortCode = entity.ShortCode,
                CreatedByUsername = entity.CreatedByUser?.UserName ?? "Unknown",
                CreatedAt = entity.CreatedAt
            };
        }
    }

}
