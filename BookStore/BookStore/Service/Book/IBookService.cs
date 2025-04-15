namespace BookStore.Service.Book
{
    public interface IBookService
    {
        public List<Models.Book> GetAllBooks();
        public Models.Book GetBookDetails(int id);

    }
}
