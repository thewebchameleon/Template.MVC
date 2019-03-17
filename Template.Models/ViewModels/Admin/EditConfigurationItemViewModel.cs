using Template.Models.ServiceModels.Admin;

namespace Template.Models.ViewModels.Admin
{
    public class EditConfigurationItemViewModel : ViewModel<UpdateConfigurationItemRequest>
    {
        #region Constructors

        public EditConfigurationItemViewModel(UpdateConfigurationItemRequest request) : base(request) { }

        public EditConfigurationItemViewModel() : this(new UpdateConfigurationItemRequest()) { }

        #endregion

        public string Key { get; set; }
    }
}
