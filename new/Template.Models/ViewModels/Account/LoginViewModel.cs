using Template.Models.ServiceModels;

namespace Template.Models.ViewModels.Account
{
    public class LoginViewModel : ViewModel<LoginRequest>
    {
        #region Constructors

        public LoginViewModel(LoginRequest request) : base(request) { }

        public LoginViewModel() : this(new LoginRequest()) { }

        #endregion
    }
}
