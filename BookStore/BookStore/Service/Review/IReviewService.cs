using BookStore.ViewModels;

namespace BookStore.Service.Review
{
    public interface IReviewService
    {
        public Task<bool> AddReview(ReviewViewModel reviewViewModel);
        public Task<bool> DeleteReview(int bookId, string userId);
        public Task<List<Models.Review>> GetReviewsByBookId(int bookId);
        public Task<Models.Review> GetUserReview(int bookId, string userId);


    }
}
