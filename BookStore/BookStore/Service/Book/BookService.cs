using BookStore.Data;
using BookStore.Service.Book;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
using BookStore.ViewModels;
using Microsoft.Identity.Client;

namespace BookStore.Service.BookService
{
    public class BookService : IBookService
    {
        private readonly BookStoreDBContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public BookService(BookStoreDBContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
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
                .Include(b => b.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(b => b.Id == id)!;
        }

        public async Task AddBook(BookViewModel bookViewModel)
        {
            var fileName = string.Empty;
            if (bookViewModel.ClientFile != null)
            {
                string myUpload = Path.Combine(_hostingEnvironment.WebRootPath, "book-images");
                fileName = bookViewModel.ClientFile.FileName;
                string fullPath = Path.Combine(myUpload, fileName);
                await bookViewModel.ClientFile.CopyToAsync(new FileStream(fullPath, FileMode.Create));
            }
            var book = new Models.Book
            {
                Title = bookViewModel.Title,
                Author = bookViewModel.Author,
                Description = bookViewModel.Description,
                Price = bookViewModel.Price,
                Stock = bookViewModel.Stock,
                Img = fileName
            };
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            foreach(var categoryId in bookViewModel.SelectedCategoryIds)
            {
                var bookCategory = new BookCategory
                {
                    BookId = book.Id,
                    CategoryId = categoryId
                };
                await _context.BookCategories.AddAsync(bookCategory);
            }
            await _context.SaveChangesAsync();
        }

        public async Task EditBook(BookViewModel bookViewModel)
        {
            var book = await _context.Books.FindAsync(bookViewModel.Id);

            book.Title = bookViewModel.Title;
            book.Author = bookViewModel.Author;
            book.Description = bookViewModel.Description;
            book.Price = bookViewModel.Price;
            book.Stock = bookViewModel.Stock;

            if (bookViewModel.ClientFile != null)
            {
                string myUpload = Path.Combine(_hostingEnvironment.WebRootPath, "book-images");
                string fileName = bookViewModel.ClientFile.FileName;
                string fullPath = Path.Combine(myUpload, fileName);
                await bookViewModel.ClientFile.CopyToAsync(new FileStream(fullPath, FileMode.Create));
                book.Img = fileName;
            }

            var existingBookCategories = await _context.BookCategories
                .Where(bc => bc.BookId == book.Id)
                .ToListAsync();
            _context.BookCategories.RemoveRange(existingBookCategories);

            foreach (var categoryId in bookViewModel.SelectedCategoryIds)
            {
                var bookCategory = new BookCategory
                {
                    BookId = book.Id,
                    CategoryId = categoryId
                };
                await _context.BookCategories.AddAsync(bookCategory);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                var bookCategories = _context.BookCategories.Where(bc => bc.BookId == id);
                _context.BookCategories.RemoveRange(bookCategories);

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Models.Book>> Search(string keyword)
        {
            var matchesByTitle = _context.Books.Where(b => b.Title.Contains(keyword));
            var matchesByAuthor = _context.Books.Where(b => b.Author.Contains(keyword));

            return await matchesByTitle.Concat(matchesByAuthor).Distinct().ToListAsync();

        }


    }

}
