using System.Collections.Generic;
using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin
{
    public class SessionLog
    {
        public string Controller { get; set; }

        public string Action { get; set; }

        public List<SessionEventEntity> Events { get; set; }
    }
}
