namespace BookStore.Service.Cart
{
    public interface ICartService
    {
        public Task<bool> AddBookToCart(int bookId);
        public Task<bool> RemoveBookFromCart(int bookId);
        public Task<bool> BookInCart(int bookId);
        public Task<List<Models.Book>> GetBooksFromCart();
    }
}
