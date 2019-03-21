using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ViewModels.Admin
{
    public class SessionEventManagementViewModel : ViewModel
    {
        #region Constructors

        public SessionEventManagementViewModel()
        {
            SessionEvents = new List<SessionEventEntity>();
        }

        #endregion

        public List<SessionEventEntity> SessionEvents { get; set; }
    }
}
