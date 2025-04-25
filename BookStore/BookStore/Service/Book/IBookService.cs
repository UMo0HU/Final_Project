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



    }
}
