using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RSS_Reader.Models
{
    /// <summary>
    /// Represents an article in the RSS reader application.
    /// This model is used for views and data transfer.
    /// </summary>
    public class Article
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to the related feed.
        /// </summary>
        [ForeignKey(nameof(Feed))]
        public int FeedId { get; set; }

        /// <summary>
        /// Navigation property to the feed.
        /// </summary>
        public Feed? Feed { get; set; }

        /// <summary>
        /// The title of the article.
        /// </summary>
        [Required]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// URL of the article.
        /// </summary>
        [Required, StringLength(2048)]
        public string Link { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the article was published.
        /// </summary>
        public DateTimeOffset? PublishedAt { get; set; }

        /// <summary>
        /// Optional summary or description of the article.
        /// </summary>
        public string? Summary { get; set; }
    }
}
