using System.ComponentModel.DataAnnotations;

namespace RSS_Reader.Entities
{
    public class Feed
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, Url, StringLength(2048)]
        public string Url { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Article> Articles { get; set; } = new List<Article>();
    }
}
