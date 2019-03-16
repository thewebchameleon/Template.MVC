using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ViewModels.Admin
{
    public class RoleManagementViewModel
    {
        public List<Role> Roles { get; set; }

        public RoleManagementViewModel()
        {
            Roles = new List<Role>();
        }
    }
}
