using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Template.Models.ServiceModels.Admin
{
    public class UpdateUserRequest : IValidatableObject
    {
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Registration confirmed")]
        public bool RegistrationConfirmed { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Mobile number")]
        public string MobileNumber { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }

        public DateTime? Lockout_End { get; set; }

        public bool Is_Locked_Out { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Password != PasswordConfirm)
            {
                yield return new ValidationResult("Passwords do not match");
            }
        }
    }
}
