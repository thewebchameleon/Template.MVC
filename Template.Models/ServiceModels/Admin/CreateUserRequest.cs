using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Template.Models.ServiceModels.Admin
{
    public class CreateUserRequest : IValidatableObject
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string EmailAddress { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Mobile number")]
        public string MobileNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string PasswordConfirm { get; set; }

        public List<int> Claims { get; set; }

        public List<int> Roles { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Password != PasswordConfirm)
            {
                yield return new ValidationResult("Password and confirm password do not match");
            }
        }
    }
}
