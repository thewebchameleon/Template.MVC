using Template.Models.ServiceModels.Admin.PermissionManagement;

namespace Template.Models.ViewModels.Admin
{
    public class EditPermissionViewModel : ViewModel<UpdatePermissionRequest>
    {
        #region Constructors

        public EditPermissionViewModel(UpdatePermissionRequest request) : base(request) { }

        public EditPermissionViewModel() : this(new UpdatePermissionRequest()) { }

        #endregion

        public string Key { get; set; }
    }
}
