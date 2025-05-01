using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Review
    {
        public int BookId { get; set; }
        public Book Book { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int Rating { get; set; }
    }
}
