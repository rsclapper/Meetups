using System.ComponentModel.DataAnnotations;

namespace MeetupAuthors.Models
{
    public class BookCreateDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int AuthorId { get; set; }
    }
}