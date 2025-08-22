using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RSS_Reader.Models
{
    public class Article
    {
        public int Id { get; set; }


        [ForeignKey(nameof(Feed))]
        public int FeedId { get; set; }
        public Feed? Feed { get; set; }


        [Required]
        public string Title { get; set; } = string.Empty;


        [Required, StringLength(2048)]
        public string Link { get; set; } = string.Empty;


        public DateTimeOffset? PublishedAt { get; set; }
        public string? Summary { get; set; }
    }
}
