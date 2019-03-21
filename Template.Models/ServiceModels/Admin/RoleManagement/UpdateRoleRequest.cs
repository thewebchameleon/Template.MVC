using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Template.Models.ServiceModels.Admin
{
    public class UpdateRoleRequest
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Claims")]
        public List<int> ClaimIds { get; set; }

        public UpdateRoleRequest()
        {
            ClaimIds = new List<int>();
        }
    }
}
