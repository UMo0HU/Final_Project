using BookStore.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.ViewModels
{
    public class BookViewModel
    {
        public int? Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Description { get; set; }
        [NotMapped]
        [DisplayName("Book Image")]
        public IFormFile? ClientFile { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        [Required]
        public int Stock { get; set; }
        [Required]
        [DisplayName("Categories")]
        public List<int> SelectedCategoryIds { get; set; }
        public List<Category>? AllCategories { get; set; }
    }
}
