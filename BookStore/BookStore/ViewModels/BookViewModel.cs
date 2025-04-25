using BookStore.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.ViewModels
{
    public class BookViewModel
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        [NotMapped]
        [DisplayName("Book Image")]
        public IFormFile? ClientFile { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        [DisplayName("Categories")]
        public List<int> SelectedCategoryIds { get; set; }
        public List<Category>? AllCategories { get; set; }
    }
}
