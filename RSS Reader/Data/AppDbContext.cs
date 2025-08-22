using Microsoft.EntityFrameworkCore;
using RSS_Reader.Entities;

namespace RSS_Reader.Data
{
    /// <summary>
    /// Application database context for managing RSS feeds and articles.
    /// </summary>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Constructor accepting DbContext options.
        /// </summary>
        /// <param name="options">Database context options.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// DbSet representing RSS feeds.
        /// </summary>
        public DbSet<Feed> Feeds => Set<Feed>();

        /// <summary>
        /// DbSet representing articles.
        /// </summary>
        public DbSet<Article> Articles => Set<Article>();

        /// <summary>
        /// Configures entity properties, indexes, and relationships.
        /// </summary>
        /// <param name="modelBuilder">The model builder to configure entities.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Feed entity configuration
            modelBuilder.Entity<Feed>(entity =>
            {
                entity.Property(f => f.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(f => f.Url)
                    .IsRequired()
                    .HasMaxLength(2048);

                entity.HasIndex(f => f.Url).IsUnique();
            });

            // Article entity configuration
            modelBuilder.Entity<Article>(entity =>
            {
                entity.Property(a => a.Title)
                    .IsRequired();

                entity.Property(a => a.Link)
                    .IsRequired()
                    .HasMaxLength(2048);

                entity.HasIndex(a => new { a.FeedId, a.Link }).IsUnique();
            });
        }
    }
}
