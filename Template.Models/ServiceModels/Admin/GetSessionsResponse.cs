using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin
{
    public class GetSessionsResponse : ServiceResponse
    {
        public List<DomainModels.Session> Sessions { get; set; }

        public GetSessionsResponse()
        {
            Sessions = new List<DomainModels.Session>();
        }
    }
}
