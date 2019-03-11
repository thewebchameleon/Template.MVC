using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Template.Models.ServiceModels
{
    public class RegisterRequest : IValidatableObject
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Password != PasswordConfirm)
            {
                yield return new ValidationResult("Passwords do not match");
            }
        }
    }
}
