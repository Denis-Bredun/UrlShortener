using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure
{
    public class UrlShortenerDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<ShortUrl> ShortUrls { get; set; } = null!;
        public DbSet<AboutInfo> AboutInfos { get; set; } = null!;

        public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options)
            : base(options)
        {
        }

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

            modelBuilder.Entity<AboutInfo>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(a => a.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
