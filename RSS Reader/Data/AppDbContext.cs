using Microsoft.EntityFrameworkCore;
using RSS_Reader.Entities;

namespace RSS_Reader.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Feed> Feeds => Set<Feed>();
        public DbSet<Article> Articles => Set<Article>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Feed>(e =>
            {
                e.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                e.Property(p => p.Url)
                    .IsRequired()
                    .HasMaxLength(2048);

                e.HasIndex(p => p.Url).IsUnique();
            });

            b.Entity<Article>(e =>
            {
                e.Property(p => p.Title)
                    .IsRequired();

                e.Property(p => p.Link)
                    .IsRequired()
                    .HasMaxLength(2048);

                e.HasIndex(p => new { p.FeedId, p.Link }).IsUnique();
            });
        }
    }
}
