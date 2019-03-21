using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ViewModels.Admin
{
    public class ClaimManagementViewModel : ViewModel
    {
        #region Constructors

        public ClaimManagementViewModel()
        {
            Claims = new List<ClaimEntity>();
        }

        #endregion

        public List<ClaimEntity> Claims { get; set; }
    }
}
