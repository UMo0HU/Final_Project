using BookStore.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookStore.ViewModels
{
    public class ReviewViewModel
    {
        public int BookId { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }
        public Book? Book { get; set; }
    }
}
