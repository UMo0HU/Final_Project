using BookStore.ViewModels;

namespace BookStore.Service.Review
{
    public interface IReviewService
    {
        public Task<bool> AddReview(ReviewViewModel reviewViewModel);
        public Task<bool> DeleteReview(int id);
        public Task<List<Models.Review>> GetReviewsByBookId(int bookId);
        public Task<List<Models.Review>> GetReviewsByUserId(int id);
        public Task<int> GetBookIdByReviewId(int id);

    }
}
