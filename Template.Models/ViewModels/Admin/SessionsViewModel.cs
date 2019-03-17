using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ViewModels.Admin
{
    public class SessionsViewModel : ViewModel
    {
        public List<Session> Sessions { get; set; }

        public SessionsViewModel()
        {
            Sessions = new List<Session>();
        }
    }
}
