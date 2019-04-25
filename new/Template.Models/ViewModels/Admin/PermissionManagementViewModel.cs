using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ViewModels.Admin
{
    public class PermissionManagementViewModel : ViewModel
    {
        #region Constructors

        public PermissionManagementViewModel()
        {
            Permissions = new List<PermissionEntity>();
        }

        #endregion

        public List<PermissionEntity> Permissions { get; set; }
    }
}
