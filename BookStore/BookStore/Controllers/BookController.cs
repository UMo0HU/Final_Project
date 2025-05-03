using AspNetCoreGeneratedDocument;
using BookStore.Data;
using BookStore.Models;
using BookStore.Service.Account;
using BookStore.Service.Book;
using BookStore.Service.Cart;
using BookStore.Service.Review;
using BookStore.Service.Wishlist;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IWishlistService _wishlistService;
        private readonly IReviewService _reviewService;
        private readonly IAccountService _accountService;
        private readonly ICartService _cartService;
        public BookController(IBookService bookService, IWishlistService wishlistService, IReviewService reviewService, IAccountService accountService, ICartService cartService)
        {
            _bookService = bookService;
            _wishlistService = wishlistService;
            _reviewService = reviewService;
            _accountService = accountService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _bookService.GetAllBooks();
            return View(books);
        }
        public async Task<IActionResult> Details(int id)
        {
            ReviewViewModel reviewViewModel = new ReviewViewModel();
            Book book = await _bookService.GetBookDetails(id);
            ViewBag.BookCategories = book.Book_Categories
                .Select(bc => bc.Category)
                .ToList();
            reviewViewModel.Book = book;
            ViewBag.BookInWishlist = await _wishlistService.IsBookInWishlist(id);
            ViewBag.BookInCart = await _cartService.BookInCart(id);
            var Reviews = await _reviewService.GetReviewsByBookId(id);
            var ratingGroup = Reviews.GroupBy(r => r.Rating)
                        .Select(g => new { Rating = g.Key, Count = g.Count() })
                        .OrderByDescending(g => g.Rating);
            if (ratingGroup.Count() == 0)
            {
                ViewBag.BookRating = 0;
                return View(reviewViewModel);
            }
            var weightedRating = ratingGroup.Sum(g => g.Rating * g.Count) / ratingGroup.Sum(g => g.Count);
            ViewBag.BookRating = Math.Round((double)weightedRating, 1);
            var userId = await _accountService.GetUserId();
            if(userId != null)
            {
                var userReview = await _reviewService.GetUserReview(id, userId);
                if(userReview != null)
                {
                    ViewBag.UserReview = userReview;
                }
            }
            return View(reviewViewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToWishlist(int id)
        {
            var result = await _wishlistService.AddToWishlist(id);
            return Json(new {success = result});
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveFromWishlist(int id)
        {
            var result = await _wishlistService.RemoveFromWishlist(id);
            return Json(new { success = result });
        }
    }
}
