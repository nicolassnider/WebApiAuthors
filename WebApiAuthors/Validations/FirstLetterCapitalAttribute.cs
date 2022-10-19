using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.Validations
{
    public class FirstLetterCapitalAttribute:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value==null||string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }
            var firstChar = value.ToString()[0].ToString();
            Console.WriteLine("-------");
            Console.WriteLine(validationContext);
            if (firstChar != firstChar.ToUpper()) return new ValidationResult($"{validationContext.MemberName} must start with capital letter");
            return base.IsValid(value, validationContext);
        }
    }
}
