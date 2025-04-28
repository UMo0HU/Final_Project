namespace BookStore.Models
{
    public class Wishlist
    {
        public string UserId { get; set; }
        public User? User { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }
    }
}
