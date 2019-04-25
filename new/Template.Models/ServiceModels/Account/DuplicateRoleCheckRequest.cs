using System.ComponentModel.DataAnnotations;

namespace Template.Models.ServiceModels
{
    public class DuplicateRoleCheckRequest
    {
        [Required]
        public string Name { get; set; }
    }
}
