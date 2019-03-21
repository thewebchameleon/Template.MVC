using Template.Models.ServiceModels.Admin.ClaimManagement;

namespace Template.Models.ViewModels.Admin
{
    public class CreateClaimViewModel : ViewModel<CreateClaimRequest>
    {
        #region Constructors

        public CreateClaimViewModel(CreateClaimRequest request) : base(request) { }

        public CreateClaimViewModel() : this(new CreateClaimRequest()) { }
        
        #endregion
    }
}
