using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Controllers.Entities
{
    public class Book
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        [FirstLetterCapital]
        public string Title { get; set; }
        public List<Comment> Comments { get; set; }
        public List<AuthorBook> AuthorsBooks { get; set; }
        public DateTime? PublishedDate { get; set; }
    }
}
