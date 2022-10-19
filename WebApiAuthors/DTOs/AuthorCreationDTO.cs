using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.DTOs
{
    public class AuthorCreationDTO
    {
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(maximumLength: 20, ErrorMessage = "{0} max length is {1}")]
        [MinLength(3, ErrorMessage = "{0} minm lenght is {1}")]
        [FirstLetterCapital]
        public string Name { get; set; }
    }
}
