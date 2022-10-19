using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.DTOs
{
    public class BookCreationDTO
    {
        [FirstLetterCapital]
        [StringLength(maximumLength:250)]
        public string Title { get; set; }
        public List<int> AuthorIds { get; set; }
        public DateTime PublishedDate { get; set; }
    }
}
