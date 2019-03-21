using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin
{
    public class GetSessionEventManagementResponse : ServiceResponse
    {
        public List<SessionEventEntity> SessionEvents { get; set; }

        public GetSessionEventManagementResponse()
        {
            SessionEvents = new List<SessionEventEntity>();
        }
    }
}
