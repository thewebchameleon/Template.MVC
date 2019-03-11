using Template.Models.ServiceModels;

namespace Template.Models.ViewModels.Account
{
    public class RegisterViewModel : BaseViewModel<RegisterRequest>
    {
        #region Constructors

        public RegisterViewModel(RegisterRequest request) : base(request) { }

        public RegisterViewModel() { }

        #endregion
    }
}
