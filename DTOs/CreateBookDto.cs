using System.ComponentModel.DataAnnotations;

namespace BookCatalog.DTOs
{
    public class CreateBookDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Genre { get; set; }
        [Required]
        public int PublishedYear { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
