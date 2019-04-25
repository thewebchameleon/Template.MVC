using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin
{
    public class SessionLogEvent
    {
        public SessionEventEntity Event { get; set; }

        public string Message { get; set; }
    }
}
