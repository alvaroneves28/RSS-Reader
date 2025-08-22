using CodeHollow.FeedReader;
using Microsoft.EntityFrameworkCore;
using RSS_Reader.Data;
using RSS_Reader.Entities;
using RSS_Reader.Interfaces;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace RSS_Reader.Services
{
    /// <summary>
    /// Service that handles ingestion of RSS feeds and inserts articles into the database.
    /// </summary>
    public class RssIngestService : IRssIngestService
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of <see cref="RssIngestService"/>.
        /// </summary>
        /// <param name="db">Application database context.</param>
        public RssIngestService(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Reloads articles from a feed and inserts any new ones into the database.
        /// </summary>
        /// <param name="feed">The feed to reload articles for.</param>
        /// <param name="ct">Optional cancellation token.</param>
        /// <returns>The number of new articles inserted.</returns>
        public async Task<int> ReloadArticlesAsync(Entities.Feed feed, CancellationToken ct = default)
        {
            // Parse the RSS feed
            var parsed = await FeedReader.ReadAsync(feed.Url, ct);
            int inserted = 0;

            // Load existing links for this feed into memory to avoid duplicates
            var existingLinks = await _db.Articles
                .Where(a => a.FeedId == feed.Id)
                .Select(a => a.Link)
                .ToListAsync(ct);

            var seenLinks = new HashSet<string>(existingLinks);

            foreach (var item in parsed.Items)
            {
                var link = item.Link?.Trim();
                if (string.IsNullOrWhiteSpace(link)) continue;

                // Normalize HTML entities
                link = WebUtility.HtmlDecode(link);

                // Skip duplicates in DB and in current feed loop
                if (seenLinks.Contains(link)) continue;
                seenLinks.Add(link);

                var article = new Article
                {
                    FeedId = feed.Id,
                    Title = item.Title?.Trim() ?? "(No title)",
                    Link = link,
                    PublishedAt = item.PublishingDate,
                    Summary = item.Description
                };

                _db.Articles.Add(article);
                inserted++;
            }

            await _db.SaveChangesAsync(ct);
            return inserted;
        }
    }
}
