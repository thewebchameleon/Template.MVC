using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin
{
    public class SessionLog
    {
        public SessionLogEntity Entity { get; set; }

        public List<SessionEventEntity> Events { get; set; }
    }
}
