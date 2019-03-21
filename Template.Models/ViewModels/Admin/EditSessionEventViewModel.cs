using Template.Models.ServiceModels.Admin;

namespace Template.Models.ViewModels.Admin
{
    public class EditSessionEventViewModel : ViewModel<UpdateSessionEventRequest>
    {
        #region Constructors

        public EditSessionEventViewModel(UpdateSessionEventRequest request) : base(request) { }

        public EditSessionEventViewModel() : this(new UpdateSessionEventRequest()) { }

        #endregion

        public string Key { get; set; }
    }
}
