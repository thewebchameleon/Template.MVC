using Template.Models.ServiceModels;

namespace Template.Models.ViewModels.Account
{
    public class ForgotPasswordViewModel : ViewModel<ForgotPasswordRequest>
    {
        #region Constructors

        public ForgotPasswordViewModel(ForgotPasswordRequest request) : base(request) { }

        public ForgotPasswordViewModel() { }

        #endregion
    }
}
