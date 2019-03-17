using System.Collections.Generic;
using Template.Models.DomainModels;
using Template.Models.ServiceModels.Account;

namespace Template.Models.ViewModels.Account
{
    public class ProfileViewModel : ViewModel<UpdateProfileRequest>
    {
        #region Constructors

        public ProfileViewModel(UpdateProfileRequest request) : base(request)
        {
            Roles = new List<Role>();
        }

        public ProfileViewModel() : this(new UpdateProfileRequest()) { }

        #endregion

        public List<Role> Roles { get; set; }
    }
}
