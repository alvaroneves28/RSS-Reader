using RSS_Reader.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace RSS_Reader.Interfaces
{
    /// <summary>
    /// Defines the contract for a service that ingests RSS feeds
    /// and manages articles in the database.
    /// </summary>
    public interface IRssIngestService
    {
        /// <summary>
        /// Reloads the articles of a given feed from its RSS URL,
        /// inserting any new articles into the database.
        /// </summary>
        /// <param name="feed">The feed to reload articles for.</param>
        /// <param name="ct">Optional cancellation token.</param>
        /// <returns>
        /// The number of new articles successfully inserted into the database.
        /// </returns>
        Task<int> ReloadArticlesAsync(Feed feed, CancellationToken ct = default);
    }
}
