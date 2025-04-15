namespace BookStore.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public List<CartItem> CartItems { get; set; }
    }
}
