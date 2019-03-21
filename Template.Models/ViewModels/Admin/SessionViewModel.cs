using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ViewModels.Admin
{
    public class SessionViewModel : ViewModel
    {
        #region Constructors

        public SessionViewModel()
        {
            Logs = new List<SessionLogEntity>();
        }

        #endregion

        public DomainModels.SessionEntity Session { get; set; }

        public List<SessionLogEntity> Logs { get; set; }
    }
}
