using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin
{
    public class GetSessionResponse : ServiceResponse
    {
        public DomainModels.SessionEntity Session { get; set; }

        public List<SessionLogEntity> Logs { get; set; }

        public GetSessionResponse()
        {
            Logs = new List<SessionLogEntity>();
        }
    }
}
