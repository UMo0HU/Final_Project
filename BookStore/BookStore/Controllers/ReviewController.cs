using BookStore.Models;
using BookStore.Service.Book;
using BookStore.Service.Review;
using BookStore.Service.Wishlist;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace BookStore.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IBookService _bookService;
        private readonly IWishlistService _wishlistService;
        public ReviewController(IReviewService reviewService, IBookService bookService, IWishlistService wishlistService)
        {
            _reviewService = reviewService;
            _bookService = bookService;
            _wishlistService = wishlistService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(ReviewViewModel reviewViewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data" });
            }
            var result = await _reviewService.AddReview(reviewViewModel);
            return Json(new { success = result });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int bookId, string userId)
        {
            var result = await _reviewService.DeleteReview(bookId, userId);
            return Json(new { success = result });
        }
    }
}
