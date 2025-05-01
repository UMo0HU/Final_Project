using BookStore.Models;

namespace BookStore.ViewModels
{
    public class UserReviewsViewModel
    {
        public List<Review> reviews { get; set; }
        public List<int> UserReviewsId { get; set; }
    }
}
