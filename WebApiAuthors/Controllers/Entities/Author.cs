using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Controllers.Entities
{
    public class Author: IValidatableObject
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(maximumLength: 20, ErrorMessage = "{0} max length is {1}")]
        [MinLength(3, ErrorMessage = "{0} minm lenght is {1}")]
        //[FirstLetterCapital]
        public string Name { get; set; }
        public List<Book> Books { get; set; }
        [Range(18, 120, ErrorMessage = "{0} must be between {1} and {2}")]
        [NotMapped]
        public int Age { get; set; }
        [CreditCard]
        [NotMapped]
        public string CreditCard { get; set; }
        [Url]
        [NotMapped]
        public string URL { get; set; }
        [NotMapped]
        public int Minor { get; set; }
        [NotMapped]
        public int Major { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                var firstChar = Name[0].ToString();
                if (firstChar!=firstChar.ToUpper())yield return new ValidationResult("Name must start with capital letter", 
                    new string[] { nameof(Name) });
            }
            if (Minor>Major) yield return new ValidationResult("Minor is invalid",
                    new string[] { nameof(Minor) });
        }
    }
}
