using Template.Models.ServiceModels.Account;

namespace Template.Models.ViewModels.Account
{
    public class ProfileViewModel : ViewModel<UpdateProfileRequest>
    {
        #region Constructors

        public ProfileViewModel(UpdateProfileRequest request) : base(request) { }

        public ProfileViewModel() : this(new UpdateProfileRequest()) { }

        #endregion


    }
}
