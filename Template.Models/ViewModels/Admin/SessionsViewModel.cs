using System.Collections.Generic;
using Template.Models.DomainModels;
using Template.Models.ServiceModels.Admin;

namespace Template.Models.ViewModels.Admin
{
    public class SessionsViewModel : ViewModel<GetSessionsRequest>
    {
        #region Constructors

        public SessionsViewModel(GetSessionsRequest request) : base(request) {
            Sessions = new List<SessionEntity>();
        }

        public SessionsViewModel() : this(new GetSessionsRequest()) { }

        #endregion

        public List<SessionEntity> Sessions { get; set; }

        public string SelectedFilter { get; set; }
    }
}
