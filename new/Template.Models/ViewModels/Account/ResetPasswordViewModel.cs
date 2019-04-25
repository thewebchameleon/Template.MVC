using Template.Models.ServiceModels.Account;

namespace Template.Models.ViewModels.Account
{
    public class ResetPasswordViewModel : ViewModel<ResetPasswordRequest>
    {
        #region Constructors

        public ResetPasswordViewModel(ResetPasswordRequest request) : base(request) { }

        public ResetPasswordViewModel() : this(new ResetPasswordRequest()) { }

        #endregion
    }
}
