using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin
{
    public class GetSessionResponse : ServiceResponse
    {
        public SessionEntity Session { get; set; }

        public List<SessionLog> Logs { get; set; }

        public GetSessionResponse()
        {
            Logs = new List<SessionLog>();
        }
    }
}
