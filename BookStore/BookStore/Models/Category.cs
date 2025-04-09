namespace BookStore.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<BookCategory> Book_Categories { get; set; }
    }
}
