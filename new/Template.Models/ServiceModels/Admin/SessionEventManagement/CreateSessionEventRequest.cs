using System.ComponentModel.DataAnnotations;

namespace Template.Models.ServiceModels.Admin
{
    public class CreateSessionEventRequest
    {
        [Required]
        public string Key { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
