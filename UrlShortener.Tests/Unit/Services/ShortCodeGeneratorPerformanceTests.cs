using Microsoft.EntityFrameworkCore;
using UrlShortener.Infrastructure;
using UrlShortener.Infrastructure.Services;
using Xunit;

namespace UrlShortener.Tests.Unit.Services
{
    public class ShortCodeGeneratorPerformanceTests
    {
        private readonly DbContextOptions<UrlShortenerDbContext> _options;

        public ShortCodeGeneratorPerformanceTests()
        {
            _options = new DbContextOptionsBuilder<UrlShortenerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GenerateUniqueCodeAsync_ShouldCompleteWithinReasonableTime()
        {
            // Arrange
            using var context = new UrlShortenerDbContext(_options);
            var generator = new ShortCodeGenerator(context);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            var result = await generator.GenerateUniqueCodeAsync(8);
            stopwatch.Stop();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(8, result.Length);
            Assert.True(stopwatch.ElapsedMilliseconds < 1000, "Code generation should complete within 1 second");
        }

        [Fact]
        public async Task GenerateUniqueCodeAsync_ShouldHandleConcurrentRequests()
        {
            // Arrange
            using var context = new UrlShortenerDbContext(_options);
            var generator = new ShortCodeGenerator(context);
            var tasks = new List<Task<string>>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(generator.GenerateUniqueCodeAsync(8));
            }

            var results = await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(10, results.Length);
            var uniqueResults = results.Distinct().Count();
            Assert.Equal(10, uniqueResults);
        }

        [Fact]
        public async Task GenerateUniqueCodeAsync_ShouldHandleVeryLongCodes()
        {
            // Arrange
            using var context = new UrlShortenerDbContext(_options);
            var generator = new ShortCodeGenerator(context);

            // Act
            var result = await generator.GenerateUniqueCodeAsync(50);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(50, result.Length);
            Assert.Matches("^[a-zA-Z0-9]+$", result);
        }

        [Fact]
        public async Task GenerateUniqueCodeAsync_ShouldHandleVeryShortCodes()
        {
            // Arrange
            using var context = new UrlShortenerDbContext(_options);
            var generator = new ShortCodeGenerator(context);

            // Act
            var result = await generator.GenerateUniqueCodeAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Length);
            Assert.Matches("^[a-zA-Z0-9]+$", result);
        }

        [Fact]
        public async Task GenerateUniqueCodeAsync_ShouldMaintainRandomness()
        {
            // Arrange
            using var context = new UrlShortenerDbContext(_options);
            var generator = new ShortCodeGenerator(context);
            var codes = new List<string>();

            // Act
            for (int i = 0; i < 100; i++)
            {
                var code = await generator.GenerateUniqueCodeAsync(8);
                codes.Add(code);
            }

            // Assert
            var uniqueCodes = codes.Distinct().Count();
            Assert.Equal(100, uniqueCodes); // All codes should be unique

            // Check character distribution (basic randomness check)
            var allChars = string.Join("", codes);
            var charCounts = allChars.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
            
            // Each character should appear multiple times (not just one character repeated)
            Assert.True(charCounts.Count > 10, "Should have good character distribution");
        }

        [Fact]
        public async Task GenerateUniqueCodeAsync_ShouldHandleDatabaseWithManyExistingCodes()
        {
            // Arrange
            using var context = new UrlShortenerDbContext(_options);
            var generator = new ShortCodeGenerator(context);
            
            for (int i = 0; i < 500; i++)
            {
                context.ShortUrls.Add(new Domain.Entities.ShortUrl
                {
                    Id = Guid.NewGuid(),
                    OriginalUrl = $"https://example{i}.com",
                    ShortCode = $"existing{i:D3}",
                    CreatedByUserId = "user1",
                    CreatedAt = DateTime.UtcNow
                });
            }
            await context.SaveChangesAsync();

            // Act
            var result = await generator.GenerateUniqueCodeAsync(8);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(8, result.Length);
            Assert.DoesNotContain("existing", result);
        }
    }
} 