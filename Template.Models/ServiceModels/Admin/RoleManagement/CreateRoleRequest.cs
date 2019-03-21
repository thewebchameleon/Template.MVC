using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Template.Models.ServiceModels.Admin
{
    public class CreateRoleRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Claims")]
        public List<int> ClaimIds { get; set; }

        public CreateRoleRequest()
        {
            ClaimIds = new List<int>();
        }
    }
}
