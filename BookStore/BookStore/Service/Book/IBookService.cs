namespace BookStore.Service.Book
{
    public interface IBookService
    {
        public Task<List<Models.Book>> GetAllBooks();
        public Task<Models.Book> GetBookDetails(int id);
        public Task AddBook(Models.Book book);

    }
}
