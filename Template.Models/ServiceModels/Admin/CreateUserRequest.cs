using System.ComponentModel.DataAnnotations;

namespace Template.Models.ServiceModels.Admin
{
    public class CreateUserRequest
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

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string PasswordConfirm { get; set; }
    }
}
