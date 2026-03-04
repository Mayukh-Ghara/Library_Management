using System.ComponentModel.DataAnnotations;

namespace LibraryWebAPI.Models
{
    public class Book
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string Author { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string ISBN { get; set; } = null!;

        public int PublishedYear { get; set; }

        public int CopiesAvailable { get; set; }
    }
}