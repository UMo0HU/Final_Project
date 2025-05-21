using BookStore.Data;
using BookStore.Models;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace BookStore.Service.Review
{
    public class ReviewService : IReviewService
    {
        private readonly BookStoreDBContext _context;
        private readonly UserManager<Models.User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;

        public ReviewService(BookStoreDBContext context, UserManager<Models.User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext?.User;
        }
        public async Task<bool> AddReview(ReviewViewModel reviewViewModel)
        {
            if (_user != null || _user.Identity.IsAuthenticated)
            {
                var review = new Models.Review
                {
                    BookId = reviewViewModel.BookId,
                    UserId = _userManager.GetUserId(_user),
                    Content = reviewViewModel.Content,
                    Rating = reviewViewModel.Rating,
                };

                await _context.Reviews.AddAsync(review);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteReview(int bookId, string userId)
        {
            if (_user != null || _user.Identity.IsAuthenticated)
            {
                var review = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId);
                if(review != null)
                {
                    _context.Reviews.Remove(review);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
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

        public async Task<Models.Review> GetUserReview(int bookId, string userId)
        {
            var review = await _context.Reviews
                .FirstOrDefaultAsync(r => r.BookId == bookId && r.UserId == userId);
            return review;
        }
    }
}
