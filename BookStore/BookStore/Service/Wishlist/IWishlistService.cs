namespace BookStore.Service.Wishlist
{
    public interface IWishlistService
    {
        public Task<List<Models.Wishlist>> GetUserWishlist();
        public Task<bool> AddToWishlist(int bookId);
        public Task<bool> IsBookInWishlist(int bookId);

        public Task<bool> RemoveFromWishlist(int bookId);
    }
}
