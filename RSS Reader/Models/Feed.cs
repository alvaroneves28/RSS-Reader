using RSS_Reader.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RSS_Reader.Models
{
    /// <summary>
    /// Represents an RSS feed in the RSS reader application.
    /// This model is used for views and data transfer.
    /// </summary>
    public class Feed
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the feed.
        /// </summary>
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// URL of the feed.
        /// </summary>
        [Required, Url, StringLength(2048)]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the feed was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Collection of articles belonging to this feed.
        /// </summary>
        public ICollection<Article> Articles { get; set; } = new List<Article>();
    }
}
