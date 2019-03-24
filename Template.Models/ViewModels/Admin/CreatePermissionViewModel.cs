using Template.Models.ServiceModels.Admin.PermissionManagement;

namespace Template.Models.ViewModels.Admin
{
    public class CreatePermissionViewModel : ViewModel<CreatePermissionRequest>
    {
        #region Constructors

        public CreatePermissionViewModel(CreatePermissionRequest request) : base(request) { }

        public CreatePermissionViewModel() : this(new CreatePermissionRequest()) { }
        
        #endregion
    }
}
