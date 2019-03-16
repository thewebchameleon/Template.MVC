using System.Collections.Generic;
using Template.Models.DomainModels;
using Template.Models.ServiceModels.Admin;

namespace Template.Models.ViewModels.Admin
{
    public class CreateRoleViewModel : ViewModel<CreateRoleRequest>
    {
        #region Constructors

        public CreateRoleViewModel(CreateRoleRequest request) : base(request) { }

        public CreateRoleViewModel() : this(new CreateRoleRequest()) { }

        #endregion

        public List<Claim> ClaimsLookup { get; set; }
    }
}
