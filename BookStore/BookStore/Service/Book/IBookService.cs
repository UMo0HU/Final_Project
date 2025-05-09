using BookStore.ViewModels;

namespace BookStore.Service.Book
{
    public interface IBookService
    {
        public Task<List<Models.Book>> GetAllBooks();
        public Task<Models.Book> GetBookDetails(int id);
        public Task AddBook(BookViewModel bookViewModel);
        public Task EditBook(BookViewModel bookViewModel);
        public Task DeleteBook(int id);
<<<<<<< HEAD
        public Task<List<Models.Book>> GetBooksForCategory(int categoryId);
=======
        public Task<List<Models.Book>> Search(string keyword);
>>>>>>> b146043d546aede81c7549d4da796170cdf44756

    }
}
