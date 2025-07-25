using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Application.Abstractions;
using UrlShortener.Infrastructure.Exceptions;

namespace UrlShortener.Infrastructure.Services
{
    public class ShortCodeGenerator(UrlShortenerDbContext context) : IShortCodeGenerator
    {
        private static readonly char[] _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        public async Task<string> GenerateUniqueCodeAsync(int length = 8)
        {
            for (int attempt = 0; attempt < 10; attempt++) 
            {
                var code = GenerateSecureShortCode(length);
                var exists = await context.ShortUrls.AsNoTracking().AnyAsync(x => x.ShortCode == code);
                if (!exists)
                    return code;
            }

            throw new ShortCodeGenerationException();
        }

        private static string GenerateSecureShortCode(int length)
        {
            var bytes = new byte[length];
            _rng.GetBytes(bytes);

            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                var index = bytes[i] % _chars.Length;
                result[i] = _chars[index];
            }

            return new string(result);
        }
    }
}
