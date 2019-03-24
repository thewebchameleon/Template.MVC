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

        [Display(Name = "Permissions")]
        public List<int> PermissionIds { get; set; }

        public UpdateRoleRequest()
        {
            PermissionIds = new List<int>();
        }
    }
}
