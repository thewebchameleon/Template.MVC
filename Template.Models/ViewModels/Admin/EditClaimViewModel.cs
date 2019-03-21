using Template.Models.ServiceModels.Admin.ClaimManagement;

namespace Template.Models.ViewModels.Admin
{
    public class EditClaimViewModel : ViewModel<UpdateClaimRequest>
    {
        #region Constructors

        public EditClaimViewModel(UpdateClaimRequest request) : base(request) { }

        public EditClaimViewModel() : this(new UpdateClaimRequest()) { }

        #endregion

        public string Key { get; set; }
    }
}
