using Template.Models.ServiceModels.Admin;

namespace Template.Models.ViewModels.Admin
{
    public class EditUserViewModel : ViewModel<UpdateUserRequest>
    {
        #region Constructors

        public EditUserViewModel(UpdateUserRequest request) : base(request) { }

        public EditUserViewModel() : base(new UpdateUserRequest()) { }

        #endregion
    }
}
