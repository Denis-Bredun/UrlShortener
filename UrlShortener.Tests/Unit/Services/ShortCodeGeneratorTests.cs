using Microsoft.EntityFrameworkCore;
using UrlShortener.Application.Abstractions;
using UrlShortener.Infrastructure;
using UrlShortener.Infrastructure.Exceptions;
using UrlShortener.Infrastructure.Services;
using Xunit;

namespace UrlShortener.Tests.Unit.Services
{
    public class ShortCodeGeneratorTests
    {
        private readonly DbContextOptions<UrlShortenerDbContext> _options;

        public ShortCodeGeneratorTests()
        {
            _options = new DbContextOptionsBuilder<UrlShortenerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GenerateUniqueCodeAsync_ShouldReturnCodeWithDefaultLength()
        {
            // Arrange
            using var context = new UrlShortenerDbContext(_options);
            var generator = new ShortCodeGenerator(context);

            // Act
            var result = await generator.GenerateUniqueCodeAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(8, result.Length);
            Assert.Matches("^[a-zA-Z0-9]+$", result);
        }

        [Theory]
        [InlineData(6)]
        [InlineData(10)]
        [InlineData(12)]
        public async Task GenerateUniqueCodeAsync_ShouldReturnCodeWithSpecifiedLength(int length)
        {
            // Arrange
            using var context = new UrlShortenerDbContext(_options);
            var generator = new ShortCodeGenerator(context);

            // Act
            var result = await generator.GenerateUniqueCodeAsync(length);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(length, result.Length);
            Assert.Matches("^[a-zA-Z0-9]+$", result);
        }

        [Fact]
        public async Task GenerateUniqueCodeAsync_ShouldReturnUniqueCodes()
        {
            // Arrange
            using var context = new UrlShortenerDbContext(_options);
            var generator = new ShortCodeGenerator(context);
            var codes = new HashSet<string>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                var code = await generator.GenerateUniqueCodeAsync();
                codes.Add(code);
            }

            // Assert
            Assert.Equal(10, codes.Count);
        }

        [Fact]
        public async Task GenerateUniqueCodeAsync_ShouldHandleExistingCodes()
        {
            // Arrange
            using var context = new UrlShortenerDbContext(_options);
            var generator = new ShortCodeGenerator(context);
            
            var existingCode = "test1234";
            context.ShortUrls.Add(new Domain.Entities.ShortUrl
            {
                Id = Guid.NewGuid(),
                OriginalUrl = "https://example.com",
                ShortCode = existingCode,
                CreatedByUserId = "user1",
                CreatedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            // Act
            var result = await generator.GenerateUniqueCodeAsync(8);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(existingCode, result);
            Assert.Equal(8, result.Length);
        }

        [Fact]
        public async Task GenerateUniqueCodeAsync_ShouldThrowExceptionAfterMaxAttempts()
        {
            // Arrange
            using var context = new UrlShortenerDbContext(_options);
            var generator = new ShortCodeGenerator(context);
            
            var chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
            var codeCount = 0;
            
            for (int i = 0; i < chars.Length && codeCount < 4000; i++)
            {
                for (int j = 0; j < chars.Length && codeCount < 4000; j++)
                {
                    context.ShortUrls.Add(new Domain.Entities.ShortUrl
                    {
                        Id = Guid.NewGuid(),
                        OriginalUrl = $"https://example{codeCount}.com",
                        ShortCode = $"{chars[i]}{chars[j]}",
                        CreatedByUserId = "user1",
                        CreatedAt = DateTime.UtcNow
                    });
                    codeCount++;
                }
            }
            await context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<ShortCodeGenerationException>(
                () => generator.GenerateUniqueCodeAsync(2)); 
        }

        [Fact]
        public async Task GenerateUniqueCodeAsync_ShouldGenerateValidCharacters()
        {
            // Arrange
            using var context = new UrlShortenerDbContext(_options);
            var generator = new ShortCodeGenerator(context);
            var validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

            // Act
            var result = await generator.GenerateUniqueCodeAsync(20);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(20, result.Length);
            Assert.All(result, c => Assert.Contains(c, validChars));
        }

        [Fact]
        public async Task GenerateUniqueCodeAsync_ShouldNotGenerateSequentialCodes()
        {
            // Arrange
            using var context = new UrlShortenerDbContext(_options);
            var generator = new ShortCodeGenerator(context);
            var codes = new List<string>();

            // Act
            for (int i = 0; i < 5; i++)
            {
                var code = await generator.GenerateUniqueCodeAsync(8);
                codes.Add(code);
            }

            // Assert
            for (int i = 0; i < codes.Count - 1; i++)
            {
                Assert.NotEqual(codes[i], codes[i + 1]);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GenerateUniqueCodeAsync_ShouldHandleInvalidLengths(int invalidLength)
        {
            // Arrange
            using var context = new UrlShortenerDbContext(_options);
            var generator = new ShortCodeGenerator(context);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ShortCodeInvalidLengthException>(
                () => generator.GenerateUniqueCodeAsync(invalidLength));
            
            Assert.Contains(invalidLength.ToString(), exception.Message);
        }
    }
} 