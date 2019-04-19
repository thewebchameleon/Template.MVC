using System.ComponentModel.DataAnnotations;

namespace Template.Models.ServiceModels
{
    public class ForgotPasswordRequest
    {
        [Required]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
    }
}
