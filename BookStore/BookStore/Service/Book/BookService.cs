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
        public Task<List<Models.Book>> GetAllBooks()
        {
            return _context.Books
                .Include(b => b.Book_Categories)
                .ThenInclude(bc => bc.Category)
                .ToListAsync();
        }
        public Task<Models.Book> GetBookDetails(int id)
        {
            return _context.Books
                .Include(b => b.Book_Categories)
                .ThenInclude(bc => bc.Category)
                .Include(b => b.reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(b => b.Id == id)!;
        }

        public async Task AddBook(Models.Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
        }



    }
      
}
