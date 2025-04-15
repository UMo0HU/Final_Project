using BookStore.Data;
using BookStore.Service.Book;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Service.BookService
{
    public class BookService : IBookService
    {
        private readonly BookStoreDBContext _context;
        public BookService(BookStoreDBContext context)
        {
            _context = context;
        }
        public List<Models.Book> GetAllBooks()
        {
            return _context.Books
                .Include(b => b.Book_Categories)
                .ThenInclude(bc => bc.Category)
                .ToList();
        }
        public Models.Book GetBookDetails(int id)
        {
            return _context.Books
                .Include(b => b.Book_Categories)
                .ThenInclude(bc => bc.Category)
                .Include(b => b.reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefault(b => b.Id == id);
        }
    }
}
