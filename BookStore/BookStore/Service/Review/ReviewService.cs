using BookStore.Data;
using BookStore.Models;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookStore.Service.Review
{
    public class ReviewService : IReviewService
    {
        private readonly BookStoreDBContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ReviewService(BookStoreDBContext context, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> AddReview(ReviewViewModel reviewViewModel)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null || user.Identity.IsAuthenticated)
            {
                var review = new Models.Review
                {
                    BookId = reviewViewModel.BookId,
                    UserId = _userManager.GetUserId(user),
                    Content = reviewViewModel.Content,
                    Rating = reviewViewModel.Rating,
                };

                await _context.Reviews.AddAsync(review);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteReview(int id)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null || user.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(user);
                var reviewerId = await _context.Reviews
                    .Where(r => r.Id == id)
                    .Select(r => r.UserId)
                    .FirstOrDefaultAsync();
                if(reviewerId != userId)
                {
                    return false;
                }
                var review = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<Models.Review>> GetReviewsByBookId(int bookId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
            return reviews;
        }

        public async Task<List<Models.Review>> GetReviewsByUserId(int bookId)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user != null || user.Identity.IsAuthenticated)
            {
                var userReviewsId = await _context.Reviews
                    .Include(r => r.User)
                    .Where(r => r.UserId == _userManager.GetUserId(user) && r.BookId == bookId)
                    .ToListAsync();
                
                return userReviewsId;
            }
            return null;
        }

        public async Task<int> GetBookIdByReviewId(int reviewId)
        {
            var bookId = await _context.Reviews
                .Where(r => r.Id == reviewId)
                .Select(r => r.BookId)
                .FirstOrDefaultAsync();
            return bookId;
        }
    }
}
