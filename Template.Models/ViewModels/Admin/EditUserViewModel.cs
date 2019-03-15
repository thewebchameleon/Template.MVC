using System.Collections.Generic;
using Template.Models.DomainModels;
using Template.Models.ServiceModels.Admin;

namespace Template.Models.ViewModels.Admin
{
    public class EditUserViewModel : ViewModel<UpdateUserRequest>
    {
        #region Constructors

        public EditUserViewModel(UpdateUserRequest request) : base(request) { }

        public EditUserViewModel() : this(new UpdateUserRequest())
        {
            ClaimsLookup = new List<Claim>();
            RolesLookup = new List<Role>();
        }

        #endregion

        public List<Claim> ClaimsLookup { get; set; }

        public List<Role> RolesLookup { get; set; }
    }
}
