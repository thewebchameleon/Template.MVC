using Template.Models.ServiceModels.Admin;

namespace Template.Models.ViewModels.Admin
{
    public class CreateConfigurationItemViewModel : ViewModel<CreateConfigurationItemRequest>
    {
        #region Constructors

        public CreateConfigurationItemViewModel(CreateConfigurationItemRequest request) : base(request) { }

        public CreateConfigurationItemViewModel() : this(new CreateConfigurationItemRequest()) { }

        #endregion
    }
}
