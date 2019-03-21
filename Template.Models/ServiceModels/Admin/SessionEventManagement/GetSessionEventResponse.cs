using Template.Models.DomainModels;

namespace Template.Models.ServiceModels.Admin
{
    public class GetSessionEventResponse : ServiceResponse
    {
        public SessionEventEntity SessionEvent { get; set; }
    }
}
