using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ViewModels.Admin
{
    public class RoleManagementViewModel : ViewModel
    {
        public List<RoleEntity> Roles { get; set; }

        public RoleManagementViewModel()
        {
            Roles = new List<RoleEntity>();
        }
    }
}
