using System.ComponentModel.DataAnnotations;

namespace Template.Models.ServiceModels.Admin
{
    public class UpdateSessionEventRequest
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
