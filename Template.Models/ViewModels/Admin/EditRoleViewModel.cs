using System.Collections.Generic;
using Template.Models.DomainModels;
using Template.Models.ServiceModels.Admin;

namespace Template.Models.ViewModels.Admin
{
    public class EditRoleViewModel : ViewModel<UpdateRoleRequest>
    {
        #region Constructors

        public EditRoleViewModel(UpdateRoleRequest request) : base(request) { }

        public EditRoleViewModel() : this(new UpdateRoleRequest()) { }

        #endregion

        public List<ClaimEntity> ClaimsLookup { get; set; }
    }
}
