using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Template.Models.ServiceModels.Admin
{
    public class CreateRoleRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Permissions")]
        public List<int> PermissionIds { get; set; }

        public CreateRoleRequest()
        {
            PermissionIds = new List<int>();
        }
    }
}
