namespace BookStore.Service.Wishlist
{
    public interface IWishlistService
    {
        public Task<List<Models.Wishlist>> GetAllWishlists();
    }
}
