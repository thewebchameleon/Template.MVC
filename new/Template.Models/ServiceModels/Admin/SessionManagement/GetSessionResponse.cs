using System.Collections.Generic;
using Template.Models.DomainModels;
using Template.Models.ServiceModels.Session;

namespace Template.Models.ServiceModels.Admin
{
    public class GetSessionResponse : ServiceResponse
    {
        public UserEntity User { get; set; }

        public SessionEntity Session { get; set; }

        public List<SessionLog> Logs { get; set; }

        public GetSessionResponse()
        {
            Logs = new List<SessionLog>();
        }
    }
}
