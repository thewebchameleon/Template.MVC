using Template.Models.ServiceModels.Admin;

namespace Template.Models.ViewModels.Admin
{
    public class CreateUserViewModel : ViewModel<CreateUserRequest>
    {
        #region Constructors

        public CreateUserViewModel(CreateUserRequest request) : base(request) { }

        public CreateUserViewModel() : base(new CreateUserRequest()) { }

        #endregion
    }
}
