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
        public Task<List<Models.Book>> GetBooksForCategory(int categoryId);
        public Task<List<Models.Book>> Search(string keyword);

        public Task<List<Models.Book>> GetBookRecommendation();
        public Task<List<string>> GetAuthorsRecommendation();

        public Task<List<Models.Book>> GetBooksByAuthor(string author);
    }
}
