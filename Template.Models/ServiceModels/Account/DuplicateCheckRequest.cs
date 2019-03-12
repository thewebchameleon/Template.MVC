using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Template.Models.ServiceModels
{
    public class DuplicateCheckRequest : IValidatableObject
    {
        /// <summary>
        /// UserID of the logged in user (optional)
        /// </summary>
        public int? UserId { get; set; }

        public string EmailAddress { get; set; }

        public string MobileNumber { get; set; }

        public string Username { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(EmailAddress) &&
                string.IsNullOrEmpty(MobileNumber) &&
                string.IsNullOrEmpty(Username)
            )
            {
                yield return new ValidationResult("Please provide at least 1 field to check for duplicate users");
            }
        }
    }
}
