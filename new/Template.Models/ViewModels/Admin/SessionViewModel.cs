using System.Collections.Generic;
using Template.Models.DomainModels;
using Template.Models.ServiceModels.Admin;

namespace Template.Models.ViewModels.Admin
{
    public class SessionViewModel : ViewModel
    {
        #region Constructors

        public SessionViewModel()
        {
            Logs = new List<SessionLog>();
        }

        #endregion

        public UserEntity User { get; set; }

        public SessionEntity Session { get; set; }

        public List<SessionLog> Logs { get; set; }
    }
}
