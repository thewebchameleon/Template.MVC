using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ViewModels.Admin
{
    public class UserManagementViewModel : ViewModel
    {
        public List<UserEntity> Users { get; set; }

        public UserManagementViewModel()
        {
            Users = new List<UserEntity>();
        }
    }
}
