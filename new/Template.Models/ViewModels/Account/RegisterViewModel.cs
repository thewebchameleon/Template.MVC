using Template.Models.ServiceModels;

namespace Template.Models.ViewModels.Account
{
    public class RegisterViewModel : ViewModel<RegisterRequest>
    {
        #region Constructors

        public RegisterViewModel(RegisterRequest request) : base(request) { }

        public RegisterViewModel() : this(new RegisterRequest()) { }

        #endregion
    }
}
