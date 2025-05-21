using BookStore.Models;

namespace BookStore.ViewModels
{
    public class ShowBooksViewModel
    {
        public List<Book> books { get; set; }
        public List<int> UserWishlist { get; set; }
        public List <int> UserCart { get; set; }
    }
}
