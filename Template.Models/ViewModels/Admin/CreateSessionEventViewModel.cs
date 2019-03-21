using Template.Models.ServiceModels.Admin;

namespace Template.Models.ViewModels.Admin
{
    public class CreateSessionEventViewModel : ViewModel<CreateSessionEventRequest>
    {
        #region Constructors

        public CreateSessionEventViewModel(CreateSessionEventRequest request) : base(request) { }

        public CreateSessionEventViewModel() : this(new CreateSessionEventRequest()) { }

        #endregion
    }
}
