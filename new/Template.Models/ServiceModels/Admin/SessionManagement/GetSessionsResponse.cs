using System.Collections.Generic;

namespace Template.Models.ServiceModels.Admin
{
    public class GetSessionsResponse : ServiceResponse
    {
        public List<SessionManagement.Session> Sessions { get; set; }

        public string SelectedFilter { get; set; }

        public GetSessionsResponse()
        {
            Sessions = new List<SessionManagement.Session>();
        }
    }
}
