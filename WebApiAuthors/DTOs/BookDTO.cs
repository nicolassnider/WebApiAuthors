using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Controllers.Entities;
using WebApiAuthors.Validations;

namespace WebApiAuthors.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }
        [StringLength(maximumLength: 20, ErrorMessage = "{0} max length is {1}")]
        [MinLength(3, ErrorMessage = "{0} minm lenght is {1}")]
        [FirstLetterCapital]
        public string Title { get; set; }
        
        public List<CommentDTO> Comments { get; set; }
    }
}
