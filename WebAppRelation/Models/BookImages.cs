using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppRelation.Models
{
    public class BookImages
    {
        public int Id { get; set; }
        public string? ImgUrl { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
