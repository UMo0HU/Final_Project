namespace BookStore.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Img { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public ICollection<BookCategory> Book_Categories { get; set; }
        public ICollection<Wishlist> Wishlists { get; set; }
        public ICollection<Review> reviews { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
        public ICollection<OrderItem> orderItems { get; set; }
    }
}
