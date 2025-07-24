using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure
{
    public class UrlShortenerDbContext : IdentityDbContext<IdentityUser>
    {
        public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options)
            : base(options)
        {
        }

        public DbSet<ShortUrl> ShortUrls { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ShortUrl>()
                .HasOne<IdentityUser>() 
                .WithMany()             
                .HasForeignKey(s => s.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ShortUrl>()
                .HasIndex(s => s.ShortCode)
                .IsUnique();
        }
    }

}
