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
            RolesLookup = new List<SelectListItem>();
        }

        #endregion

        public List<SelectListItem> RolesLookup { get; set; }
    }
}
