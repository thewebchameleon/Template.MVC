using System.Collections.Generic;
using Template.Models.ServiceModels.Admin;
using Template.Models.ServiceModels.Admin.SessionManagement;

namespace Template.Models.ViewModels.Admin
{
    public class SessionsViewModel : ViewModel<GetSessionsRequest>
    {
        #region Constructors

        public SessionsViewModel(GetSessionsRequest request) : base(request) {
            Sessions = new List<Session>();
        }

        public SessionsViewModel() : this(new GetSessionsRequest()) { }

        #endregion

        public List<Session> Sessions { get; set; }

        public string SelectedFilter { get; set; }
    }
}
